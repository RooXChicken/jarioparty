using Godot;
using System;

public partial class obj_button : Node2D
{
	private int button;
	private AnimatedSprite2D spr_button;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Variant _Button = GetMeta("Button");
		button = _Button.As<int>();

		spr_button = GetNode<AnimatedSprite2D>("spr_button");
		spr_button.SpriteFrames = GD.Load<SpriteFrames>("res://sprites/gui/controls/anim_buttons" + button + ".tres");
		spr_button.Play();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
