#nullable enable

using Godot;
using System;
using System.Collections.Generic;

public enum TransferAmount
{
	Full,
	Half,
	Single
}

public sealed class Inventory
{
	public event Action? Changed;
	private readonly ItemStack?[] slots;
	public ItemStack? GetStackAt(int index) => slots[index];
	public bool IsSlotEmpty(int index) => slots[index] == null;

	internal void NotifyChanged()
	{
		Changed?.Invoke();
	}

	public static void Transfer(Inventory sourceInventory, int sourceIndex, Inventory targetInventory, int targetIndex, TransferAmount amountMode)
	{
		// NOTE:
		// sourceInventory is expected to be the cursor inventory (1-slot)
		// so sourceStack.Quantity already represents the intended transfer amount
		var sourceStack = sourceInventory.GetStackAt(sourceIndex);
		var targetStack = targetInventory.GetStackAt(targetIndex);

		// Case 1: Same inventory & same slot -> do nothing
		if (ReferenceEquals(sourceInventory, targetInventory) && sourceIndex == targetIndex)
		{
			return;
		}

		// Case 2: Try merge
		if (targetStack != null && sourceStack != null && targetStack.Item.Name == sourceStack.Item.Name)
		{
			int amountToMove = amountMode switch
			{
				TransferAmount.Full   => sourceStack.Quantity,
				TransferAmount.Half   => (sourceStack.Quantity + 1) / 2,
				TransferAmount.Single => 1,
				_ => 0
			};

			amountToMove = Math.Min(amountToMove, sourceStack.Quantity);
			int leftover = targetStack.Add(amountToMove);
			int moved = amountToMove - leftover;

			sourceStack.Quantity -= moved;
			if (sourceStack.Quantity <= 0)
			{
				sourceInventory.ClearSlot(sourceIndex);
			}

			// Notify both inventories of change
			targetInventory.NotifyChanged();
			sourceInventory.NotifyChanged();
			return;
		}

		// Case 3: Source slot empty -> pick up
		if (sourceStack == null && targetStack != null)
		{
			int amount = amountMode switch
			{
				TransferAmount.Full => targetStack!.Quantity,
				TransferAmount.Half => (targetStack!.Quantity + 1) / 2,
				TransferAmount.Single => 1,
				_ => 0
			};
			IItem item = targetStack!.Item;
			sourceInventory.SetSlot(sourceIndex, item, amount);
			targetStack.Quantity -= amount;
			if (targetStack.Quantity <= 0)
			{
				targetInventory.ClearSlot(targetIndex);
			}
			
			// Notify both inventories of change
			targetInventory.NotifyChanged();
			sourceInventory.NotifyChanged();
			return;
		}

		// Case 4: Swap
		Swap(sourceInventory, sourceIndex, targetInventory, targetIndex);
	}

	public Inventory(int size)
	{
		slots = new ItemStack[size];
	}

	internal void ClearSlot(int index)
	{
		slots[index] = null;
	}

	private void SetSlot(int index, IItem item, int amount)
	{
		slots[index] = new ItemStack(item, amount);
		// Clear slot if amount is zero or less
		if (amount <= 0)
		{
			ClearSlot(index);
		}
	}

	internal static void Swap(Inventory a, int indexA, Inventory b, int indexB)
	{
		var temp = a.slots[indexA];
		a.slots[indexA] = b.slots[indexB];
		b.slots[indexB] = temp;
		// Notify both inventories of change
		a.NotifyChanged();
		b.NotifyChanged();
	}

	public bool TryGetItemAt(int index, out IItem? item)
	{
		if (slots[index] is ItemStack stack)
		{
			item = stack.Item;
			return true;
		}

		item = null;
		return false;
	}

	public int AddItemToSlot(IItem item, int amount, int index)
	{
		var slot = slots[index];

		// Empty slot
		if (slot == null)
		{
			GD.Print("Adding new stack to empty slot");
			int stackAmount = Math.Min(item.MaxStackSize, amount);
			slots[index] = new ItemStack(item, stackAmount);
			return amount - stackAmount;
		}

		// Different item in slot
		if (slot.Item.Name != item.Name)
		{
			GD.Print("Cant add item to slot: different item present");
			return amount; // Cant add item here
		}

		// Same item in slot
		GD.Print("Adding to existing stack in slot");
		return slot.Add(amount);
	}

	public int AddItem(IItem item, int amount)
	{
		//Fill existing stacks
		foreach (var slot in slots)
		{
			if (slot?.Item.Name == item.Name)
			{
				GD.Print("Filling existing stack");
				amount = slot.Add(amount);
				if (amount == 0)
				{
					return 0;
				}
			}
		}

		for (int i = 0; i < slots.Length; i++)
		{
			if (slots[i] == null)
			{
				GD.Print("Creating new stack in empty slot");
				int stackAmount = Math.Min(item.MaxStackSize, amount);
				slots[i] = new ItemStack(item, stackAmount);
				amount -= stackAmount;

				if (amount == 0)
				{
					return 0;
				}
			}
		}

		GD.Print("Not enough space to add all items");
		return amount; // Leftovers that didnt fit
	}
}
