using Godot;
using System;
using System.Collections.Generic;

public partial class obj_ball : RigidBody2D
{
	private CollisionShape2D obj_collider;
	private Area2D obj_score;
	private Sprite2D spr_ball;
	private Random rand;

	private List<string> collisions;
	private bool scoring = false;

	private Vector2 newPosition = new Vector2(0, 0);
	private bool shouldSnap = false;

	private float scale = 2f;
	private bool landed = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		obj_collider = GetNode<CollisionShape2D>("obj_collider");
		spr_ball = GetNode<Sprite2D>("spr_ball");

		collisions = new List<string>();

		rand = new Random();

		ResetBall();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GD.Print(LinearVelocity.Y);
		if(!Freeze)
			scale = 3 - (LinearVelocity.Y/420);
		else
			return;

		bool colliding = false;
		foreach(Node2D body in GetCollidingBodies())
		{
			if(body.Name == "BOUND_ALL")
				colliding = true;
		}

		spr_ball.Rotation += LinearVelocity.X*0.0002f;

		if(!scoring && (scale < 2 || colliding))
		{
			scoring = true;
			Score();
		}

		spr_ball.Scale = new Vector2(scale, scale);
		obj_collider.Scale = new Vector2(scale/2, scale/2);
	}

	public void Unfreeze()
	{
		Freeze = false;
	}

	private void Score()
	{
		if(collisions.Contains("SIDE1"))
			if(!collisions.Contains("SIDE2"))
				GetNode<obj_playerScore2v>("../obj_minigameBase/Score2v2/spr_playerBar2/spr_playerScore").AddScore(1, ((GameManager)GetNode("/root/GameManager")).playerData[3]);
			else
			{
				ResetBall();
				return;
			}
		
		if(collisions.Contains("SIDE2"))
			GetNode<obj_playerScore2v>("../obj_minigameBase/Score2v2/spr_playerBar1/spr_playerScore").AddScore(1, ((GameManager)GetNode("/root/GameManager")).playerData[0]);

		ResetBall();
	}

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if(shouldSnap)
		{
			state.Transform = new Transform2D(0, new Vector2(1, 1), 0, newPosition);
			state.LinearVelocity = new Vector2(0, 0);
		}
		else
		{
			scoring = false;
		}
		shouldSnap = false;

		base._IntegrateForces(state);
	}

	private void ResetBall()
	{
		int random = rand.Next(0, 10);
		if(random > 4)
			newPosition = new Vector2(516, 276);
		else
			newPosition = new Vector2(768, 276);

		shouldSnap = true;
	}
	
	private void AreaEntered(Area2D area)
	{
		if(!collisions.Contains(area.Name))
			collisions.Add(area.Name);
	}


	private void AreaExited(Area2D area)
	{
		if(collisions.Contains(area.Name))
			collisions.Remove(area.Name);
	}
}
