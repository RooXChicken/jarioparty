using Godot;
using System;

public partial class rm_minigame_lookaway : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<obj_start>("obj_minigameBase/obj_start").onStart = new Callable(this, "StartMinigame");
		GetNode<obj_timerText>("obj_minigameBase/spr_timer/obj_text").onEnd = new Callable(this, "EndMiniGame");

		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/mus_minigame_lookaway.wav", "mus_minigame_lookaway");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartMinigame()
	{

	}
}
