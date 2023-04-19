using Godot;
using System;

public partial class obj_character_parent : RigidBody2D
{
	public int Player = 0;
	private PlayerData playerData;
	public int CharacterIndex {get {return playerData.characterIndex; }}

	private int state = 0;
	private bool minigameStarted = false;

	public int controllerIndex = 1;

	public float movementSpeed = 250f;
	public float scale = 4;
	public float density = 1f;
	public float strength = 400f;
	public bool Lost = false;

	private Area2D feetArea;
	private CollisionShape2D collider;
	private AnimatedSprite2D sprite;

	private float joyhaxis = 0;
	private float joyvaxis = 0;

	private Vector2 velocity = new Vector2(0, 0);
	private float idleTimer = 0;
	private short flip = 1;

	public override void _Ready()
	{
		feetArea = GetNode<Area2D>("obj_feetArea");
		collider = GetNode<CollisionShape2D>("obj_hitbox");
		sprite = GetNode<AnimatedSprite2D>("obj_sprite");

		Variant _State = GetMeta("State");
		state = _State.As<int>();

		Variant _Player = GetMeta("Player");
		Player = _Player.As<int>() - 1;

		playerData = ((GameManager)GetNode("/root/GameManager")).playerData[Player];
		sprite.SpriteFrames = playerData.animationFrames;
		
		controllerIndex = playerData.controllerIndex;
		movementSpeed *= playerData.speedMult;
		density *= playerData.weightMult;
		strength *= playerData.strengthMult;

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
			return;
		
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
			return;

		joyhaxis = Input.GetAxis("left" + controllerIndex, "right" + controllerIndex);
		joyvaxis = Input.GetAxis("up" + controllerIndex, "down" + controllerIndex);
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
}
