using Godot;
using System;

public partial class obj_mapGUI : CanvasLayer
{
	private Node[] wallets;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		wallets = new Node[4];
		for(int i = 0; i < 4; i++)
		{
			wallets[i] = GetNode<Node>("obj_wallet" + (i + 1));
			wallets[i].GetNode<Sprite2D>("spr_icon").Texture = GD.Load<CompressedTexture2D>("res://stage/gui/sprites/ids/spr_" + ((GameManager)GetNode("/root/GameManager")).playerData[i].characterName.ToLower() + "ID.png");
		}

		wallets[0].GetNode<spr_bagPaper>("spr_bagPaperClip/spr_bagPaper").state = 1;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		for(int i = 0; i < 4; i++)
		{
			int coins = ((GameManager)GetNode("/root/GameManager")).playerData[i].coins;
			int stars = ((GameManager)GetNode("/root/GameManager")).playerData[i].stars;
			bool moved = false;
			int index = 1;

			for(int k = 3; k > -1; k--)
			{
				int ans = ((int)Math.Floor(coins / (Math.Pow(10, k)))) % 10;
				if(!moved)
					if(ans != 0)
						moved = true;
					else
						wallets[i].GetNode<AnimatedSprite2D>("CoinNumbers/spr_num" + (k+1)).Frame = 10;
				
				if(moved)
				{
					wallets[i].GetNode<AnimatedSprite2D>("CoinNumbers/spr_num" + index).Frame = ans;
					index++;
				}
			}

			if(coins <= 0)
				wallets[i].GetNode<AnimatedSprite2D>("CoinNumbers/spr_num" + 1).Frame = 0;

			index = 1;
			moved = false;

			for(int k = 1; k > -1; k--)
			{
				int ans = ((int)Math.Floor(stars / (Math.Pow(10, k)))) % 10;
				if(!moved)
					if(ans != 0)
						moved = true;
					else
						wallets[i].GetNode<AnimatedSprite2D>("StarNumbers/spr_num" + (k+1)).Frame = 10;
				
				if(moved)
				{
					wallets[i].GetNode<AnimatedSprite2D>("StarNumbers/spr_num" + index).Frame = ans;
					index++;
				}
			}

			if(stars <= 0)
				wallets[i].GetNode<AnimatedSprite2D>("StarNumbers/spr_num" + 1).Frame = 0;
		}
	}

	public void SwitchPlayers(int _playerGoing)
	{
		for(int i = 0; i < 4; i++)
		{
			if(i == _playerGoing - 1)
				wallets[i].GetNode<spr_bagPaper>("spr_bagPaperClip/spr_bagPaper").state = 1;
			else
				wallets[i].GetNode<spr_bagPaper>("spr_bagPaperClip/spr_bagPaper").state = 0;
		}
	}
}
