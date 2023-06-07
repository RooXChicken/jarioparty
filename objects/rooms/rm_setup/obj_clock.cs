using Godot;
using System;

public partial class obj_clock : Node2D
{
	public int Turns = 20;
	private Sprite2D spr_clockArrow;
	private RichTextLabel obj_text;
	private RichTextLabel obj_text2;

	private float joyhaxis;
	private float joyvaxis;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spr_clockArrow = GetNode<Sprite2D>("spr_clock/spr_clockArrow");
		obj_text = GetNode<RichTextLabel>("obj_text");
		obj_text2 = GetNode<RichTextLabel>("obj_text2");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!Visible)
			return;
		
		GetControllerInput();

		GD.Print(spr_clockArrow.Rotation);

		if(joyhaxis > 0 && spr_clockArrow.Rotation < 6.18f)
			spr_clockArrow.Rotation += joyhaxis;
		if(joyhaxis < 0 && spr_clockArrow.Rotation > 0f)
			spr_clockArrow.Rotation += joyhaxis;

		if(spr_clockArrow.Rotation < 0)
			spr_clockArrow.Rotation = 0;
		if(spr_clockArrow.Rotation > 6.18f)
			spr_clockArrow.Rotation = 6.18f;

		Turns = (int)Math.Ceiling(spr_clockArrow.Rotation * 9.708f);


	}

	private void GetControllerInput()
	{
		joyhaxis = Input.GetAxis("left" + 1, "right" + 1)*0.2f;
		joyvaxis = Input.GetAxis("up" + 1, "down" + 1);
	}
}
