using Godot;
using System;

public partial class DragPayload : RefCounted // IMPORTANT: Must be a RefCounted object to be passed via Variant
{
	// Inventory where the drag started
	public readonly Inventory Source;

	// Slot index in the source inventory
	public readonly int SourceIndex;

	// Which item type is being dragged
	public readonly IItem Item;

	// How many units are being dragged
	public readonly int Amount;

	public DragPayload(Inventory source, int sourceIndex, IItem item, int amount)
	{
		Source = source;
		SourceIndex = sourceIndex;
		Item = item;
		Amount = amount;
	}
	
}
