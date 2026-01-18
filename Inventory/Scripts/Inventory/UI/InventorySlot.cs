using Godot;
using System;

public partial class InventorySlot : Control
{
	// Reference to the global inventory controller
	public InventoryController InventoryController {get; set; } = null!;

	// Emitted when the user requests a quick move of an item
	[Signal] public delegate void QuickMoveRequestEventHandler(int slotIndex);

	// Which index this slot represents in the inventory
	public int SlotIndex {get; set;}
	public bool IsLocked {get; set;} = false;

	// Reference to the inventory this slot belongs to
	public Inventory Inventory {get; set;}

	private TextureRect _icon;
	private Label _quantity;
	private Panel _panel;

	private Color _normalColor = new Color(1,1,1,1);
	private Color _highlightColor = new Color(1.6f,1.6f,1.6f,1);

    public override void _Ready()
	{
		_icon = GetNode<TextureRect>("Panel/Icon");
		_quantity = GetNode<Label>("Quantity");
		_panel = GetNode<Panel>("Panel");

		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
    	GD.Print($"Slot {SlotIndex} size: {Size}");
	}

    private void OnMouseEntered()
    {
        _panel.Modulate = _highlightColor;
		GD.Print("Mouse entered slot " + SlotIndex);
    }

    private void OnMouseExited()
    {
        _panel.Modulate = _normalColor;
		GD.Print("Mouse exited slot " + SlotIndex);
    }

    // Called whenever the inventory changes and the slot needs to update its visuals
    public void Refresh()
	{
		// GD.Print("Refreshing slot " + SlotIndex);
		var stack = Inventory.GetStackAt(SlotIndex);

		// Hide if slot is empty
		if (stack == null)
		{
			_icon.Visible = false;
			_quantity.Visible = false;
			return;
		}

		// Show icon and quantity if slot has an item
		_icon.Visible = true;
		_icon.Texture = stack.Item.Icon;

		_quantity.Visible = stack.Quantity > 1;
		_quantity.Text = stack.Quantity.ToString();
	}

    public override void _GuiInput(InputEvent e)
    {
		if (e is not InputEventMouseButton mb || !mb.Pressed)
		{
			return;
		}

		// Quick transfer to another inventory
		if (mb.ButtonIndex == MouseButton.Left && Input.IsKeyPressed(Key.Shift))
		{
			EmitSignal(nameof(QuickMoveRequest), SlotIndex);
			AcceptEvent();
			return;
		}

		var mode = 
			mb.ButtonIndex == MouseButton.Right ? TransferAmount.Half :
			Input.IsKeyPressed(Key.Ctrl) 		? TransferAmount.Single :
												  TransferAmount.Full;

		// Start dragging from this slot
		if (mb.ButtonIndex == MouseButton.Left || mb.ButtonIndex == MouseButton.Right)
		{
			if (InventoryController == null)
			{
				GD.PrintErr("InventoryController is null!");
				return;
			}
			if (Inventory == null)
			{
				GD.PrintErr("Inventory is null!");
				return;
			}
			if (InventoryController.CursorInventory == null)
			{
				GD.PrintErr("CursorInventory is null!");
				return;
			}
			Inventory.Transfer(
				sourceInventory: InventoryController.CursorInventory,
				sourceIndex: 0,
				targetInventory: Inventory,
				targetIndex: SlotIndex,
				amountMode: mode
			);
		}

		Inventory.NotifyChanged();
		InventoryController.CursorInventory.NotifyChanged();
		AcceptEvent();
    }
}
