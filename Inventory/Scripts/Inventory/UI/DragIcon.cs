using Godot;
using System;

public partial class DragIcon : TextureRect
{
	
	public override void _Process(double delta)
	{
		// Follow the mouse
		GlobalPosition = GetGlobalMousePosition();
	}
}
