using Godot;
using System;

public partial class obj_bg : Sprite2D
{
	private float alpha = 0f;
	private float xValue = 1080;
	public float alphaMultiplier = 320f;
	public float timeMult = 10f;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
		Position = new Vector2(Position.X - xValue, Position.Y);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Visible && Position.X > 640)
		{
			xValue = alphaMultiplier * (float)delta;
			xValue *= timeMult;
			timeMult *= (float)delta;
			timeMult += (float)delta;
			//GD.Print(xValue);
			if(Position.X < 640)
				Position = new Vector2(640, Position.Y);
			Modulate = new Color(1, 1, 1, alpha);
			
			
			Position = new Vector2(Position.X - xValue, Position.Y);
		}
	}
}
