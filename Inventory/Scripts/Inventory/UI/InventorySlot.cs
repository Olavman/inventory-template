using Godot;
using System;

public partial class InventorySlot : Control
{
	// Emitted when the user starts dragging from this slot
	[Signal] public delegate void DragStartedEventHandler(DragPayload payload);

	// Emitted when something is dropped onto this slot
	[Signal] public delegate void DroppedEventHandler(DragPayload payload, int targetIndex);

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
    private DragMode _nextDragMode;

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

	// Godot calls this automatically when user starts dragging from this Control
	public override Variant _GetDragData(Vector2 atPosition)
	{
		GD.Print("Want to drag from slot " + SlotIndex);
		
		var stack = Inventory.GetStackAt(SlotIndex);
		if (stack == null)
		{
			return default;
		}

		int amount = _nextDragMode switch
		{
			DragMode.Half => stack.Quantity / 2,
			DragMode.Single => 1,
			_ => stack.Quantity
		};

		_nextDragMode = DragMode.Full; // Reset for next time

		if (amount <= 0)
		{
			return default;
		}

		// Create a DragPayload
		// IMPORTANT: This is DATA, not the actual stack
		var payload = new DragPayload(
			Inventory,			// Which inventory it came from
			SlotIndex,			// Which slot it came from
			stack.Item,			// What item it is
			amount,				// How many items are being dragged
			DragMode.Full		// Drag mode
		);

		// Create drag preview
		var preview = new TextureRect();
		preview.Texture = stack.Item.Icon;
		preview.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;
		preview.CustomMinimumSize = stack.Item.Icon.GetSize();
		preview.Modulate = new Color(1, 1, 1, 0.85f);

		SetDragPreview(preview);

		// Notify UI owner
		EmitSignal(nameof(DragStarted), payload);

		// Godot will pass this payload to _DropData
		return Variant.From(payload);
	}

    public override void _GuiInput(InputEvent e)
    {
		if (e is InputEventMouseButton mb && mb.Pressed)
		{
			// Handle quick move requests
			if (mb.ButtonIndex == MouseButton.Left && Input.IsKeyPressed(Key.Shift))
			{
				EmitSignal(nameof(QuickMoveRequest), SlotIndex);
				AcceptEvent();
			}
			// Half stack drag
			else if (mb.ButtonIndex == MouseButton.Right)
			{
				_nextDragMode = DragMode.Half;
				AcceptEvent();
			}
			// Single item drag
			else if (mb.ButtonIndex == MouseButton.Left && Input.IsKeyPressed(Key.Ctrl))
			{
				_nextDragMode =  DragMode.Single;
				AcceptEvent();
			}
			// Full stack drag
			else
			{
				_nextDragMode = DragMode.Full;
			}
		}
    }

    

	// Called continously while dragging over this Control
    public override bool _CanDropData(Vector2 atPosition, Variant data)
    {
		GD.Print("Checking if can drop on slot " + SlotIndex);
		// Only accept if data is a DragPayload
		return data.VariantType == Variant.Type.Object &&
			data.As<DragPayload>() != null;
    }

	// Called when the mouse is released over this Control
	public override void _DropData(Vector2 atPosition, Variant data)
	{
		GD.Print("Dropped on slot " + SlotIndex);
		var payload = data.As<DragPayload>();

		// Tell InventoryUI that something was dropped here
		EmitSignal(nameof(Dropped), payload, SlotIndex);
	}
}
