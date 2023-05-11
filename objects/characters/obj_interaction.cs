using Godot;
using System;
using System.Collections.Generic;

public partial class obj_interaction : Area2D
{
	public List<Area2D> collisions;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		collisions = new List<Area2D>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnBodyEntered(Area2D body)
	{
		if(body.Name == "obj_feetArea" && !collisions.Contains(body) && body != GetNode<Area2D>("../obj_feetArea"))
		{
			collisions.Add(body);
			GD.Print(body.Name);
		}

	}

	private void OnBodyExit(Area2D body)
	{
		if(collisions.Contains(body))
			collisions.Remove(body);
	}

}
