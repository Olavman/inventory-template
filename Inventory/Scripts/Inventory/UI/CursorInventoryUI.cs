using Godot;
using System;

public partial class CursorInventoryUI : Control
{
	private TextureRect _icon;
	private Label _quantity;

    public override void _Ready()
    {
		_icon = GetNode<TextureRect>("Icon");
		_quantity = GetNode<Label>("Quantity");
    }
    
	public override void _Process(double delta)
	{
		GlobalPosition = GetViewport().GetMousePosition()-new Vector2(16,16);
	}

	public void SetItem(IItem item, int amount)
	{
		if (item == null)
		{
			_icon.Visible = false;
			_quantity.Visible = false;
			return;
		}

		_icon.Texture = item.Icon;
		_icon.Visible = true;

		if (amount > 1)
		{
			_quantity.Text = amount.ToString();
			_quantity.Visible = true;
		}
		else
		{
			_quantity.Visible = false;
		}
	}
}
