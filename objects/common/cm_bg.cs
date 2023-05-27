using Godot;
using System;

public partial class cm_bg : Sprite2D
{
	public float scrollSpeed = 0;
	private bool directionY = true;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(HasMeta("ScrollSpeed"))
		{
			Variant _mult = GetMeta("ScrollSpeed");
			scrollSpeed = _mult.As<float>();
		}

		if(HasMeta("DirectionIsY"))
		{
			Variant _dir = GetMeta("DirectionIsY");
			directionY = _dir.As<bool>();
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

		if(directionY)
		{
			Position = new Vector2(Position.X, Position.Y - (scrollSpeed * (float)delta));

			if(Position.Y <= 0)
				Position = new Vector2(Position.X, 720);
		}
		else
		{
			Position = new Vector2(Position.X + (scrollSpeed * (float)delta), Position.Y);

			if(Position.X >= 1280)
				Position = new Vector2(0, Position.Y);
		}
	}
}
