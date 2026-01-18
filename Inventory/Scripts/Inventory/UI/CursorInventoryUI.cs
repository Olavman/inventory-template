using Godot;
using System;

public partial class CursorInventoryUI : Control
{
	[Export] private InventoryUI _inventoryUI;
	public override void _Process(double delta)
	{
		GlobalPosition = GetViewport().GetMousePosition()-new Vector2(16,16);
	}
}
