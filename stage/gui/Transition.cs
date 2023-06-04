using Godot;
using System;

public partial class Transition : Node2D
{
	public byte state = 0;
	public int playerGoing = 0;
	public bool snap = false;

	private float speed = 1000;

	private Sprite2D obj_top;
	private Sprite2D obj_bottom;
	private Alarm t_wait;
	private AnimationPlayer anim_transition;

	public Callable transitionEnd;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		anim_transition = GetNode<AnimationPlayer>("../anim_transition");
		obj_top = GetNode<Sprite2D>("obj_top");
		obj_bottom = GetNode<Sprite2D>("obj_bottom");

		t_wait = new Alarm(0.5, true, this, new Callable(this, "TransitionEnd"), false);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		switch(state)
		{
			case 1:
				if(playerGoing == 4)
				{
					GetNode<obj_map>("../../").SetZoomLevel(0.45f);
					anim_transition.Play("spinnertoCloud");
					state = 4;
				}
				else
				{
					obj_top.Scale = new Vector2(obj_top.Scale.X, obj_top.Scale.Y + (float)delta * speed);
					obj_bottom.Scale = new Vector2(obj_bottom.Scale.X, obj_bottom.Scale.Y - (float)delta * speed);

					if(obj_top.Scale.Y >= 360)
					{
						t_wait.Start();
						state = 3;
					}
				}
				break;
			case 2:
				obj_top.Scale = new Vector2(obj_top.Scale.X, obj_top.Scale.Y - (float)delta * speed);
				obj_bottom.Scale = new Vector2(obj_bottom.Scale.X, obj_bottom.Scale.Y + (float)delta * speed);
				if(obj_top.Scale.Y <= 0)
				{
					state = 3;
					//GetNode<obj_playerStart>("../PlayerStart").StartAnimation(playerGoing);
				}
				break;
			case 3:
				GetNode<obj_mapGUI>("../../obj_mapGUI").SwitchPlayers(playerGoing);
				state = 4;
				break;

			case 5:
				obj_top.Scale = new Vector2(obj_top.Scale.X, obj_top.Scale.Y + (float)delta * speed);
				obj_bottom.Scale = new Vector2(obj_bottom.Scale.X, obj_bottom.Scale.Y - (float)delta * speed);

				if(obj_top.Scale.Y >= 360)
				{
					t_wait.Start();
					transitionEnd.Call();
					state = 7;
					//GetNode<obj_playerStart>("../PlayerStart").StartAnimation(playerGoing);
				}
				break;
			case 6:
				obj_top.Scale = new Vector2(obj_top.Scale.X, obj_top.Scale.Y - (float)delta * speed);
				obj_bottom.Scale = new Vector2(obj_bottom.Scale.X, obj_bottom.Scale.Y + (float)delta * speed);
				if(obj_top.Scale.Y <= 0)
				{
					transitionEnd.Call();
					state = 4;
				}
				break;
		}
	}

	public void TransitionEnd()
	{
		if(state == 7)
		{
			state = 6;
		}
		else if(playerGoing == 4)
		{
			((GameManager)GetNode("/root/GameManager")).TurnsLeft--;
			//if(state == 5)
			((GameManager)GetNode("/root/GameManager")).SwitchScene("rm_minigame_info");
			// else
			// {
			// 	GetNode<obj_cloudTransition>("../obj_cloudTransition").Visible = true;
			// 	GetNode<obj_cloudTransition>("../obj_cloudTransition").PlayAnimation();
			// 	t_wait.WaitTime = 1.5;
			// 	t_wait.Start();
			// 	state = 5;
			// }
		}
		else
		{
			state = 2;
			GetNode<obj_diceBlock>("../../obj_diceBlock").Initialize();
			if(snap)
			{
				GetNode<obj_map>("../../").SetPlayer(++playerGoing);
				GetNode<obj_map>("../../").Snap();
			}
			else
				GetNode<obj_map>("../../").SetPlayer(++playerGoing);
		}
	}
}
