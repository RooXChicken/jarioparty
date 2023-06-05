using Godot;
using System;
using System.Collections.Generic;

public partial class obj_fire : AnimatedSprite2D
{
	public float speed = 1;
	private short deadPlayers = 0;
	private AnimatedSprite2D spr_bg;
	private obj_character_parent[] players;
	private List<PlayerData> places = new List<PlayerData>();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).MusicEffect("pitch", 0.93f + (speed * 0.06f));
		spr_bg = GetNode<AnimatedSprite2D>("../spr_bg");

		players = new obj_character_parent[4];
		for(int i = 0; i < 4; i++)
		{
			players[i] = (obj_character_parent)GetNode<RigidBody2D>("../Players/obj_character_parent" + (i + 1));
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void FrameChanged()
	{
		if(Frame == 9)
		{
			speed += 0.03f;

			SpeedScale = speed;
			spr_bg.SpeedScale = speed;
			((AudioController)GetNode("/root/AudioController")).MusicEffect("pitch", 0.93f + (speed * 0.06f));
		}
		if(Frame >= 8 && Frame <= 10)
		{
			((ShaderMaterial)Material).SetShaderParameter("canDie", true);
			ZIndex = 1;

			for(int i = 0; i < 4; i++)
			{
				if(!players[i].Lost && players[i].Position.Y > 450)
				{
					players[i].Burn();
					places.Add(players[i].playerData);
					if(++deadPlayers >= 3)
					{
						EndMinigame();
					}
				}
			}
		}
		else if(Frame == 11)
			((ShaderMaterial)Material).SetShaderParameter("canDie", false);
		else if(Frame == 15)
		{
			ZIndex = 3;
		}
	}

	private void EndMinigame()
	{
		int playersAlive = 0;
		SpeedScale = 0;
		short ind = 0;
		int[] aliveList = new int[4-deadPlayers];

		for(int i = 0; i < 4; i++)
		{
			if(!players[i].Lost)
			{
				playersAlive++;
				aliveList[ind] = players[i].CharacterIndex;
				players[i].joyLock = true;
				places.Add(players[i].playerData);
				ind++;
			}
		}
		((AudioController)GetNode("/root/AudioController")).MusicEffect("pitch", 1);
		((AudioController)GetNode("/root/AudioController")).StopMusic();
		GetNode<obj_winner>("../obj_minigameBase/Win").EndMiniGame(playersAlive, aliveList, places, new int[] {12, 8, 4, 0});
	}

}
