using Godot;
using System;

public partial class cm_bg : Sprite2D
{
	public float scrollSpeed = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(HasMeta("ScrollSpeed"))
		{
			Variant _mult = GetMeta("ScrollSpeed");
			scrollSpeed = _mult.As<float>();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(HasMeta("ScrollSpeed"))
		{
			Variant _mult = GetMeta("ScrollSpeed");
			scrollSpeed = _mult.As<float>();
		}

		Position = new Vector2(Position.X, Position.Y - (scrollSpeed * (float)delta));

		if(Position.Y <= 0)
			Position = new Vector2(Position.X, 720);
	}
}
