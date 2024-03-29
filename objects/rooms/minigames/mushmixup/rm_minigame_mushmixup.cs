using Godot;
using System;
using System.Collections.Generic;

public partial class rm_minigame_mushmixup : Node2D
{
	public override void _Ready()
	{
		GetNode<obj_start>("obj_minigameBase/obj_start").onStart = new Callable(this, "StartMinigame");

		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/mus_minigame_savingcourage.wav", "mus_minigame_savingcourage");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/mushmixup/snd_splash.wav", "mushmixup_splash");
	}

	public void StartMinigame()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("mus_minigame_savingcourage");
		GetNode<obj_mushmixup_frog>("obj_minigame_mushmixup_frog").StartMinigame();
	}
}
