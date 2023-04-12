using Godot;
using System;
using System.Collections.Generic;

public partial class ItemBase : Node
{
    public int ItemIndex;
    public int Cost;
    public Texture2D Texture;

    public ItemBase(int _ItemIndex, int _Cost)
    {
        ItemIndex = _ItemIndex;
        Cost = _Cost;

        Texture = GD.Load<Texture2D>("res://sprites/items/spr_item" + _ItemIndex + ".png");
    }

    public ItemBase(ItemBase _Clone) : this(_Clone.ItemIndex, _Clone.Cost) {}
}