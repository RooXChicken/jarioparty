using Godot;
using System;
using System.Collections.Generic;

public partial class DoubleDice : ItemBase
{
    public DoubleDice(int _ItemIndex, int _Cost) : base(_ItemIndex, _Cost) {}

    public override void ItemUseMap(PlayerData player)
    {
        player.PowerupState = 2;
    }
}