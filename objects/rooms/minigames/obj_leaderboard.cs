using Godot;
using System;
using System.Collections.Generic;

public partial class obj_leaderboard : Node2D
{
	private bool countingCoins = false;
	private double delay = 0;
	private Node2D[] portraits;
	private List<PlayerData> playerData;
	private AnimationPlayer animation;
	private int[] places = new int[] {0, 1, 1, 2};
	private int[] coinVal;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animation = GetNode<AnimationPlayer>("anim_wingui");

		portraits = new Node2D[4];
		for(int i = 1; i < 5; i++)
		{
			portraits[i - 1] = GetNode<Node2D>("obj_portrait" + i);
		}
	}

	public override void _Process(double delta)
	{
		if(countingCoins && coinVal[0] + 1 > 0)
		{
			delay -= delta;
			if(delay <= 0)
			{
				delay = 0.08;
				Redraw();
				for(int i = 0; i < 4; i++)
				{
					if(coinVal[i] > 0)
					{
						playerData[3 - i].coins++;
						coinVal[i]--;
					}
				}

				
			}
		}
	}

	public void Start()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("minigameResults");
	}

	public void CountCoins()
	{
		countingCoins = true;
	}

	public void End()
	{
		((GameManager)GetNode("/root/GameManager")).CurrentMinigame = 3;
		((GameManager)GetNode("/root/GameManager")).SwitchScene("rm_map");
	}

	public void StartSequence(List<PlayerData> players, int[] coinValues, int mode)
	{
		coinVal = coinValues;
		playerData = players;
		for(int i = 0; i < 4; i++)
		{
			//GD.Print(players[i].characterName);
			portraits[i].GetNode<AnimatedSprite2D>("spr_portrait").SpriteFrames = playerData[3-i].animationFrames;
			if(mode == 0)
				portraits[i].GetNode<AnimatedSprite2D>("spr_portrait").Frame = places[i];
			else if(mode == 1)
				if(i == 0)
					portraits[i].GetNode<AnimatedSprite2D>("spr_portrait").Frame = 0;
				else
					portraits[i].GetNode<AnimatedSprite2D>("spr_portrait").Frame = 2;
			else if(mode == 3)
				if(i < 3)
					portraits[i].GetNode<AnimatedSprite2D>("spr_portrait").Frame = 0;
				else
					portraits[i].GetNode<AnimatedSprite2D>("spr_portrait").Frame = 2;
			else if(mode == 2)
				if(i < 2)
					portraits[i].GetNode<AnimatedSprite2D>("spr_portrait").Frame = 0;
				else
					portraits[i].GetNode<AnimatedSprite2D>("spr_portrait").Frame = 2;

			if(mode == 0)
				portraits[i].GetNode<Sprite2D>("spr_place").Frame = i;
			else if(mode == 1)
				if(i == 0)
					portraits[i].GetNode<Sprite2D>("spr_place").Frame = 0;
				else
					portraits[i].GetNode<Sprite2D>("spr_place").Frame = 1;
			else if(mode == 3)
				if(i < 3)
					portraits[i].GetNode<Sprite2D>("spr_place").Frame = 0;
				else
					portraits[i].GetNode<Sprite2D>("spr_place").Frame = 1;
			else if(mode == 2)
				if(i < 2)
					portraits[i].GetNode<Sprite2D>("spr_place").Frame = 0;
				else
					portraits[i].GetNode<Sprite2D>("spr_place").Frame = 1;

			Redraw();

		}
		animation.Play("play");
	}

	private void Redraw()
	{
		for(int i = 0; i < 4; i++)
		{
			int coins = playerData[i].coins;
			int stars = playerData[i].stars;
			int addition = coinVal[3-i];
			bool moved = false;
			int index = 1;

			for(int k = 3; k > -1; k--)
			{
				int ans = ((int)Math.Floor(coins / (Math.Pow(10, k)))) % 10;
				if(!moved)
					if(ans != 0)
						moved = true;
					else
						GetNode<AnimatedSprite2D>("obj_portrait" + (3-i+1) + "/CoinNumbers/spr_num" + (k+1)).Frame = 10;
				
				if(moved)
				{
					GetNode<AnimatedSprite2D>("obj_portrait" + (3-i+1) + "/CoinNumbers/spr_num" + index).Frame = ans;
					index++;
				}
			}

			if(coins <= 0)
				GetNode<AnimatedSprite2D>("obj_portrait" + (3-i+1) + "/CoinNumbers/spr_num1").Frame = 0;

			index = 1;
			moved = false;

			for(int k = 1; k > -1; k--)
			{
				int ans = ((int)Math.Floor(stars / (Math.Pow(10, k)))) % 10;
				if(!moved)
					if(ans != 0)
						moved = true;
					else
						GetNode<AnimatedSprite2D>("obj_portrait" + (3-i+1) + "/StarNumbers/spr_num" + (k+1)).Frame = 10;
				
				if(moved)
				{
					GetNode<AnimatedSprite2D>("obj_portrait" + (3-i+1) + "/StarNumbers/spr_num" + index).Frame = ans;
					index++;
				}
			}

			if(stars <= 0)
				GetNode<AnimatedSprite2D>("obj_portrait" + (3-i+1) + "/StarNumbers/spr_num" + index).Frame = 0;

			index = 1;
			moved = false;

			for(int k = 1; k > -1; k--)
			{
				int ans = ((int)Math.Floor(addition / (Math.Pow(10, k)))) % 10;
				if(!moved)
					if(ans != 0)
						moved = true;
					else
						GetNode<AnimatedSprite2D>("obj_portrait" + (3-i+1) + "/AdditionNumbers/spr_num" + (k+1)).Frame = 10;
				
				if(moved)
				{
					GetNode<AnimatedSprite2D>("obj_portrait" + (3-i+1) + "/AdditionNumbers/spr_num" + index).Frame = ans;
					index++;
				}
			}

			if(addition <= 0)
				GetNode<AnimatedSprite2D>("obj_portrait" + (3-i+1) + "/AdditionNumbers/spr_num" + index).Frame = 0;
		}
	}
}
