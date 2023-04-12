using Godot;
using System;

public partial class obj_logo : Sprite2D
{
	private float scale = 0f;
	private int index = 0;
	private bool moving = false;
	private Vector2[] positionsLeft;

	private Sprite2D spr_arrowLeft;
	private Sprite2D obj_transition;

	public float scaleMultiplier = 8f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
		spr_arrowLeft = GetNode<Sprite2D>("spr_arrowLeft");
		obj_transition = GetNode<Sprite2D>("../obj_transition");
		obj_transition.Visible = false;
		positionsLeft = new Vector2[] {
			new Vector2(-64, 38), new Vector2(-78, 68), new Vector2(-94, 98), new Vector2(-46, 120)
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Visible)
		{
			if(scale < 2)
			{
				scale += scaleMultiplier * (float)delta;
				if(scale > 2)
					scale = 2;
				Scale = new Vector2(scale, scale);
			}
			else
			{
				_HandleGUIInput();
				spr_arrowLeft.Position = positionsLeft[index]; //spr_arrowLeft.Position.Lerp(positionsLeft[index], (float)delta * 12);
				switch(index)
				{
					case 0:
						((cm_transition)obj_transition).nextRoom = "rm_setup";
						break;
					case 1:
						((cm_transition)obj_transition).nextRoom = "rm_setup";
						break;
					case 2:
						((cm_transition)obj_transition).nextRoom = "rm_setup_challenge";
						break;
					case 3:
						((cm_transition)obj_transition).nextRoom = "rm_quit";
						break;
				}
			}
		}
	}

	private void _HandleGUIInput()
	{
		if(Input.IsActionJustPressed("pause1"))
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
			index = 3;
		if(index > 3)
			index = 0;
	}
}
