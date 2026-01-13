using Godot;
using System;

public interface IItem
{
	string Name { get; set; }
	int MaxStackSize { get; set;}
    Texture2D Icon { get; set; }
}
