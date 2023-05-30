using Godot;
using System;
using System.Collections.Generic;

public partial class obj_amongus : RigidBody2D
{
	private Random rand;
	public bool Imposter = false;
	private AnimatedSprite2D spr_amongus;
	private AnimatedSprite2D spr_blood;
	private CanvasLayer layer;
	public bool Dead;
	public bool CanKill = false;

	public int Index = -1;

	private Vector2 aiTarget = new Vector2(0, 0);
	private int state = 0;
	private float movementSpeed = 120;
	private double alpha = 1;

	private List<obj_amongus> collisions;
	private bool fadeOut = false;

	private Alarm t_ai;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		rand = new Random();
		spr_amongus = GetNode<AnimatedSprite2D>("spr_amongus");
		spr_blood = GetNode<AnimatedSprite2D>("CanvasLayer/spr_blood");
		layer = GetNode<CanvasLayer>("CanvasLayer");

		spr_amongus.Material = new ShaderMaterial() { Shader = (spr_amongus.Material as ShaderMaterial).Shader.Duplicate() as Shader};
		float newHue = (float)rand.NextDouble() * 6;
		((ShaderMaterial)spr_amongus.Material).SetShaderParameter("newHue", newHue);
		((ShaderMaterial)spr_amongus.Material).SetShaderParameter("alpha", 1);

		aiTarget = new Vector2(rand.Next(112, 1168), rand.Next(112, 692));
		collisions = new List<obj_amongus>();

		t_ai = new Alarm(rand.NextDouble() * 3, false, this, new Callable(this, "ChangeAI"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		layer.Offset = Position;
		if(fadeOut)
		{
			alpha -= delta;
			((ShaderMaterial)spr_amongus.Material).SetShaderParameter("alpha", alpha);
			if(alpha <= 0)
				QueueFree();
		}

		if(GetNode<obj_amongies>("../").fadeOut)
			fadeOut = true;
	}

	private bool KillAmongUs(obj_amongus among)
	{
		if(among.Dead)
			return false;
		collisions.Remove(among);
		among.Kill(true);

		return true;
	}

	public override void _PhysicsProcess(double delta)
	{
		if(Dead)
			return;
		if(state != 3)
		{
			float xVel = (Position.X / aiTarget.X - 0.1f);
			float yVel = (Position.Y / aiTarget.Y - 0.1f);

			if(xVel < 0.9f)
				xVel = -(aiTarget.X / Position.X - 0.1f);

			xVel = -Mathf.Clamp(xVel, -1, 1);

			if(Math.Abs(xVel) < 0.98f)
				xVel = 0;

			//

			if(yVel < 0.9f)
				yVel = -(aiTarget.Y / Position.Y - 0.1f);
				
			yVel = -Mathf.Clamp(yVel, -1, 1);

			if(Math.Abs(yVel) < 0.98f)
				yVel = 0;

			if(xVel == 0 && yVel == 0)
				spr_amongus.Frame = 0;

			ApplyCentralImpulse(new Vector2(xVel * movementSpeed, yVel * movementSpeed));

			if(xVel < 0)
				spr_amongus.Scale = new Vector2(2, 2);
			else
				spr_amongus.Scale = new Vector2(-2, 2);
		}
	}

	private void ChangeAI()
	{
		if(Imposter && CanKill)
		{
			int index = 0;
			while(index < collisions.Count && !KillAmongUs(collisions[index]))
				index++;
		}
		aiTarget = new Vector2(rand.Next(112, 1168), rand.Next(112, 692));
		state = rand.Next(1, 3);
		t_ai.WaitTime = rand.NextDouble() * 3;
	}

	public void Kill(bool imposterKill = false)
	{
		Dead = true;
		if(!imposterKill)
			((AudioController)GetNode("/root/AudioController")).PlaySound("susville_amongusKill");
		else
			((AudioController)GetNode("/root/AudioController")).PlaySound("susville_imposterKill");
		spr_amongus.Frame = 0;
		spr_amongus.Animation = "dead";
		spr_amongus.Play();
		spr_blood.Play();
		spr_blood.Visible = true;

		ZIndex--;

		if(Imposter)
		{
			GetNode<obj_amongies>("../").ImposterDied();
		}
	}

	private void AreaEntered(Area2D area)
	{
		if(area.Name != "hitDetect")
			return;
		if(!collisions.Contains(area.GetNode<obj_amongus>("../")))
		{
			collisions.Add(area.GetNode<obj_amongus>("../"));
		}
	}

	private void AreaExited(Area2D area)
	{
		if(area.Name != "hitDetect")
			return;
		if(collisions.Contains(area.GetNode<obj_amongus>("../")))
		{
			collisions.Remove(area.GetNode<obj_amongus>("../"));
		}
	}
}
