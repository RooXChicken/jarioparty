using Godot;
using System;
using System.Collections.Generic;

public partial class PocketDimension : ItemBase
{
	public PocketDimension(int _ItemIndex, int _Cost) : base(_ItemIndex, _Cost) {}

	public override void ItemUseMap(PlayerData player)
	{
		int randomPlayer = player.playerOrder - 1;

		while(randomPlayer == player.playerOrder - 1)
			randomPlayer = ((GameManager)GetNode("/root/GameManager")).rand.Next(0, 3);

		PlayerData p2 = ((GameManager)GetNode("/root/GameManager")).playerData[randomPlayer];
		float progress1 = player.progress;
		float progress2 = p2.progress;
		int path1 = player.pathID;
		int path2 = p2.pathID;

		player.PowerupState = 4;
		p2.PowerupState = 4;

		player.progress = progress2;
		p2.progress = progress1;

		player.pathID = path2;
		p2.pathID = path1;
	}
}
