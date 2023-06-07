using Godot;
using System;

public partial class spc_shop : Area2D
{
	private CanvasLayer layer;
	private Sprite2D spr_arrow;
	private int index = 0;
	private Vector2[] positions;
	public PlayerData player;
	private Callable onEnd;

	private bool canMove = true;

	public int controllerIndex = -1;

	private float joyhaxis = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spr_arrow = GetNode<Sprite2D>("CanvasLayer/spr_arrow");
		layer = GetNode<CanvasLayer>("CanvasLayer");

		positions = new Vector2[] {
			new Vector2(148, 228), new Vector2(148, 250), new Vector2(148, 270)
		};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!layer.Visible)
			return;

		GetControllerInput();

		if(canMove)
		{
			if(joyhaxis > 0)
			{
				canMove = false;
				index++;
			}
			else if(joyhaxis < 0)
			{
				canMove = false;
				index--;
			}
		}
		
		if(!canMove)
			if(joyhaxis == 0)
				canMove = true;

		if(index < 0)
			index = 2;
		if(index > 2)
			index = 0;

		spr_arrow.Position = positions[index];

		if(Input.IsActionJustPressed("jump" + controllerIndex))
		{
			switch(index)
			{
				case 0:
					if(player.coins >= 7)
					{
						player.items.Add(((GameManager)GetNode("/root/GameManager")).itemLookup[1]);
						player.coins -= 7;
						layer.Visible = false;
						onEnd.Call(false);
					}
					break;
				case 1:
					if(player.coins >= 4)
					{
						player.items.Add(((GameManager)GetNode("/root/GameManager")).itemLookup[0]);
						player.coins -= 4;
						layer.Visible = false;
						onEnd.Call(false);
					}
					break;
				case 2:
					if(player.coins >= 6)
					{
						player.items.Add(((GameManager)GetNode("/root/GameManager")).itemLookup[3]);
						player.coins -= 6;
						layer.Visible = false;
						onEnd.Call(false);
					}
					break;
			}
		}
	}

	private void GetControllerInput()
	{
		joyhaxis = Input.GetAxis("up" + controllerIndex, "down" + controllerIndex);
	}

	public void WakeUp(PlayerData data, Callable _onEnd)
	{
		onEnd = _onEnd;
		player = data;
		if(player.items.Count < 3)
			layer.Visible = true;
		else
			onEnd.Call(false);

		controllerIndex = player.controllerIndex;
	}
}
