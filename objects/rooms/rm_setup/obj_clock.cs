using Godot;
using System;

public partial class obj_clock : Node2D
{
	public int Turns = 20;
	public bool Lock = false;
	private Sprite2D spr_clockArrow;
	private RichTextLabel obj_text;
	private RichTextLabel obj_text2;

	private string text;

	private float joyhaxis;
	private float joyvaxis;

	private float dpadhaxis;
	private float dpadvaxis;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/rm_setup/snd_tick.wav", "tick");
		spr_clockArrow = GetNode<Sprite2D>("spr_clock/spr_clockArrow");
		obj_text = GetNode<RichTextLabel>("obj_text");
		obj_text2 = GetNode<RichTextLabel>("obj_text2");

		spr_clockArrow.Rotation = 2.048f;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!Visible || Lock)
			return;
		
		float newDpadhaxis = Input.GetAxis("clockTickLeft", "clockTickRight");

		if(dpadhaxis != newDpadhaxis)
		{
			joyhaxis = newDpadhaxis*0.2f;
			GetControllerInput(true);
		}
		else
			GetControllerInput(false);

		if(joyhaxis > 0 && spr_clockArrow.Rotation < 6.22f)
		{
			//((AudioController)GetNode("/root/AudioController")).PlaySound("tick");
			spr_clockArrow.Rotation += joyhaxis*0.5f;
		}
		if(joyhaxis < 0 && spr_clockArrow.Rotation > 0.01f)
		{
			//((AudioController)GetNode("/root/AudioController")).PlaySound("tick");
			spr_clockArrow.Rotation += joyhaxis*0.5f;
		}

		if(spr_clockArrow.Rotation < 0.01f)
			spr_clockArrow.Rotation = 0.01f;
		if(spr_clockArrow.Rotation > 6.22f)
			spr_clockArrow.Rotation = 6.22f;

		if(joyhaxis != 0)
		{
			Turns = (int)Math.Ceiling(spr_clockArrow.Rotation * 9.646f);

			text = "[center]" + Turns;

			if(Turns == 1)
				text += " Turn";
			else
				text += " Turns";

			obj_text.Text = text;
			obj_text2.Text = text;

			joyhaxis = 0;
		}
	}

	private void GetControllerInput(bool ignoreJoy)
	{
		dpadhaxis = Input.GetAxis("clockTickLeft", "clockTickRight");
		dpadvaxis = Input.GetAxis("clockTickDown", "clockTickUp");

		if(ignoreJoy)
			return;

		joyhaxis = Input.GetAxis("left" + 1, "right" + 1)*0.2f;
		joyvaxis = Input.GetAxis("up" + 1, "down" + 1);

	}
}
