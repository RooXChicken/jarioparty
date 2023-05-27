using Godot;
using System;
using System.Collections.Generic;

public partial class obj_amongies : Node2D
{
	private double delay = 0.05;
	private List<RigidBody2D> amongies;

	private AnimationPlayer anim_lights;

	private Random rand;
	//private bool spawning = false;
	private int index = 0;
	private int imposter = 0;
	private bool spawned = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		anim_lights = GetNode<AnimationPlayer>("../anim_lights");
		amongies = new List<RigidBody2D>();
		rand = new Random();

		imposter = rand.Next(0, 29);

		anim_lights.Play("start");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		delay -= delta;
		if(!spawned && delay <= 0)
		{
			delay = 0.05;

			if(index > 29)
			{
				((obj_amongus)amongies[imposter - 1]).CanKill = true;
				GD.Print(imposter);
				spawned = true;
				return;
			}

			index++;

			RigidBody2D amongus = GD.Load<PackedScene>("objects/rooms/minigames/susville/obj_amongus.tscn").Instantiate<RigidBody2D>();
			if(rand.NextDouble() > 0.5)
				amongus.Position = new Vector2(50, 400);
			else
				amongus.Position = new Vector2(1230, 400);
			
			if(index == imposter)
			{
				GD.Print(index);
				((obj_amongus)amongus).Imposter = true;
				amongus.GetNode<Area2D>("hitDetect").Monitoring = true;
			}

			AddChild(amongus);
			amongies.Add(amongus);
		}
	}

	public void StartGame()
	{
		if(!((GameManager)GetNode("/root/GameManager")).MinigameStarted)
			GetNode<obj_timerText>("../obj_minigameBase/spr_timer/obj_text").Start();
	}

	public void ImposterDied()
	{
		foreach(RigidBody2D amogus in amongies)
			((obj_amongus)amogus).FadeOut();
		GD.Print("Completed");
		anim_lights.Seek(5);
		anim_lights.PlayBackwards();
	}

	public void SpawnAmongus()
	{
		amongies.Clear();
		spawned = false;
		index = 0;

		anim_lights.Play();
	}
}
