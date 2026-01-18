using Godot;
using System;

public partial class Game : Node
{
	[Export] private InventoryController _inventoryController;
	private InventoryUI _playerInventoryUI => GetNode<InventoryUI>("InventoryUI");
	private InventoryUI _otherInventoryUI => GetNode<InventoryUI>("InventoryUI2");
	public override void _Ready()
	{

		_inventoryController.PlayerInventory = GetNode<InventoryUI>("InventoryUI");
		_inventoryController.OtherInventory = GetNode<InventoryUI>("InventoryUI2");

		_playerInventoryUI.Initialize(_inventoryController);
		_otherInventoryUI.Initialize(_inventoryController);
	}

}
