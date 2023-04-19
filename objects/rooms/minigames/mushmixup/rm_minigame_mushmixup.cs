using Godot;
using System;
using System.Collections.Generic;

public partial class rm_minigame_mushmixup : Node2D
{
	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/mus_minigame_savingcourage.wav", "mus_minigame_savingcourage");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/mushmixup/snd_splash.wav", "mushmixup_splash");
	}
}
