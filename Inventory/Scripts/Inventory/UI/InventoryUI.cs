using Godot;
using System;

public partial class InventoryUI : Control
{
	[Export] private NodePath _gridPath;
	
	private GridContainer _grid;
	private Inventory _inventory;

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

			// Listen for drop events
			slot.Dropped += OnSlotDropped;
		}

		RefreshAllSlots();
    }

	public void AddItem(IItem item, int amount)
	{
		_inventory.AddItem(item, amount);
		GD.Print("Item added");
		RefreshAllSlots();
	}

    private void OnSlotDropped(DragPayload payload, int targetIndex)
    {
		// This is the ONLY place where inventory changes happen
		Inventory.Transfer(payload, _inventory, targetIndex);

		// Update all slot visuals
		RefreshAllSlots();
		GD.Print("Inventory refreshed");
    }

    private void RefreshAllSlots()
    {
		foreach (InventorySlot slot in _grid.GetChildren())
		{
			slot.Refresh();
		}
    }
}
