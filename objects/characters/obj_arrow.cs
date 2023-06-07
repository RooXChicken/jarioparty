using Godot;
using System;

public partial class obj_arrow : Node2D
{
	public int controllerIndex = 0;

	private int type = -1;
	private int direction = 2;

	private Sprite2D spr_arrow1;
	private Sprite2D spr_arrow2;

	private float joyhaxis = 0;
	private float joyvaxis = 0;

	private double delay = 0.3;
	private bool chosen = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Variant _Direction = GetMeta("Direction");
		type = _Direction.As<int>();

		direction -= type;

		spr_arrow1 = GetNode<Sprite2D>("spr_arrow1");
		spr_arrow2 = GetNode<Sprite2D>("spr_arrow2");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!Visible)
			return;

		GetControllerInput();

		delay -= delta;

		switch(type)
		{
			case 0:
				if(joyhaxis > joyvaxis)
					direction = 2;
				else if(joyvaxis > joyhaxis)
					direction = 1;
				break;
			case 1:
				if(-joyhaxis > joyvaxis)
					direction = 2;
				else if(joyvaxis > -joyhaxis)
					direction = 1;
				break;
		}

		if(direction == 1)
		{
			spr_arrow1.Scale = new Vector2(2.5f, 2.5f);
			if(type == 0)
				spr_arrow2.Scale = new Vector2(-2, 2);
			else
				spr_arrow2.Scale = new Vector2(2, 2);
		}
		else
		{
			spr_arrow1.Scale = new Vector2(2, 2);
			if(type == 0)
				spr_arrow2.Scale = new Vector2(-2.5f, 2.5f);
			else
				spr_arrow2.Scale = new Vector2(2.5f, 2.5f);
		}
	}

	public void ConfirmChoice(int _pathID = -1, float _progress = -1)
	{
		if(_pathID != -1)
		{
			CallDeferred("ChangeNodeDef", _pathID, _progress);
			return;
		}

		if(type == 0 && direction == 1)
			CallDeferred("ChangeNodeDef", 2, 0);
		else if(type == 1 && direction == 2)
			CallDeferred("ChangeNodeDef", 3, 0);

		GetNode<obj_character_map>("../").state = 3;
		GetNode<obj_character_map>("../").canMove = true;

		Visible = false;
	}

	public void ChangeNodeDef(int _pathID, float _progress)
	{
		Path2D pt = GetNode<Path2D>("../../../../pt_0" + _pathID);
		PathFollow2D pf = GetNode<PathFollow2D>("../../");

		GetNode<Path2D>("../../../").RemoveChild(GetNode<PathFollow2D>("../../"));
		pt.AddChild(pf);

		pf.Progress = _progress;

		GetNode<obj_character_map>("../").playerData.pathID = _pathID;
		GetNode<obj_character_map>("../").canMove = true;
	}

	private void GetControllerInput()
	{
		if(controllerIndex != -1)
		{
			joyhaxis = Input.GetAxis("left" + controllerIndex, "right" + controllerIndex);
			joyvaxis = Input.GetAxis("up" + controllerIndex, "down" + controllerIndex);

			if(Input.IsActionJustPressed("jump" + controllerIndex))
				ConfirmChoice();
		}
		else
		{
			if(delay <= 0)
			{
				if(!chosen)
				{
					chosen = true;
					delay = 0.4;
					if(((GameManager)GetNode("/root/GameManager")).rand.NextDouble() >= 0.5)
						direction = 1;
					else
						direction = 2;
				}
				else
				{
					ConfirmChoice();
				}
			}
		}
	}
}
