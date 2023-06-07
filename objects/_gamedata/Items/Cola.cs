using Godot;
using System;
using System.Collections.Generic;

public partial class Cola : ItemBase
{
    public Cola(int _ItemIndex, int _Cost) : base(_ItemIndex, _Cost) {}

    // public new void ItemUseMinigame(PlayerData player, int minigame)
    // {
    //     switch(minigame)
    //     {
    //         case 1:
    //             player.speedMult = 1.2f;
    //             player.weightMult = 0.8f;
    //             player.strengthMult = 1.2f;
    //             break;
    //     }
        
    // }

    public override void ItemUseMap(PlayerData player)
    {
        player.PowerupState = 1;
        GD.Print("HIIIIIIII");
    }
}