using Godot;
using System;

public partial class rm_credits : Node2D
{

	private Node2D obj_scroll;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		obj_scroll = GetNode<Node2D>("Scroll");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/rm_credits/mus_credits.wav", "mus_credits");
		((AudioController)GetNode("/root/AudioController")).PlaySound("mus_credits");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		obj_scroll.Position = new Vector2(0, obj_scroll.Position.Y - (50 * (float)delta));
	}
}
