using Godot;
using System;

public partial class obj_clock : Node2D
{
	private Sprite2D spr_clockArrow;

	private float joyhaxis;
	private float joyvaxis;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spr_clockArrow = GetNode<Sprite2D>("spr_clock/spr_clockArrow");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetControllerInput();

		GD.Print(spr_clockArrow.Rotation);

		if(joyhaxis > 0 && spr_clockArrow.Rotation < 6.15f)
			spr_clockArrow.Rotation += joyhaxis;
		if(joyhaxis < 0 && spr_clockArrow.Rotation > 0f)
			spr_clockArrow.Rotation += joyhaxis;
	}

	private void GetControllerInput()
	{
		joyhaxis = Input.GetAxis("left" + 1, "right" + 1)*0.2f;
		joyvaxis = Input.GetAxis("up" + 1, "down" + 1);
	}
}
