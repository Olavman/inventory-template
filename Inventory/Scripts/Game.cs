using Godot;
using System;

public partial class Game : Node
{
	private InventoryController _inventoryController => GetNode<InventoryController>("/root/Game/InventoryController");
	public override void _Ready()
	{

		_inventoryController.PlayerInventory = GetNode<InventoryUI>("InventoryUI");
		_inventoryController.OtherInventory = GetNode<InventoryUI>("InventoryUI2");
	}

}
