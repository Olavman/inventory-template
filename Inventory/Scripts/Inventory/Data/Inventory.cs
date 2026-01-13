#nullable enable

using Godot;
using System;
using System.Collections.Generic;

public sealed class Inventory
{
	private readonly ItemStack?[] slots;
	public ItemStack? GetStackAt(int index) => slots[index];
	public bool IsSlotEmpty(int index) => slots[index] == null;

	public static void Transfer(
		DragPayload payload,
		Inventory target,
		int targetIndex)
	{
		var source = payload.Source;
		int sourceIndex = payload.SourceIndex;

		var sourceStack = source.GetStackAt(sourceIndex);
		if (sourceStack == null)
		{
			return; //Nothing to transfer
		}

		if (sourceStack.Item.Name != payload.Item.Name)
		{
			return; //Item mismatch
		}

		int amountToMove = Math.Min(payload.Amount, sourceStack.Quantity);

		// Case 1: Same inventory, same slot -> do nothing
		if (ReferenceEquals(source, target) && sourceIndex == targetIndex)
		{
			return;
		}

		// Case 2: Try merge
		var targetStack = target.GetStackAt(targetIndex);
		if (targetStack != null &&
			targetStack.Item.Name == payload.Item.Name)
		{
			int leftover = targetStack.Add(amountToMove);
			int moved = amountToMove - leftover;

			sourceStack.Quantity -= moved;
			if (sourceStack.Quantity == 0)
			{
				source.ClearSlot(sourceIndex);
			}

			return;
		}

		// Case 3: Target slot empty -> move
		if (targetStack == null)
		{
			target.SetSlot(targetIndex, payload.Item, amountToMove);
			source.ClearSlot(sourceIndex);
			return;
		}

		// Case 4: Swap
		Swap(source, sourceIndex, target, targetIndex);
	}

	public Inventory(int size)
	{
		slots = new ItemStack[size];
	}

	private void ClearSlot(int index)
	{
		slots[index] = null;
	}

	private void SetSlot(int index, IItem item, int amount)
	{
		slots[index] = new ItemStack(item, amount);
	}

	private static void Swap(
		Inventory a, int indexA,
		Inventory b, int indexB)
	{
		var temp = a.slots[indexA];
		a.slots[indexA] = b.slots[indexB];
		b.slots[indexB] = temp;
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

	public void RemoveAt(int index)
	{
		slots[index] = null;
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
