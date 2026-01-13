using Godot;
using System;

public partial class ItemStack
{
	public IItem Item {get;}
	public int Quantity {get; set;}

	public ItemStack(IItem item, int quantity)
	{
		Item = item;
		Quantity = quantity; //Initial quantity can be more than maxstacksize
	}

	public int Add (int amount)
	{
		int space = Item.MaxStackSize -Quantity;
		int added = Math.Min(space, amount);
		Quantity += added;
		return amount - added; // Remainder
	}
}
