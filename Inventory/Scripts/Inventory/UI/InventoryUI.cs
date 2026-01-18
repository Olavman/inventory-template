using Godot;
using System;

public partial class InventoryUI : Control
{
	[Export] private NodePath _gridPath;

	[Signal] public delegate void QuickMoveRequestEventHandler(int slotIndex);
	
	private GridContainer _grid;
	internal Inventory _inventory;
	private Inventory _otherInventory; 
	public InventoryController InventoryController {get; set;} = null!;

    public override void _Ready()
    {
		// Get GridContainer node
		_grid = GetNode<GridContainer>(_gridPath);

		// Create the actual inventory model
		int slotCount = _grid.GetChildCount();
		_inventory = new Inventory(slotCount);

		// Wire inventory change event to refresh UI
		_inventory.Changed += OnInventoryChanged;

    }

	public void Initialize(InventoryController controller)
	{
		InventoryController = controller;
		
		// Wire each UI slot to an inventory index
		for (int i = 0; i < _grid.GetChildCount(); i++)
		{
			var slot = _grid.GetChild<InventorySlot>(i);
			slot.SlotIndex = i;
			slot.Inventory = _inventory;
			slot.InventoryController = controller;

			// Listen for quick move requests
			slot.QuickMoveRequest += OnQuickMove;
		}
		
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

    private void RefreshAllSlots()
    {
		foreach (InventorySlot slot in _grid.GetChildren())
		{
			slot.Refresh();
		}
    }
}
