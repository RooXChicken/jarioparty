using Godot;
using System;
using System.Collections.Generic;

public partial class rm_minigame_tugofjar : Node2D
{
	private obj_player_tugofjar[] players;
	private Node2D obj_rope;

	private Vector2 ropePos;
	private int ropeProgress = 0;
	private int oldProgress = 0;
	private bool minigameEnded = false;

	private Alarm t_anim;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<obj_start>("obj_minigameBase/obj_start").onStart = new Callable(this, "StartMinigame");
		GetNode<obj_timerText>("obj_minigameBase/spr_timer/obj_text").onEnd = new Callable(this, "EndMinigame");

		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/mus_minigame_tugojar.wav", "mus_minigame_tugojar");

		obj_rope = GetNode<Node2D>("obj_rope");
		ropePos = obj_rope.Position;

		players = new obj_player_tugofjar[4];
		for(int i = 1; i <= 4; i++)
			players[i-1] = GetNode<obj_player_tugofjar>("obj_rope/Players/obj_player_tugofjar"+i);

		players[0].strength = -4;

		GetNode<obj_timerText>("obj_minigameBase/spr_timer/obj_text").Start();

		t_anim = new Alarm(0.4, false, this, new Callable(this, "Animate"), false);
	}

	public override void _Process(double delta)
	{
		obj_rope.Position = new Vector2(ropePos.X + ropeProgress, ropePos.Y);
		if(minigameEnded)
			return;
		if(obj_rope.Position.X < 510)
		{
			EndMinigameLeft();
		}
		if(obj_rope.Position.X > 820)
		{
			EndMinigameRight();
		}
	}

	public void Pull(int amount)
	{
		ropeProgress += amount;
	}

	public void StartMinigame()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("mus_minigame_tugojar");
		t_anim.Start();
	}

	public void EndMinigame()
	{
		minigameEnded = true;
		GetNode<obj_winner>("obj_minigameBase/Win").EndMiniGame(0, new int[] {}, new List<PlayerData>(), new int[] {0, 0, 0, 0});
	}

	public void EndMinigameRight()
	{
		minigameEnded = true;
		List<PlayerData> places = new List<PlayerData>();
		for(int i = 1; i < 4; i++)
			places.Add(players[i].playerData);
		
		places.Add(players[0].playerData);

		places.Reverse();
		GetNode<obj_winner>("obj_minigameBase/Win").EndMiniGame(3, new int[] {players[1].CharacterIndex, players[2].CharacterIndex, players[3].CharacterIndex}, places, new int[] {4, 4, 4, 0}, 3);
	}

	public void EndMinigameLeft()
	{
		minigameEnded = true;
		List<PlayerData> places = new List<PlayerData>();
		for(int i = 0; i < 4; i++)
			places.Add(players[i].playerData);

		places.Reverse();
		GetNode<obj_winner>("obj_minigameBase/Win").EndMiniGame(1, new int[] {players[0].CharacterIndex}, places, new int[] {12, 0, 0, 0}, 1);
	}

	public void Animate()
	{
		if(oldProgress > ropeProgress)
		{
			players[0].Losing();
			for(int i = 1; i < 4; i++)
				players[i].Winning();
		}
		else if(oldProgress < ropeProgress)
		{
			players[0].Winning();
			for(int i = 1; i < 4; i++)
				players[i].Losing();
		}
		else
		{
			for(int i = 0; i < 4; i++)
				players[i].Losing();
		}

		oldProgress = ropeProgress;
	}
}
