using Godot;
using System;
using System.Collections.Generic;

public abstract class ItemBase
{
    public int ItemIndex;
    public int Cost;
    public Texture2D Texture;
    public Type t;

    public ItemBase(int _ItemIndex, int _Cost)
    {
        ItemIndex = _ItemIndex;
        Cost = _Cost;

        Texture = GD.Load<Texture2D>("res://sprites/items/spr_item" + _ItemIndex + ".png");
    }

    public ItemBase(ItemBase _Clone) : this(_Clone.ItemIndex, _Clone.Cost) {}

    public abstract void ItemUseMap(PlayerData player); //{ GD.Print("This item cannot be used here. This is an error..."); }

    //public void ItemUseMinigame(PlayerData player, int minigame) { GD.Print("This item cannot be used here. This is an error..."); }
}