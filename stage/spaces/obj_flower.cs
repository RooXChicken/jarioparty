using Godot;
using System;

public partial class obj_flower : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		AnimatedSprite2D spr_flower = GetNode<AnimatedSprite2D>("spr_flower");
		Variant _Flower = GetMeta("Flower");
		if(_Flower.As<int>() == 1)
		{
			spr_flower.Animation = "flower1";
			spr_flower.Frame = ((GameManager)GetNode("/root/GameManager")).rand.Next(0, 3);
		}
		else
			spr_flower.Animation = "rose";

		spr_flower.Play();
	}
}
