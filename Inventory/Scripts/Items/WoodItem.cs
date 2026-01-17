using Godot;
using System;

public partial class WoodItem : Node, IItem
{
    string IItem.Name {get;set;} = "Wood";
    public int MaxStackSize { get; set; } = 5;
    public Texture2D Icon { get; set; } = GD.Load<Texture2D>("res://Inventory/Assets/wood_icon.png");
}
