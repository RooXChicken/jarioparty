using Godot;
using System;
using System.Collections.Generic;

public partial class rm_minigame_lookaway : Node2D
{
	private obj_character_lookaway[] players;
	private AnimatedSprite2D spr_bg;
	private List<PlayerData> places = new List<PlayerData>();

	private int turnsLeft = 5;
	private int killedPlayers = 0;
	private float[] scoreDisplay = new float[] {0, 0.176f, 0.39f, 0.615f, 0.83f, 1};

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spr_bg = GetNode<AnimatedSprite2D>("spr_bg");
		GetNode<obj_start>("obj_minigameBase/obj_start").onStart = new Callable(this, "StartMinigame");
		GetNode<obj_timerText>("obj_minigameBase/spr_timer/obj_text").onEnd = new Callable(this, "EndMiniGame");

		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/mus_minigame_lookaway.wav", "mus_minigame_lookaway");

		players = new obj_character_lookaway[4];
		for(int i = 1; i <= 4; i++)
		{
			players[i - 1] = (obj_character_lookaway)GetNode<obj_character_lookaway>("Players/obj_character_lookaway" + i);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void UpdateGameState()
	{
		if(spr_bg.Frame == 11)
		{
			//((GameManager)GetNode("/root/GameManager")).MinigameStarted = false;
			for(int i = 0; i < 4; i++)
			{
				players[i].Locked = true;
				if(i != 0 && !players[i].Out)
				{
					if(players[i].DirectionFacing == players[0].DirectionFacing)
					{
						players[i].Shrink();
						players[i].Lost = true;
						killedPlayers++;
						places.Insert(0, players[i].playerData);
					}
				}
			}
		}
		else if(spr_bg.Frame == 12)
		{
			Progress();
		}
	}

	private void Progress()
	{
		if(killedPlayers >= 3 && turnsLeft == 1)
			EndMiniGameTie();
		if(killedPlayers >= 3)
			EndMiniGameWin();
		if(turnsLeft == 1)
			EndMiniGameLoss();
		((ShaderMaterial)GetNode<Sprite2D>("spr_bg/spr_score").Material).SetShaderParameter("remaining", scoreDisplay[--turnsLeft]);
		
		foreach(obj_character_lookaway p in players)
		{
			p.Locked = false;
		}
		StartMinigame();
	}

	private void EndMiniGameWin()
	{
		places.Insert(0, players[0].playerData);
		GetNode<obj_winner>("../obj_minigameBase/Win").EndMiniGame(0, new int[] {players[0].CharacterIndex}, places, new int[] {10, 6, 4, 0});
	}

	private void EndMiniGameLoss()
	{
		places.Add(players[0].playerData);
		int[] playerPlaces = new int[3];
		for(int i = 0; i < 3; i++)
			playerPlaces[i] = places[i].characterIndex;
		GetNode<obj_winner>("../obj_minigameBase/Win").EndMiniGame(0, playerPlaces, places, new int[] {10, 6, 4, 0});
	}

	private void EndMiniGameTie()
	{
		places.Add(players[0].playerData);
		GetNode<obj_winner>("../obj_minigameBase/Win").EndMiniGame(0, new int[] {}, places, new int[] {2, 2, 2, 2});
	}

	public void StartMinigame()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("mus_minigame_lookaway");
		spr_bg.Frame = 0;
		foreach(obj_character_lookaway p in players)
		{
			p.StartAnimation();
		}
	}
}
