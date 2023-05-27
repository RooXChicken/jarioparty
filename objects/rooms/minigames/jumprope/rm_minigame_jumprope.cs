using Godot;
using System;

public partial class rm_minigame_jumprope : Node2D
{
	private AnimatedSprite2D spr_fire;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GetNode<obj_start>("obj_minigameBase/obj_start").onStart = new Callable(this, "StartMinigame");
		GetNode<obj_timerText>("obj_minigameBase/spr_timer/obj_text").onEnd = new Callable(this, "EndMiniGame");
		GetNode<obj_timerText>("obj_minigameBase/spr_timer/obj_text").Start();

		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/mus_minigame_jumprope.wav", "mus_minigame_jumprope");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/player/snd_burn.wav", "jumprope_burn");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void StartMinigame()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("mus_minigame_jumprope");
		GetNode<AnimatedSprite2D>("obj_fire").Play("default");
		GetNode<AnimatedSprite2D>("obj_fire").ZIndex = 3;
		// foreach(obj_character_parent p in players)
		// {
			
		// }
	}
}
