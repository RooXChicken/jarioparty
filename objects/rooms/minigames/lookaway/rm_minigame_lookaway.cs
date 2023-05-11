using Godot;
using System;

public partial class rm_minigame_lookaway : Node2D
{
	private obj_character_lookaway[] players;
	private AnimatedSprite2D spr_bg;

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
		if(spr_bg.Frame == 14)
		{
			((GameManager)GetNode("/root/GameManager")).MinigameStarted = false;
			for(int i = 0; i < 4; i++)
			{
				players[i].Locked = true;
				if(i != 0 && !players[i].Out)
				{
					if(players[i].DirectionFacing == players[0].DirectionFacing)
						players[i].Shrink();
				}
			}
		}
	}

	public void StartMinigame()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("mus_minigame_lookaway");
		foreach(obj_character_lookaway p in players)
		{
			p.StartAnimation();
		}
	}
}
