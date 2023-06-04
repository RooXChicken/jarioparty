using Godot;
using System;
using System.Collections.Generic;

public partial class obj_playerScore2v : Sprite2D
{
	private int score = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/snd_addScore.wav", "minigame_addScore");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void AddScore(int addition, PlayerData pdata)
	{
		score += addition;
		Frame = score;

		pdata.minigameScore = score;

		((AudioController)GetNode("/root/AudioController")).PlaySound("minigame_addScore");

		if(score >= 5)
		{
			GetNode<obj_timerText>("../../spr_timer/obj_text").onEnd.Call();
		}
	}
}
