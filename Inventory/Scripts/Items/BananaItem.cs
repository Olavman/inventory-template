using Godot;
using System;

public partial class BananaItem : Node, IItem
{
    string IItem.Name {get;set;} = "Banana";
    public int MaxStackSize { get; set; } = 20;
    public Texture2D Icon { get; set; } = GD.Load<Texture2D>("res://Inventory/Assets/banana_icon.png");
}
