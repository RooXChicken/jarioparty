using Godot;
using System;
using System.Collections.Generic;

public partial class obj_character_parent : RigidBody2D
{
	public int Player = 0;
	public PlayerData playerData;
	private List<string> abilities;
	public int CharacterIndex {get {return playerData.characterIndex; }}
	public int controllerIndex = 1;

	public float movementSpeed = 450f;
	public float scale = 3;
	public float density = 1f;
	public float strength = 400f;
	public bool Lost = false;
	public Vector2 aiTarget = new Vector2(0, 0);

	private Area2D feetArea;
	private CollisionShape2D collider;
	private obj_interaction obj_interact;
	private AnimatedSprite2D sprite;
	private obj_jumpbar jumpBar;
	private obj_shadow shadow;
	private AnimationPlayer animation;
	private bool justJumped = false;

	private float joyhaxis = 0;
	private float joyvaxis = 0;

	private float animhaxis = 0;
	private float animvaxis = 0;

	private short lastDirection = 0;
	public bool joyLock = false;
	private bool resetAnim = true;

	private Vector2 velocity = new Vector2(0, 0);
	private float jumpHeight = 700;
	private float jumpCountdown = 0;
	private float idleTimer = 0;
	private short flip = 1;
	public bool jumping = false;

	private Alarm t_jumpDuration;
	public Alarm t_stun;

	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/player/snd_jump.wav", "plr_jump");
		joyhaxis = 0;
		joyvaxis = 0;
		
		feetArea = GetNode<Area2D>("obj_feetArea");
		collider = GetNode<CollisionShape2D>("obj_hitbox");
		sprite = GetNode<AnimatedSprite2D>("scaleManager/obj_sprite");
		animation = GetNode<AnimationPlayer>("animations");
		obj_interact = GetNode<obj_interaction>("obj_interaction");

		Variant _Player = GetMeta("Player");
		Player = _Player.As<int>() - 1;

		playerData = ((GameManager)GetNode("/root/GameManager")).playerData[Player];
		abilities = ((GameManager)GetNode("/root/GameManager")).minigameLookup[((GameManager)GetNode("/root/GameManager")).CurrentMinigame].Abilities;
		if(abilities.Contains("jump_bar"))
			jumpBar = GetNode<obj_jumpbar>("obj_jumpbar");
		sprite.SpriteFrames = playerData.animationFrames;

		t_stun = new Alarm(0.3, true, this, new Callable(this, "EndStun"), false);

		scale = ((GameManager)GetNode("/root/GameManager")).minigameLookup[((GameManager)GetNode("/root/GameManager")).CurrentMinigame].Scale;
		scale /= 3;
		//GD.Print(scale);
		sprite.Scale = new Vector2(3 * scale, 3 * scale);
		collider.Scale = new Vector2(4 * scale, 4 * scale);
		feetArea.Scale = new Vector2(1 * scale, 1 * scale);

		if(abilities.Contains("jump_phy"))
		{
			GravityScale = 1.3f;
			PhysicsMaterialOverride = null;
			LinearDamp = 0;

			animvaxis = 1;
			joyLock = true;
		}

		controllerIndex = playerData.controllerIndex;
		movementSpeed *= playerData.speedMult;
		density *= playerData.weightMult;
		strength *= playerData.strengthMult;

		t_jumpDuration = new Alarm(0.1, true, this, new Callable(this, "StopJump"), false);

		if(HasMeta("MovementSpeed"))
		{
			Variant _movementSpeed = GetMeta("MovementSpeed");
			movementSpeed = _movementSpeed.As<float>();
		}

		if(abilities.Contains("shadow"))
		{
			shadow = (obj_shadow)GetNode<Sprite2D>("scaleManager/obj_shadow");
			shadow.Visible = true;

			shadow.Scale = new Vector2(3 * scale, 3 * scale);
		}

		SpriteChanged();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!((GameManager)GetNode("/root/GameManager")).MinigameStarted)
		{
			return;
		}
			
		GetControllerInput();

		idleTimer += (float)delta;
		jumpCountdown -= (float)delta;
		ProcessAnimations();
	}

	public override void _PhysicsProcess(double delta)
	{
		ApplyCentralImpulse(new Vector2(joyhaxis * movementSpeed, joyvaxis * movementSpeed) + velocity);
		velocity = new Vector2(0, 0);
		justJumped = false;
	}

	private void GetControllerInput()
	{
		if(controllerIndex < 0)
		{
			if(controllerIndex == -1)
				ProcessAI();
			return;
		}

		if(!joyLock && abilities.Contains("move"))
		{
			joyhaxis = Input.GetAxis("left" + controllerIndex, "right" + controllerIndex);
			joyvaxis = Input.GetAxis("up" + controllerIndex, "down" + controllerIndex);

			animhaxis = joyhaxis;
			animvaxis = joyvaxis;
		}

		if(abilities.Contains("punch") && Input.IsActionJustPressed("punch" + controllerIndex) && sprite.Animation != "punch")
		{
			joyLock = true;
			sprite.Animation = "punch";

			resetAnim = false;
			ResetJoystick();
		}

		if(abilities.Contains("jump") && Input.IsActionJustPressed("jump" + controllerIndex))
		{
			jumpCountdown = 0.2f;
		}

		if(Position.Y > 458 && LinearVelocity.Y > 0)
		{
			jumping = false;
		}

		if(abilities.Contains("jump") && jumpCountdown > 0)
		{
			if(abilities.Contains("jump_bar"))
			{
				if(jumpBar.JumpTime > 0.2f)
				{
					((AudioController)GetNode("/root/AudioController")).PlaySound("plr_jump");
					jumping = true;
					justJumped = true;
					sprite.Modulate = new Color(1, 1, 1, 0.5f);
					collider.Disabled = true;
					t_jumpDuration.WaitTime = jumpBar.Jump * 0.5f;
					t_jumpDuration.Start();
				}
			}
			else if(abilities.Contains("jump_phy"))
				if(LinearVelocity.Y == 0 && !justJumped)
				{
					((AudioController)GetNode("/root/AudioController")).PlaySound("plr_jump");
					velocity.Y = -jumpHeight;
					jumping = true;
					justJumped = true;
				}
		}

		if(abilities.Contains("jump_phy") && !Input.IsActionPressed("jump" + controllerIndex) && LinearVelocity.Y < 0)
		{
			StopJump();
		}
	}

	private void ProcessAnimations()
	{
		if(controllerIndex < -1 || (animhaxis == 0 && animvaxis == 0) && resetAnim)
		{
			if(idleTimer > 5 && abilities.Contains("move"))
			{
				sprite.SpeedScale = 1;
				sprite.Play("idle");
			}
			else
				sprite.Frame = 0;
			return;
		}

		idleTimer = 0;

		if(animhaxis > 0)
			flip = 1;
		else if(animhaxis < 0)
			flip = -1;

		string action = "null";

		if(jumping)
		{
			action = "jump";
			sprite.SpeedScale = 0;
		}
		else
		{
			action = "walk";
			sprite.SpeedScale = 0;
		}

		if(abilities.Contains("move"))
		{
			sprite.SpeedScale = Mathf.Clamp(Mathf.Abs(animhaxis + animvaxis), 0.2f, 1);
		}

		if(sprite.Animation == "punch")
		{
			sprite.SpeedScale = 1;
		}

		if((animhaxis > animvaxis && animhaxis > -animvaxis) || (-animhaxis > animvaxis && -animhaxis > -animvaxis))
		{
			lastDirection = 1;
			sprite.Play(action + "Right");
		}
		else if(-animvaxis > animhaxis && -animvaxis > -animhaxis)
		{
			lastDirection = 2;
			sprite.Play(action + "Down");
		}
		else if(animvaxis > animhaxis && animvaxis > -animhaxis)
		{
			lastDirection = 3;
			sprite.Play(action + "Up");
		}

		sprite.Scale = new Vector2(3 * scale * flip, 3 * scale * 1);
		obj_interact.Scale = new Vector2(flip, 1);
	}

	public void Burn()
	{
		if(controllerIndex != -2)
		{
			controllerIndex = -2;
			((AudioController)GetNode("/root/AudioController")).PlaySound("jumprope_burn");
			animation.Play("out_burn");
		}
	}

	public void StopJump()
	{
		if(abilities.Contains("jump_bar"))
		{
			sprite.Modulate = new Color(1, 1, 1, 1);
			collider.Disabled = false;
			jumping = false;
		}
		else if(abilities.Contains("jump_phy"))
		{
			velocity.Y = -LinearVelocity.Y * 0.3f;
		}
	}

	private void ProcessAI()
	{
		// switch(((GameManager)GetNode("/root/GameManager")).CurrentMinigame)
		// {
		// 	case 1:
		// 		float distanceX = (Position.X / aiTarget.X - 0.1f);
		// 		if(distanceX < 0.9f)
		// 			distanceX = -(aiTarget.X / Position.X - 0.1f);
		// 		joyhaxis = -Mathf.Clamp(distanceX, -1, 1);
		// 		if(Math.Abs(joyhaxis) < 0.98f)
		// 			joyhaxis = 0;
				
		// 		float distanceY = (Position.Y / aiTarget.Y - 0.1f);
		// 		if(distanceY < 0.9f)
		// 			distanceY = -(aiTarget.Y / Position.Y - 0.1f);
		// 		joyvaxis = -Mathf.Clamp(distanceY, -1, 1);
		// 		if(Math.Abs(joyvaxis) < 0.98f)
		// 			joyvaxis = 0;
		// 		break;
		// }

		animhaxis = joyhaxis;
		animvaxis = joyvaxis;
	}

	private void SpriteChanged()
	{
		if(abilities.Contains("shadow"))
		{
			//GD.Print(sprite.Animation + " | " + sprite.Frame);
			shadow.Texture = sprite.SpriteFrames.GetFrameTexture(sprite.Animation, sprite.Frame);
		}

		if(sprite.Animation == "punch")
		{
			switch(sprite.Frame)
			{
				case 3:
					foreach(Area2D area in obj_interact.collisions)
					{
						area.GetNode<RigidBody2D>("../").ApplyCentralImpulse(new Vector2(10500, 0));
						area.GetNode<obj_character_parent>("../").t_stun.Start();
						area.GetNode<obj_character_parent>("../").joyLock = true;
						area.GetNode<obj_character_parent>("../").ResetJoystick();
						area.GetNode<obj_character_parent>("../").sprite.Animation = "hurt";
					}
					break;
				case 10:
					joyLock = false;
					resetAnim = true;
					break;
			}
		}
	}

	public void EndStun()
	{
		joyLock = false;
	}

	public void PlayAnimation(string anim)
	{
		animation.Play(anim);
	}

	public void ResetJoystick()
	{
		joyhaxis = 0;
		joyvaxis = 0;

		animhaxis = joyhaxis;
		animvaxis = joyvaxis;
	}
}
