using Godot;
using System;

public partial class rm_minigame_susville : Node2D
{
	public override void _Ready()
	{
		GetNode<obj_start>("obj_minigameBase/obj_start").onStart = new Callable(this, "StartMinigame");

		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/mus_minigame_susville.wav", "mus_minigame_susville");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/susville/snd_amongusKill.wav", "susville_amongusKill");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/susville/snd_imposterKill.wav", "susville_imposterKill");
	}

	public void StartMinigame()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("mus_minigame_susville");
		//GetNode<obj_mushmixup_frog>("obj_minigame_mushmixup_frog").StartMinigame();
	}
}
