using Godot;
using System;

public partial class obj_character_map : RigidBody2D
{
	public int Player = 0;
	public bool isTurn = false;
	public PlayerData playerData;
	public int CharacterIndex { get { return playerData.characterIndex; } }

	public byte state = 128;
	private int moves = 0;
	private bool starSpace = false;

	public int controllerIndex = 1;
	public bool canJump = false;

	private AnimatedSprite2D sprite;
	private float scale = 3;
	private float idleTimer = 0;
	private PathFollow2D follower;
	private short flip = 1;
	private bool movesCount = false;
	private bool grace = true;

	private Area2D lastCollision;
	private CollisionShape2D collision;

	private Alarm t_moveOnBoard;
	private bool jumping = false;
	private bool canMove = true;

	private float velocityX;
	private float velocityY;
	private Vector2 posOld;
	private double delay;
	private bool locked = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Variant _Player = GetMeta("Player");
		Player = _Player.As<int>() - 1;

		Variant _IsTurn = GetMeta("IsTurn");
		isTurn = _IsTurn.As<bool>();

		playerData = ((GameManager)GetNode("/root/GameManager")).playerData[Player];
		delay = ((GameManager)GetNode("/root/GameManager")).rand.NextDouble() * 2;

		sprite = GetNode<AnimatedSprite2D>("obj_sprite");
		follower = GetNode<PathFollow2D>("../../pf_0" + (Player + 1));
		collision = GetNode<CollisionShape2D>("obj_collision");
		follower.ProgressRatio = playerData.progress;
		sprite.SpriteFrames = playerData.animationFrames;
		
		controllerIndex = playerData.controllerIndex;
		// if(controllerIndex == -1)
		// 	controllerIndex = 1;

		collision.Disabled = true;

		if(playerData.PlayerStarted)
			Position = new Vector2(0, -26);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		ProcessAnimations();
		
		if(!isTurn)
			return;

		idleTimer += (float)delta;

		switch(state)
		{
			case 0:
				BeginTurn();
				break;
			case 1:
				RollDice(delta);
				break;
			case 2:
				DiceRolled();
				break;
			case 3:
				MoveOnBoard((float)delta);
				break;
			case 4:
				HandleSpace();
				break;
			case 5:
				EndTurn();
				break;
		}
	}

	private void BeginTurn()
	{
		GetNode<Node2D>("../../../../obj_diceBlock").Position = new Vector2(follower.Position.X, follower.Position.Y - 138);
		GetNode<obj_diceBlock>("../../../../obj_diceBlock").a_spinStart(this);
		EnableCollision();
		state = 1;
	}

	public void EnableCollision()
	{
		collision.Disabled = false;
	}

	private void RollDice(double delta)
	{
		if(controllerIndex == -1)
			if(canJump)
			{
				delay -= delta;
				if(delay <= 0)
					Jump();
				return;
			}
		
		if(canJump && Input.IsActionJustPressed("jump" + controllerIndex))
			Jump();

		Input.ActionRelease("jump" + controllerIndex);
	}

	private void Jump()
	{
		GravityScale = 1;
		jumping = true;
		ApplyCentralImpulse(new Vector2(0, -750));
		state = 2;
	}

	public void DiceRolled()
	{
		if(playerData.PlayerStarted)
		{
			if(((obj_diceBlock)GetNode<Node2D>("../../../../obj_diceBlock")).numState == 2)
			{
				moves = ((obj_diceBlock)GetNode<Node2D>("../../../../obj_diceBlock")).num + 1;
				GetNode<CollisionShape2D>("spacehitbox/obj_collision").Disabled = false;
				Freeze = true;
				movesCount = true;
				state = 3;
			}
		}
		else
		{
			if(playerData.diceRoll != 0)
			{
				GetNode<obj_shawarma>("../../../../obj_shawarma").diceRoll--;
				state = 6;
			}
		}
	}

	private void MoveOnBoard(float delta)
	{
		if(moves > 0)
		{
			if(!canMove)
				return;
			GetNode<Node2D>("../../../../obj_diceBlock").Position = new Vector2(follower.Position.X, follower.Position.Y - 138);
			follower.ProgressRatio += delta * 0.04f;
		}
		else
		{
			starSpace = false;
			state = 4;
		}
	}

	private void HandleSpace()
	{
		if(locked)
			return;
		//GD.Print(lastCollision.Name);
		byte walletID = (byte)(Player + 1);
		((ShaderMaterial)GetNode<Sprite2D>("../../../../obj_mapGUI/obj_wallet" + walletID + "/spr_wallet/spr_walletColor").Material).SetShaderParameter("walletColor", new Vector3(0.2f, 1, 0.2f));
		if(!locked && !starSpace)
		switch(lastCollision.Name.ToString().Substring(0, 6))
		{
			case "spc_st":
				HandleStar();
				locked = true;
				return;

				break;
			case "spc_bl":
				((GameManager)GetNode("/root/GameManager")).playerData[Player].coins += 3;
				GetNode<obj_coinIndicator>("obj_coinIndicator/Indicator").PlayAnimation(3);
				((ShaderMaterial)GetNode<Sprite2D>("../../../../obj_mapGUI/obj_wallet" + walletID + "/spr_wallet/spr_walletColor").Material).SetShaderParameter("walletColor", new Vector3(0.2f, 0.2f, 1));
				break;
			case "spc_re":
				((GameManager)GetNode("/root/GameManager")).playerData[Player].coins -= 3;
				GetNode<obj_coinIndicator>("obj_coinIndicator/Indicator").PlayAnimation(-3);
				((ShaderMaterial)GetNode<Sprite2D>("../../../../obj_mapGUI/obj_wallet" + walletID + "/spr_wallet/spr_walletColor").Material).SetShaderParameter("walletColor", new Vector3(1f, 0.2f, 0.2f));
				break;
			case "spc_br":
			
				break;
			case "spc_ch":
				
				break;
			case "spc_ev":
				
				break;
			case "spc_it":
				
				break;
			case "spc_ar":
				
				break;
		}

		((GameManager)GetNode("/root/GameManager")).playerData[Player].coins = Math.Clamp(((GameManager)GetNode("/root/GameManager")).playerData[Player].coins, (short)0, (short)9999);
		
		state = 5;
	}

	private void EndTurn()
	{
		if(GetNode<obj_coinIndicator>("obj_coinIndicator/Indicator").AnimationFinished)
			return;
		GetNode<Transition>("../../../../obj_mapGUI/Transition").playerGoing = Player + 1;
		GetNode<Transition>("../../../../obj_mapGUI/Transition").state = 1;
		playerData.progress = follower.ProgressRatio;
		collision.Disabled = true;
		state = 6;
	}

	private void ProcessAnimations()
	{
		velocityX = follower.Position.X - posOld.X;
		velocityY = follower.Position.Y - posOld.Y;

		posOld = follower.Position;

		//GD.Print("VX: " + velocityX + " | VY: " + velocityY);

		if(velocityX == 0 && velocityY == 0)
		{
			sprite.Frame = 0;
		}

		if(velocityX > 0)
			flip = 1;
		else if(velocityX < 0)
			flip = -1;

		if(jumping)
		{
			if((velocityX > velocityY && velocityX > -velocityY) || (-velocityX > velocityY && -velocityX > -velocityY))
			{
				sprite.SpeedScale = Mathf.Clamp(Mathf.Abs(velocityX), 0.2f, 1);
				sprite.Play("jumpRight");
			}
			else if(-velocityY > velocityX && -velocityY > -velocityX)
			{
				sprite.SpeedScale = Mathf.Clamp(Mathf.Abs(velocityY), 0.2f, 1);
				sprite.Play("jumpDown");
			}
			else //if(velocityY > velocityX && velocityY > -velocityX)
			{
				sprite.SpeedScale = Mathf.Clamp(Mathf.Abs(velocityY), 0.2f, 1);
				sprite.Play("jumpUp");
			}
		}
		else
		{
			if((velocityX > velocityY && velocityX > -velocityY) || (-velocityX > velocityY && -velocityX > -velocityY))
			{
				sprite.SpeedScale = Mathf.Clamp(Mathf.Abs(velocityX), 0.2f, 1);
				sprite.Play("walkRight");
			}
			else if(-velocityY > velocityX && -velocityY > -velocityX)
			{
				sprite.SpeedScale = Mathf.Clamp(Mathf.Abs(velocityY), 0.2f, 1);
				sprite.Play("walkDown");
			}
			else //if(velocityY > velocityX && velocityY > -velocityX)
			{
				sprite.SpeedScale = Mathf.Clamp(Mathf.Abs(velocityY), 0.2f, 1);
				sprite.Play("walkUp");
			}
		}

		sprite.Scale = new Vector2(scale * flip, scale * 1);
	}

	private void SpaceEnter(Area2D area)
	{
		if(area.Name == "obj_character_map" || !movesCount)
			return;

		if(grace)
		{
			grace = false;
			return;
		}

		moves--;
		GetNode<obj_diceBlock>("../../../../obj_diceBlock").num--;
		if(canMove)
			lastCollision = area;

		if(area.Name == "spc_star")
		{
			lastCollision = area;
			HandleStar();
		}
	}

	private void HandleStar()
	{
		((obj_map)GetNode<Node2D>("../../../../")).SetZoomLevel(1.5f);
		((obj_map)GetNode<Node2D>("../../../../")).offset = new Vector2(0, -64);
		GetNode<spc_star>("../../../../spc_star").ShowDialogue(playerData, new Callable(this, "Unlock"));
		canMove = false;
		starSpace = true;
	}

	public void Unlock(bool gotStar = false)
	{
		locked = false;
		canMove = true;
		((obj_map)GetNode<Node2D>("../../../../")).SetZoomLevel(1f);
		((obj_map)GetNode<Node2D>("../../../../")).offset = new Vector2(0, 0);
		GetNode<spc_star>("../../../../spc_star").HideDialogue();
	}

	private new void BodyShapeEntered(Rid body_rid, Node body, long body_shape_index, long local_shape_index)
	{
		if(body.Name == "obj_pfloor")
			jumping = false;
	}
}
