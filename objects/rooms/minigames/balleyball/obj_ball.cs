using Godot;
using System;

public partial class obj_ball : RigidBody2D
{
	private CollisionShape2D obj_collider;
	private Sprite2D spr_ball;

	private Vector2 hitVelocity = new Vector2(0, 0);

	private float scale = 2f;
	private bool landed = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		obj_collider = GetNode<CollisionShape2D>("obj_collider");
		spr_ball = GetNode<Sprite2D>("spr_ball");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GD.Print(LinearVelocity.Y);
		if(!Freeze)
			scale = 3 - (LinearVelocity.Y/420);

		if(scale < 2)
			Freeze = true;

		spr_ball.Scale = new Vector2(scale, scale);
		obj_collider.Scale = new Vector2(scale/2, scale/2);
	}

	public override void _PhysicsProcess(double delta)
	{
		ApplyCentralImpulse(hitVelocity);
		hitVelocity = new Vector2(0, 0);
	}

	public void Hit(Vector2 velocity)
	{
		hitVelocity = velocity;
	}

	public void Unfreeze()
	{
		Freeze = false;
	}
}
