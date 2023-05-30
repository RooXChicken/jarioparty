using Godot;
using System;
using System.Collections.Generic;

public partial class rm_minigame_susville : Node2D
{
	public override void _Ready()
	{
		GetNode<obj_start>("obj_minigameBase/obj_start").onStart = new Callable(this, "StartMinigame");
		GetNode<obj_timerText>("obj_minigameBase/spr_timer/obj_text").onEnd = new Callable(this, "EndMinigame");

		GetNode<Node2D>("obj_minigameBase/Score").Visible = true;

		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/mus_minigame_susville.wav", "mus_minigame_susville");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/susville/snd_amongusKill.wav", "susville_amongusKill");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/susville/snd_imposterKill.wav", "susville_imposterKill");
	}

	public void StartMinigame()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("mus_minigame_susville");
		//GetNode<obj_mushmixup_frog>("obj_minigame_mushmixup_frog").StartMinigame();
	}

	public void EndMinigame()
	{
		List<PlayerData> places = new List<PlayerData>();
		for(int i = 0; i < 4; i++)
			places.Add(((GameManager)GetNode("/root/GameManager")).playerData[i]);

		for(int k = 0; k < 16; k++)
		{
			for(int i = 1; i < 4; i++)
			{
				if(places[i-1].minigameScore < places[i].minigameScore)
				{
					PlayerData p0 = places[i-1];
					PlayerData p1 = places[i];

					places[i-1] = p1;
					places[i] = p0;
				}
			}
		}

		places.Reverse();

		GetNode<obj_winner>("obj_minigameBase/Win").EndMiniGame(1, new int[] {places[0].characterIndex}, places, new int[] {10, 6, 4, 0});
	}
}
