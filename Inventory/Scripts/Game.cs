using Godot;
using System;

public partial class Game : Node
{
	InventoryController inventoryController;
	public override void _Ready()
	{
		inventoryController = GetNode<InventoryController>("InventoryController");

		inventoryController.PlayerInventory = GetNode<InventoryUI>("InventoryUI");
		inventoryController.OtherInventory = GetNode<InventoryUI>("InventoryUI2");
	}

}
