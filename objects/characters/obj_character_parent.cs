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
	private int punchForce = 8000;

	private float joyhaxis = 0;
	private float joyvaxis = 0;

	private float animhaxis = 0;
	private float animvaxis = 0;

	private short lastDirection = 1;
	public bool joyLock = false;
	private bool resetAnim = true;

	private Vector2 velocity = new Vector2(0, 0);
	private float jumpHeight = 1400;
	private float jumpCountdown = 0;
	private float idleTimer = 0;
	private short flip = 1;
	public bool jumping = false;
	private float groundY = 0;

	private Alarm t_jumpDuration;
	private Alarm t_startJump;
	public Alarm t_stun;

	private Random rand;

	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/player/snd_jump.wav", "plr_jump");
		joyhaxis = 0;
		joyvaxis = 0;

		rand = new Random();
		
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
		t_startJump = new Alarm(0.1, true, this, new Callable(this, "FireJump"), false);

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
			if(((GameManager)GetNode("/root/GameManager")).MinigameOver)
				sprite.Animation = "win";
			return;
		}
			
		GetControllerInput();

		idleTimer += (float)delta;
		jumpCountdown -= (float)delta;
		ProcessAnimations();
	}

	public override void _PhysicsProcess(double delta)
	{
		if(justJumped)
		{
			//ApplyCentralImpulse(-LinearVelocity);
			ApplyCentralImpulse(new Vector2(0, -jumpHeight - (LinearVelocity.Y*2)));
			//GD.Print("+LINVEL: " + LinearVelocity + " -LINVEL: " + -LinearVelocity);
		}
		else
			ApplyCentralImpulse(new Vector2(joyhaxis * movementSpeed, joyvaxis * movementSpeed) + velocity);

		// if(controllerIndex == 1)
		// 	GD.Print(joyhaxis * movementSpeed + " | " + joyvaxis * movementSpeed + " || " + velocity);
		
		velocity = new Vector2(0, 0);
		justJumped = false;

		if(jumping && abilities.Contains("jump_balley") && Position.Y > groundY)
		{
			//GD.Print(Position.Y + " > " + groundY);
			StopJump();
		}
	}

	private void GetControllerInput()
	{
		if(Lost)
		{
			ResetJoystick();
			return;
		}
		if(controllerIndex < 0)
		{
			if(controllerIndex == -1)
				ProcessAI();
			//return;
		}

		if(!joyLock && controllerIndex >= 0 && abilities.Contains("move"))
		{
			joyhaxis = Input.GetAxis("left" + controllerIndex, "right" + controllerIndex);
			joyvaxis = Input.GetAxis("up" + controllerIndex, "down" + controllerIndex);

			animhaxis = joyhaxis;
			animvaxis = joyvaxis;
		}

		if(abilities.Contains("punch") && controllerIndex >= 0 && Input.IsActionJustPressed("punch" + controllerIndex) && sprite.Animation != "punch")
		{
			joyLock = true;
			sprite.Animation = "attack";
			sprite.Frame = 0;

			resetAnim = false;
			ResetJoystick();
		}

		if(abilities.Contains("jump") && controllerIndex >= 0 && Input.IsActionJustPressed("jump" + controllerIndex))
		{
			jumpCountdown = 0.2f;
		}

		if(!abilities.Contains("jump_balley") && Position.Y > 458 && LinearVelocity.Y > 0)
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
			{
				if(LinearVelocity.Y == 0 && !justJumped)
				{
					((AudioController)GetNode("/root/AudioController")).PlaySound("plr_jump");
					velocity.Y = -jumpHeight;
					jumping = true;
					justJumped = true;
				}
			}
			else if(abilities.Contains("jump_balley"))
			{
				if(!jumping && !justJumped)
				{
					((AudioController)GetNode("/root/AudioController")).PlaySound("plr_jump");
					GravityScale = 1.3f;
					LinearDamp = 0;
					joyLock = true;
					ResetJoystick(false);
					groundY = Position.Y+1;
					// velocity.X = 0;
					// velocity.Y = 0;
					//ApplyCentralImpulse(new Vector2(-LinearVelocity.X, -LinearVelocity.Y - jumpHeight));
					jumping = true;
					justJumped = true;
					jumpCountdown = -1;
					SetCollisionMaskValue(2, false);
					resetAnim = false;
				}
			}
		}

		if(abilities.Contains("jump_phy") && controllerIndex >= 0 && !Input.IsActionPressed("jump" + controllerIndex) && LinearVelocity.Y < 0)
		{
			StopJump();
		}
	}

	private void ProcessAnimations()
	{
		if(resetAnim && (controllerIndex < -2 || (animhaxis == 0 && animvaxis == 0)))
		{
			if(idleTimer > 5 && abilities.Contains("move"))
			{
				sprite.SpeedScale = 1;
				sprite.Play("idle");
			}
			else
				sprite.Frame = 0;
			if(jumping)
			{
				Animate("jump");
			}
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

		if(sprite.Animation == "attack")
		{
			sprite.SpeedScale = 1;
		}

		Animate(action);

		if(-animhaxis > animvaxis && -animhaxis > -animvaxis)
			lastDirection = 2;

		sprite.Scale = new Vector2(3 * scale * flip, 3 * scale * 1);
		obj_interact.Scale = new Vector2(flip, 1);
	}

	private void Animate(string action)
	{
		if((animhaxis > animvaxis && animhaxis > -animvaxis) || (-animhaxis > animvaxis && -animhaxis > -animvaxis))
		{
			lastDirection = 1;
			//obj_interact.Position = new Vector2(0, 0);
			sprite.Play(action + "Right");
		}
		else if(-animvaxis > animhaxis && -animvaxis > -animhaxis)
		{
			lastDirection = 3;
			//obj_interact.Position = new Vector2(-26, -26);
			sprite.Play(action + "Down");
		}
		else if(animvaxis > animhaxis && animvaxis > -animhaxis)
		{
			lastDirection = 4;
			//obj_interact.Position = new Vector2(-26, 26);
			sprite.Play(action + "Up");
		}
	}

	public void Burn()
	{
		if(controllerIndex != -2)
		{
			Lost = true;
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
		else if(abilities.Contains("jump_balley"))
		{
			GravityScale = 0;
			LinearDamp = 25;
			joyLock = false;
			resetAnim = true;
			// velocity.Y = -jumpHeight;
			jumping = false;
			SetCollisionMaskValue(2, true);
		}
	}

	private void ProcessAI()
	{
		if(Lost)
			return;
		
		switch(((GameManager)GetNode("/root/GameManager")).CurrentMinigame)
		{
			case 1:
				if(joyLock)
					return;
				
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
			case 3:
				if(GetNode<AnimatedSprite2D>("../../obj_fire").Frame == 5 && jumpCountdown <= 0)
				{
					t_startJump.WaitTime = ((rand.NextDouble()/2) * 0.1);
					t_startJump.Start();
				}
				break;
		}

		animhaxis = joyhaxis;
		animvaxis = joyvaxis;
	}

	private void FireJump()
	{
		jumpCountdown = 0.1f;
		jumping = true;
		//sprite.Animation = "jumpUp";
		t_jumpDuration.WaitTime = Math.Clamp((1.4-GetNode<obj_fire>("../../obj_fire").speed) * 0.6f, 0.05, 1);
		//GD.Print(t_jumpDuration.WaitTime);
		t_jumpDuration.Start();
	}

	private void SpriteChanged()
	{
		if(abilities.Contains("shadow"))
		{
			//GD.Print(sprite.Animation + " | " + sprite.Frame);
			shadow.Texture = sprite.SpriteFrames.GetFrameTexture(sprite.Animation, sprite.Frame);
		}

		if(sprite.Animation == "attack")
		{
			switch(sprite.Frame)
			{
				case 3:
					foreach(Area2D area in obj_interact.collisions)
					{
						switch(flip)
						{
							case 1:
								area.GetNode<RigidBody2D>("../").ApplyCentralImpulse(new Vector2(punchForce, 0));
								break;
							case -1:
								area.GetNode<RigidBody2D>("../").ApplyCentralImpulse(new Vector2(-punchForce, 0));
								break;
							// case 3:
							// 	area.GetNode<RigidBody2D>("../").ApplyCentralImpulse(new Vector2(0, -punchForce));
							// 	break;
							// case 4:
							// 	area.GetNode<RigidBody2D>("../").ApplyCentralImpulse(new Vector2(0, punchForce));
							// 	break;
						}

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
		sprite.Animation = "idle";
	}

	public void PlayAnimation(string anim)
	{
		animation.Play(anim);
	}

	public void ResetJoystick(bool anim = true)
	{
		joyhaxis = 0;
		joyvaxis = 0;

		if(anim)
		{
			animhaxis = joyhaxis;
			animvaxis = joyvaxis;
		}
	}
}
