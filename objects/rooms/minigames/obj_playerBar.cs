using Godot;
using System;

public partial class obj_playerBar : Sprite2D
{
	private int player = -1;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Variant _Player = GetMeta("Player");
		player = _Player.As<int>() - 1;

		Texture = GD.Load<Texture2D>("res://sprites/gui/minigames/spr_playerbar" + (((GameManager)GetNode("/root/GameManager")).playerData[player].characterIndex+1) + ".png");
	}
}
