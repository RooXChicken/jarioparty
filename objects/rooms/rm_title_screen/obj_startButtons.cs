using Godot;
using System;

public partial class obj_startButtons : Node2D
{
	public Sprite2D obj_pressStart;
	private Sprite2D obj_partyMode;
	private Sprite2D obj_challengeMode;
	private Sprite2D obj_quit;
	private Sprite2D obj_transition;
	
	private int index = 0;
	private bool moving = false;
	private bool enabled = false;
	public override void _Ready()
	{
		obj_pressStart = GetNode<Sprite2D>("obj_pressStart");
		obj_pressStart.Visible = false;

		obj_partyMode = GetNode<Sprite2D>("obj_partyMode");
		obj_partyMode.Visible = false;

		obj_challengeMode = GetNode<Sprite2D>("obj_challengeMode");
		obj_challengeMode.Visible = false;

		obj_quit = GetNode<Sprite2D>("obj_quit");
		obj_quit.Visible = false;

		obj_transition = GetNode<Sprite2D>("../obj_transition");
		obj_transition.Visible = false;
	}

	public override void _Process(double delta)
	{
		if(!Visible)
			return;

		if(obj_pressStart.Visible && Input.IsActionJustPressed("pause1"))
		{
			obj_pressStart.Visible = false;

			obj_partyMode.Visible = true;
			obj_challengeMode.Visible = true;
			obj_quit.Visible = true;
		}

		if(!obj_pressStart.Visible)
		{
			_HandleGUIInput();

			obj_partyMode.Scale = new Vector2(2, 2);
			obj_challengeMode.Scale = new Vector2(2, 2);
			obj_quit.Scale = new Vector2(2, 2);

			switch(index)
			{
				case 0:
					obj_partyMode.Scale = new Vector2(2.5f, 2.5f);
					((cm_transition)obj_transition).nextRoom = "rm_setup";
					break;
				case 1:
					obj_challengeMode.Scale = new Vector2(2.5f, 2.5f);
					((cm_transition)obj_transition).nextRoom = "rm_setup_challenge";
					break;
				case 2:
					obj_quit.Scale = new Vector2(2.5f, 2.5f);
					((cm_transition)obj_transition).nextRoom = "rm_quit";
					break;
			}
		}
	}

	private void _HandleGUIInput()
	{
		if(Input.IsActionJustReleased("pause1"))
			enabled = true;
		if(enabled && Input.IsActionJustPressed("pause1"))
			obj_transition.Visible = true;
		float joyvaxis = Input.GetAxis("down1", "up1");
		if(moving)
		{
			if(joyvaxis == 0)
				moving = false;

			return;
		}
		else if(joyvaxis != 0)
			moving = true;
		else
			return;

		if(joyvaxis > 0)
			index--;
		if(joyvaxis < 0)
			index++;

		if(index < 0)
			index = 2;
		if(index > 2)
			index = 0;
	}
}
