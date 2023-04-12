using Godot;
using System;

public partial class obj_bob : AnimatedSprite2D
{
	private float start = -5;
	private float end = 5;
	private float floatSpeed = 10f;
	private short state = 0;
	
	public override void _Ready()
	{
		Visible = false;
	}

	public override void _Process(double delta)
	{
		switch(state)
		{
			case 0:
				Position = new Vector2(Position.X, Position.Y + (floatSpeed * (float)delta));
				if(Position.Y >= end)
					state = 1;
				break;
			case 1:
				Position = new Vector2(Position.X, Position.Y - (floatSpeed * (float)delta));
				if(Position.Y <= start)
					state = 0;
				break;
		}
	}
}
