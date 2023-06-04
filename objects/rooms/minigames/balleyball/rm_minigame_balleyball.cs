using Godot;
using System;

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

	}
}
