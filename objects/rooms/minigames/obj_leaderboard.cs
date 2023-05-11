using Godot;
using System;
using System.Collections.Generic;

public partial class obj_leaderboard : Node2D
{
	private Node2D[] portraits;
	private int[] places = new int[] {0, 1, 1, 2};
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		portraits = new Node2D[4];
		for(int i = 1; i < 5; i++)
		{
			portraits[i - 1] = GetNode<Node2D>("obj_portrait" + i);
		}
	}

	public void Start()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("")
	}

	public void Organize(List<PlayerData> players)
	{
		for(int i = 0; i < 4; i++)
		{
			//GD.Print(players[i].characterName);
			portraits[i].GetNode<AnimatedSprite2D>("spr_portrait").SpriteFrames = players[3 - i].animationFrames;
			portraits[i].GetNode<AnimatedSprite2D>("spr_portrait").Frame = places[i];
			portraits[i].GetNode<Sprite2D>("spr_place").Frame = i;
		}
	}
}
