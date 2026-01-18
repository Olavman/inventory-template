using Godot;
using System;

public partial class InventoryUI : Control
{
	[Export] private NodePath _gridPath;

	[Signal] public delegate void QuickMoveRequestEventHandler(int slotIndex);
	
	private GridContainer _grid;
	internal Inventory _inventory;
	private Inventory _otherInventory; 

    public override void _Ready()
    {
		// Get GridContainer node
		_grid = GetNode<GridContainer>(_gridPath);

		// Create the actual inventory model
		int slotCount = _grid.GetChildCount();
		_inventory = new Inventory(slotCount);

		// Wire each UI slot to an inventory index
		for (int i = 0; i < slotCount; i++)
		{
			var slot = _grid.GetChild<InventorySlot>(i);
			slot.SlotIndex = i;
			slot.Inventory = _inventory;

			// Listen for quick move requests
			slot.QuickMoveRequest += OnQuickMove;
		}

		// Wire inventory change event to refresh UI
		_inventory.Changed += OnInventoryChanged;

		RefreshAllSlots();
    }

	internal void OnQuickMove (int slotIndex)
	{
		GD.Print("Quick move requested for slot " + slotIndex);
		EmitSignal(nameof(QuickMoveRequest), slotIndex);
	}

    private void OnInventoryChanged()
    {
		RefreshAllSlots();
    }

    public void AddItem(IItem item, int amount)
	{
		_inventory.AddItem(item, amount);
		GD.Print("Item added");
		RefreshAllSlots();
	}

    // private void OnSlotDropped(DragPayload payload, int targetIndex)
    // {
	// 	// This is the ONLY place where inventory changes happen
	// 	Inventory.Transfer(payload, _inventory, targetIndex);

	// 	// Update all slot visuals
	// 	RefreshAllSlots();
	// 	GD.Print("Inventory refreshed");
    // }

    private void RefreshAllSlots()
    {
		foreach (InventorySlot slot in _grid.GetChildren())
		{
			slot.Refresh();
		}
    }

	// Accept a quick move request from another inventory
    internal void AcceptQuickMove(DragPayload payload)
    {
		_inventory.AddItem(payload.Item, payload.Amount);
		payload.Source.ClearSlot(payload.SourceIndex);

		RefreshAllSlots();
    }
}
