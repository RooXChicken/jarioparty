using Godot;
using System;
using System.Collections.Generic;

public partial class rm_minigame_mushmixup : Node2D
{
	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("res://sound/rooms/maps/mus_sushroom.wav");
	}
}
