using Godot;
using System;

public partial class obj_splash : Node2D
{
	public void Splash()
	{
		GetNode<AnimatedSprite2D>("spr_splash").Visible = true;
		GetNode<AnimatedSprite2D>("spr_splash").Play();
		((AudioController)GetNode("/root/AudioController")).PlaySound("mushmixup_splash");
	}
}
