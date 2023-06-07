using Godot;
using System;

public partial class obj_mapGUI : CanvasLayer
{
	private Node[] wallets;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Initialize();

		wallets[0].GetNode<spr_bagPaper>("spr_bagPaperClip/spr_bagPaper").state = 1;

		if(((GameManager)GetNode("/root/GameManager")).TurnsMax + 1 == ((GameManager)GetNode("/root/GameManager")).TurnNumber)
			GetNode<Node2D>("TurnsLeft").Visible = false;

		GetNode<RichTextLabel>("TurnsLeft/obj_text").Text = "[center]" + ((GameManager)GetNode("/root/GameManager")).TurnNumber + "/" + ((GameManager)GetNode("/root/GameManager")).TurnsMax;
	}

	public void Initialize()
	{
		wallets = new Node[4];
		for(int i = 0; i < 4; i++)
		{
			wallets[i] = GetNode<Node>("obj_wallet" + (i + 1));
			wallets[i].GetNode<spr_bagPaper>("spr_bagPaperClip/spr_bagPaper").ChangeItems(((GameManager)GetNode("/root/GameManager")).playerData[i]);
			wallets[i].GetNode<Sprite2D>("spr_icon").Texture = GD.Load<CompressedTexture2D>("res://stage/gui/sprites/ids/spr_" + ((GameManager)GetNode("/root/GameManager")).playerData[i].characterName.ToLower() + "ID.png");
		}
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

	public void HideWallets()
	{
		for(int i = 1; i <= 4; i++)
			GetNode<Node2D>("obj_wallet" + i).Visible = false;
	}

	public void ShowWallet(int player)
	{
		GetNode<Node2D>("obj_wallet" + player).Visible = true;
		wallets[player-1].GetNode<spr_bagPaper>("spr_bagPaperClip/spr_bagPaper").state = 1;
	}

	public void ShowWallets()
	{
		for(int i = 1; i <= 4; i++)
			GetNode<Node2D>("obj_wallet" + i).Visible = true;
	}
}
