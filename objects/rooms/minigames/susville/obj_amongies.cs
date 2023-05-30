using Godot;
using System;
using System.Collections.Generic;

public partial class obj_amongies : Node2D
{
	private double delay = 0.05;
	private List<RigidBody2D> amongies;

	private AnimationPlayer anim_lights;
	private CollisionShape2D obj_wall1;
	private CollisionShape2D obj_wall2;

	private int count = 29;
	public bool fadeOut = false;

	private Random rand;
	//private bool spawning = false;
	private int index = 0;
	private int imposter = 0;
	private bool spawned = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		anim_lights = GetNode<AnimationPlayer>("../anim_lights");
		obj_wall1 = GetNode<CollisionShape2D>("../obj_bg/obj_wall1");
		obj_wall2 = GetNode<CollisionShape2D>("../obj_bg/obj_wall2");
		amongies = new List<RigidBody2D>();
		rand = new Random();

		imposter = rand.Next(0, count);

		anim_lights.Play("start");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		delay -= delta;
		if(!spawned && delay <= 0)
		{
			delay = 0.05;

			if(index > count)
			{
				((obj_amongus)amongies[imposter]).CanKill = true;
				spawned = true;
				return;
			}

			RigidBody2D amongus = GD.Load<PackedScene>("objects/rooms/minigames/susville/obj_amongus.tscn").Instantiate<RigidBody2D>();
			if(rand.NextDouble() > 0.5)
				amongus.Position = new Vector2(0, 400);
			else
				amongus.Position = new Vector2(1280, 400);

			if(index == imposter)
			{
				((obj_amongus)amongus).Imposter = true;
				amongus.GetNode<Area2D>("hitDetect").Monitoring = true;
			}

			AddChild(amongus);
			amongies.Add(amongus);
		
			index++;
		}
	}

	public void StartGame()
	{
		if(!((GameManager)GetNode("/root/GameManager")).MinigameStarted)
			GetNode<obj_timerText>("../obj_minigameBase/spr_timer/obj_text").Start();
	}

	public void ImposterDied()
	{	
		anim_lights.Seek(5);
		anim_lights.PlayBackwards();

		fadeOut = true;
	}

	public void SpawnAmongus()
	{
		fadeOut = false;
		amongies.Clear();
		spawned = false;
		index = 0;

		obj_wall1.Disabled = true;
		obj_wall2.Disabled = true;

		anim_lights.Play();
	}

	public void EnableCollision()
	{
		obj_wall1.Disabled = false;
		obj_wall2.Disabled = false;
	}
}
