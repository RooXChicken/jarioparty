using Godot;
using System;
using System.Collections.Generic;

public partial class rm_minigame_balleyball : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<obj_start>("obj_minigameBase/obj_start").onStart = new Callable(this, "StartMinigame");
		GetNode<obj_timerText>("obj_minigameBase/spr_timer/obj_text").onEnd = new Callable(this, "EndMinigame");
		GetNode<obj_timerText>("obj_minigameBase/spr_timer/obj_text").Start();

		GetNode<Node2D>("obj_minigameBase/Score2v2").Visible = true;

		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/mus_minigame_balleyball.wav", "mus_minigame_balleyball");
	}

	public void StartMinigame()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("mus_minigame_balleyball");
		GetNode<obj_ball>("obj_ball").Unfreeze();
	}

	public void EndMinigame()
	{
		GetNode<obj_ball>("obj_ball").Freeze = true;
		List<PlayerData> places = new List<PlayerData>();

		if(((GameManager)GetNode("/root/GameManager")).playerData[0].minigameScore > ((GameManager)GetNode("/root/GameManager")).playerData[3].minigameScore)
		{
			places.Add(((GameManager)GetNode("/root/GameManager")).playerData[0]);
			places.Add(((GameManager)GetNode("/root/GameManager")).playerData[1]);
			places.Add(((GameManager)GetNode("/root/GameManager")).playerData[2]);
			places.Add(((GameManager)GetNode("/root/GameManager")).playerData[3]);
		}
		else
		{
			places.Add(((GameManager)GetNode("/root/GameManager")).playerData[2]);
			places.Add(((GameManager)GetNode("/root/GameManager")).playerData[3]);
			places.Add(((GameManager)GetNode("/root/GameManager")).playerData[0]);
			places.Add(((GameManager)GetNode("/root/GameManager")).playerData[1]);
		}

		places.Reverse();

		GetNode<obj_winner>("obj_minigameBase/Win").EndMiniGame(2, new int[] {places[2].characterIndex, places[3].characterIndex}, places, new int[] {6, 6, 0, 0}, 2);

		for(int i = 0; i < 4; i++)
			((GameManager)GetNode("/root/GameManager")).playerData[i].minigameScore = 0;
	}
}
