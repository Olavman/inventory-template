using Godot;
using System;

public partial class Game : Node
{
	InventoryUI inventoryUI;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		inventoryUI = GetNode<InventoryUI>("InventoryUI");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		// GD.Print("A");
		if (Input.IsActionJustPressed("test"))
		{
			GD.PrintT("Pressed");
			inventoryUI.AddItem(new BananaItem(), 7);
		}
	}
}
