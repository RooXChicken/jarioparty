using Godot;
using System;

public partial class obj_logo : Sprite2D
{
	private float scale = 0f;
	public float scaleMultiplier = 8f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Visible && scale < 2)
		{
			scale += scaleMultiplier * (float)delta;
			if(scale > 2)
				scale = 2;
			Scale = new Vector2(scale, scale);
		}
	}
}
