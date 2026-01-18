#nullable enable
using Godot;
using System;

public partial class InventoryController : Node
{
	[Export] private PackedScene _cursorInventoryUIScene = null!;
	private InventoryUI? _playerInventory;
	private InventoryUI? _otherInventory;

	public InventoryUI? PlayerInventory {
		get{
			return _playerInventory;
		}
		set
		{
			_playerInventory = value;
			if (_playerInventory != null)
				_playerInventory.QuickMoveRequest += OnPlayerQuickMoveRequest;

		}
	}
	public InventoryUI? OtherInventory {
		get{
			return _otherInventory;
		}
		set
		{
			_otherInventory = value;
			if (_otherInventory != null)
				_otherInventory.QuickMoveRequest += OnOtherQuickMoveRequest;

		}
	}
	private CursorInventoryUI _cursorUI = null!;
	public Inventory CursorInventory { get; private set; } = new Inventory(1);
	
	private TextureRect _cursorItemIcon = null!;
	private Label _cursorItemQuantity = null!;

	public override void _Ready()
	{
		_cursorUI = _cursorInventoryUIScene.Instantiate<CursorInventoryUI>();
		AddChild(_cursorUI);

		// Wire inventory change event to refresh UI
		CursorInventory.Changed += OnInventoryChanged;
	}

	private void OnInventoryChanged()
	{
		var stack = CursorInventory.GetStackAt(0);

		_cursorUI?.SetItem(
			stack?.Item,
			stack?.Quantity ?? 0
		);
		return;
	}

	public override void _Process(double delta)
	{
		// GD.Print("A");
		if (Input.IsActionJustPressed("test"))
		{
			PlayerInventory?.AddItem(new BananaItem(), 7);
		}
		if (Input.IsActionJustPressed("test2"))
		{
			PlayerInventory?.AddItem(new WoodItem(), 2);
		}
	}

	private void OnOtherQuickMoveRequest(int slotIndex)
	{
		if (PlayerInventory == null || OtherInventory == null)
			return;

		QuickMove(OtherInventory._inventory, PlayerInventory._inventory, slotIndex
		);
	}

	private void OnPlayerQuickMoveRequest(int slotIndex)
	{
		if (PlayerInventory == null || OtherInventory == null)
		return;

		QuickMove(PlayerInventory._inventory, OtherInventory._inventory, slotIndex
		);
	}

	private void QuickMove(Inventory source, Inventory target, int slotIndex)
	{
		GD.Print("Quick moving item from slot " + slotIndex);
		var stack = source.GetStackAt(slotIndex);
		if (stack == null)
		{
			GD.Print("No stack");
			return;
		}

		int leftover = target.AddItem(stack.Item, stack.Quantity);
		int moved = stack.Quantity - leftover;

		stack.Quantity -= moved;
		if (stack.Quantity <= 0)
		{
			source.ClearSlot(slotIndex);
		}

		GD.Print("Moved " + moved + " items");
		source.NotifyChanged();
		target.NotifyChanged();
	}

}
