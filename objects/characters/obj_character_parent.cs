using Godot;
using System;
using System.Collections.Generic;

public partial class obj_character_parent : RigidBody2D
{
	public int Player = 0;
	private PlayerData playerData;
	private List<string> abilities;
	public int CharacterIndex {get {return playerData.characterIndex; }}

	private int state = 0;
	private bool minigameStarted = false;

	public int controllerIndex = 1;

	public float movementSpeed = 450f;
	public float scale = 4;
	public float density = 1f;
	public float strength = 400f;
	public bool Lost = false;
	public Vector2 aiTarget = new Vector2(0, 0);

	private Area2D feetArea;
	private CollisionShape2D collider;
	private AnimatedSprite2D sprite;
	private obj_jumpbar jumpBar;

	private float joyhaxis = 0;
	private float joyvaxis = 0;

	private Vector2 velocity = new Vector2(0, 0);
	private float jumpHeight = 600;
	private float idleTimer = 0;
	private short flip = 1;
	public bool jumping = false;

	private Alarm t_jumpDuration;

	public override void _Ready()
	{
		feetArea = GetNode<Area2D>("obj_feetArea");
		collider = GetNode<CollisionShape2D>("obj_hitbox");
		sprite = GetNode<AnimatedSprite2D>("obj_sprite");
		jumpBar = GetNode<obj_jumpbar>("obj_jumpbar");

		Variant _State = GetMeta("State");
		state = _State.As<int>();

		Variant _Player = GetMeta("Player");
		Player = _Player.As<int>() - 1;

		playerData = ((GameManager)GetNode("/root/GameManager")).playerData[Player];
		abilities = ((GameManager)GetNode("/root/GameManager")).minigameLookup[((GameManager)GetNode("/root/GameManager")).CurrentMinigame].Abilities;
		sprite.SpriteFrames = playerData.animationFrames;
		
		controllerIndex = playerData.controllerIndex;
		movementSpeed *= playerData.speedMult;
		density *= playerData.weightMult;
		strength *= playerData.strengthMult;

		t_jumpDuration = new Alarm(0, true, this, new Callable(this, "StopJump"), false);

		if(HasMeta("MovementSpeed"))
		{
			Variant _movementSpeed = GetMeta("MovementSpeed");
			movementSpeed = _movementSpeed.As<float>();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!((GameManager)GetNode("/root/GameManager")).MinigameStarted)
		{
			joyhaxis = 0;
			joyvaxis = 0;
			sprite.Play("idle");
			return;
		}
			
		
		GetControllerInput();

		idleTimer += (float)delta;
		ProcessAnimations();

		switch(state)
		{
			case 0:
				GameMap();
				break;
			case 1:
				MushMixUp();
				break;
		}
	}

	private void GameMap()
	{
		
	}

	private void MushMixUp()
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		velocity = new Vector2(joyhaxis * movementSpeed, joyvaxis * movementSpeed);
		ApplyCentralImpulse(velocity);

		//feetArea.Position = Position;
	}

	private void GetControllerInput()
	{
		if(controllerIndex == -1)
		{
			ProcessAI();
			return;
		}

		if(abilities.Contains("move"))
		{
			joyhaxis = Input.GetAxis("left" + controllerIndex, "right" + controllerIndex);
			joyvaxis = Input.GetAxis("up" + controllerIndex, "down" + controllerIndex);
		}

		if(abilities.Contains("jump") && Input.IsActionJustPressed("jump" + controllerIndex))
		{
			if(abilities.Contains("jump_bar"))
			{
				if(jumpBar.JumpTime > 0)
				{
					jumping = true;
					sprite.Modulate = new Color(1, 1, 1, 0.5f);
					collider.Disabled = true;
					t_jumpDuration.WaitTime = jumpBar.Jump * 0.5f;
					t_jumpDuration.Start();
				}
			}
			else if(abilities.Contains("jump_phys"))
				if(velocity.Y == 0)
					velocity.Y = jumpHeight;
		}
	}

	private void ProcessAnimations()
	{
		if(joyhaxis == 0 && joyvaxis == 0)
		{
			if(idleTimer > 5)
			{
				sprite.SpeedScale = 1;
				sprite.Play("idle");
			}
			else
				sprite.Frame = 0;
			return;
		}

		idleTimer = 0;

		if(joyhaxis > 0)
			flip = 1;
		else if(joyhaxis < 0)
			flip = -1;

		if((joyhaxis > joyvaxis && joyhaxis > -joyvaxis) || (-joyhaxis > joyvaxis && -joyhaxis > -joyvaxis))
		{
			sprite.SpeedScale = Mathf.Clamp(Mathf.Abs(joyhaxis), 0.2f, 1);
			sprite.Play("walkRight");
		}
		else if(-joyvaxis > joyhaxis && -joyvaxis > -joyhaxis)
		{
			sprite.SpeedScale = Mathf.Clamp(Mathf.Abs(joyvaxis), 0.2f, 1);
			sprite.Play("walkDown");
		}
		else if(joyvaxis > joyhaxis && joyvaxis > -joyhaxis)
		{
			sprite.SpeedScale = Mathf.Clamp(Mathf.Abs(joyvaxis), 0.2f, 1);
			sprite.Play("walkUp");
		}

		sprite.Scale = new Vector2(scale * flip, scale * 1);
	}

	public void StopJump()
	{
		sprite.Modulate = new Color(1, 1, 1, 1);
		collider.Disabled = false;
		jumping = false;
	}

	private void ProcessAI()
	{
		switch(((GameManager)GetNode("/root/GameManager")).CurrentMinigame)
		{
			case 1:
				float distanceX = (Position.X / aiTarget.X - 0.1f);
				if(distanceX < 0.9f)
					distanceX = -(aiTarget.X / Position.X - 0.1f);
				joyhaxis = -Mathf.Clamp(distanceX, -1, 1);
				if(Math.Abs(joyhaxis) < 0.98f)
					joyhaxis = 0;
				
				float distanceY = (Position.Y / aiTarget.Y - 0.1f);
				if(distanceY < 0.9f)
					distanceY = -(aiTarget.Y / Position.Y - 0.1f);
				joyvaxis = -Mathf.Clamp(distanceY, -1, 1);
				if(Math.Abs(joyvaxis) < 0.98f)
					joyvaxis = 0;
				break;
		}
	}
}
