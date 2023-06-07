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
	private bool setPath = false;
	private bool pathReset = false;

	private Area2D lastCollision;
	private CollisionShape2D collision;
	private Node2D obj_beginTurn;
	private Node2D obj_itemPicker;

	private Alarm t_moveOnBoard;
	private bool jumping = false;
	public bool canMove = true;

	private obj_arrow obj_arrowRD;
	private obj_arrow obj_arrowLD;

	private int itemIndex = 1;
	private bool itemIndexMoved = false;
	private float[] randomBagPosX;
	private float[] randomBagPosY;

	private Vector2[] itemSelectPos2;
	private Vector2[] itemSelectPos3;

	private float joyhaxis;
	private float joyhaxisOld;
	private float velocityX;
	private float velocityY;
	private Vector2 posOld;
	private double delay;
	private bool locked = false;

	private double animDelay = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Variant _Player = GetMeta("Player");
		Player = _Player.As<int>() - 1;

		Variant _IsTurn = GetMeta("IsTurn");
		isTurn = _IsTurn.As<bool>();

		playerData = ((GameManager)GetNode("/root/GameManager")).playerData[Player];
		delay = ((GameManager)GetNode("/root/GameManager")).rand.NextDouble() * 2;
		obj_beginTurn = GetNode<Node2D>("obj_beginTurn");
		obj_itemPicker = GetNode<Node2D>("obj_itemPicker");

		sprite = GetNode<AnimatedSprite2D>("obj_sprite");
		follower = GetNode<PathFollow2D>("../../pf_0" + (Player + 1));
		collision = GetNode<CollisionShape2D>("obj_collision");

		obj_arrowRD = GetNode<obj_arrow>("obj_arrowRD");
		obj_arrowLD = GetNode<obj_arrow>("obj_arrowLD");

		itemSelectPos2 = new Vector2[] {
			new Vector2(-20, -20), new Vector2(20, -20)
		};

		itemSelectPos3 = new Vector2[] {
			new Vector2(-36, -20), new Vector2(0, -20), new Vector2(36, -20)
		};

		randomBagPosX = new float[6];
		randomBagPosY = new float[6];

		for(int i = 0; i < 6; i++)
		{
			randomBagPosX[i] = (i - 3) * 2 + obj_itemPicker.Position.X;
			randomBagPosY[i] = (i - 3) * 2 + obj_itemPicker.Position.Y;
		}

		follower.Progress = playerData.progress;
		sprite.SpriteFrames = playerData.animationFrames;
		
		controllerIndex = playerData.controllerIndex;
		
		obj_arrowRD.controllerIndex = controllerIndex;
		obj_arrowLD.controllerIndex = controllerIndex;

		obj_arrowRD.ConfirmChoice(playerData.pathID, playerData.progress);

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
		animDelay -= delta;

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
			case 9:
				TurnMenu(delta);
				break;
			case 10:
				OpenItemMenu();
				break;
			case 11:
				HandleItemMenu();
				break;
			case 12:
				ExitZoom();
				break;
			case 13:
				ItemsMove(delta);
				break;
		}
	}

	private void BeginTurn()
	{
		GetNode<Node2D>("../../../../obj_diceBlock").Position = new Vector2(follower.Position.X, follower.Position.Y - 138);
		ZIndex++;

		obj_beginTurn.Visible = true;

		if(playerData.items.Count <= 0)
			obj_beginTurn.GetNode<Sprite2D>("ItemDisable").Visible = true;
		state = 9;
	}

	private void BeginDiceSpin()
	{
		GetNode<obj_diceBlock>("../../../../obj_diceBlock").a_spinStart(this);
		EnableCollision();
	}

	private void OpenItemMenu()
	{
		if(animDelay <= 0)
		{
			animDelay = 0.04;
			obj_itemPicker.Position = new Vector2(randomBagPosX[((GameManager)GetNode("/root/GameManager")).rand.Next(0,5)], randomBagPosY[((GameManager)GetNode("/root/GameManager")).rand.Next(0,5)]);
		}
		if(!obj_itemPicker.GetNode<AnimationPlayer>("anim_itemPicker").IsPlaying())
		{
			obj_itemPicker.GetNode<AnimationPlayer>("anim_itemPicker").Play("item" + playerData.items.Count);
			for(int i = 0; i < playerData.items.Count; i++)
				obj_itemPicker.GetNode<Sprite2D>("Items/spr_item" + (i+1)).Texture = playerData.items[i].Texture;
			state = 11;
		}
	}

	private void HandleItemMenu()
	{
		joyhaxis = Input.GetAxis("left" + controllerIndex, "right" + controllerIndex);

		if(!itemIndexMoved)
		{
			itemIndexMoved = true;

			if(joyhaxis > 0)
			{
				((AudioController)GetNode("/root/AudioController")).PlaySound("gui_selectionMove");
				itemIndex++;
			}
			else if(joyhaxis < 0)
			{
				((AudioController)GetNode("/root/AudioController")).PlaySound("gui_selectionMove");
				itemIndex--;
			}
		}

		if(joyhaxis == 0)
			itemIndexMoved = false;

		if(itemIndex > playerData.items.Count-1)
			itemIndex = 0;
		else if(itemIndex < 0)
			itemIndex = playerData.items.Count-1;

		if(playerData.items.Count == 1)
			itemIndex = 1;

		if(playerData.items.Count == 1 || playerData.items.Count == 3)
		{
			obj_itemPicker.GetNode<Sprite2D>("Items/spr_itemSelect").Position = itemSelectPos3[itemIndex];
		}
		else
		{
			obj_itemPicker.GetNode<Sprite2D>("Items/spr_itemSelect").Position = itemSelectPos2[itemIndex];
		}

		if(Input.IsActionJustPressed("jump" + controllerIndex))
		{
			state = 13;
			((AudioController)GetNode("/root/AudioController")).PlaySound("gui_select");
			obj_itemPicker.GetNode<Sprite2D>("Items/spr_item1").Visible = false;
			obj_itemPicker.GetNode<Sprite2D>("Items/spr_item2").Visible = false;
			obj_itemPicker.GetNode<Sprite2D>("Items/spr_item3").Visible = false;

			obj_itemPicker.GetNode<Sprite2D>("Items/spr_item" + (itemIndex+1)).Visible = true;
			GD.Print(playerData.items[itemIndex].ItemIndex);
			playerData.items[itemIndex].ItemUseMap(playerData);
		}
	}

	private void ItemsMove(double delta)
	{
		obj_itemPicker.GetNode<Sprite2D>("Items/spr_item" + (itemIndex+1)).Position = obj_itemPicker.GetNode<Sprite2D>("Items/spr_item" + (itemIndex+1)).Position.Lerp(itemSelectPos3[1], (float)delta*5);
		if(obj_itemPicker.GetNode<Sprite2D>("Items/spr_item" + (itemIndex+1)).Position.X < 1 && obj_itemPicker.GetNode<Sprite2D>("Items/spr_item" + (itemIndex+1)).Position.X > -1)
		{
			obj_itemPicker.Visible = false;
			obj_beginTurn.Visible = true;
			state = 9;
		}
	}

	public void EnableCollision()
	{
		collision.Disabled = false;
	}

	private void TurnMenu(double delta)
	{
		if(controllerIndex == -1)
		{
			delay -= delta;
			if(delay <= 0)
			{
				state = 1;
				BeginDiceSpin();

				obj_beginTurn.Visible = false;
			}
			return;
		}
		if(Input.IsActionJustPressed("jump" + controllerIndex))
		{
			state = 1;
			BeginDiceSpin();

			obj_beginTurn.Visible = false;
		}
		else if(!obj_beginTurn.GetNode<Sprite2D>("ItemDisable").Visible && Input.IsActionJustPressed("punch" + controllerIndex))
		{
			if(playerData.items.Count <= 0)
				return;
			state = 10;
			obj_itemPicker.Visible = true;
			obj_itemPicker.GetNode<AnimationPlayer>("anim_itemPicker").Play("bagOpening");
			obj_beginTurn.GetNode<Sprite2D>("ItemDisable").Visible = true;

			obj_beginTurn.Visible = false;
		}
		else if(Input.IsActionJustPressed("viewMap" + controllerIndex))
		{
			state = 12;
			obj_beginTurn.Visible = false;
			((AudioController)GetNode("/root/AudioController")).MusicEffect("volume", -8);
			GetNode<CanvasLayer>("ExitZoomLayer").Visible = true;
			GetNode<obj_map>("../../../../").SetZoomLevel(0.35f);
			GetNode<obj_map>("../../../../").SetPosition(new Vector2(-350, 0));
		}
	}

	private void ExitZoom()
	{
		if(Input.IsActionJustPressed("viewMap" + controllerIndex))
		{
			obj_beginTurn.Visible = true;
			GetNode<CanvasLayer>("ExitZoomLayer").Visible = false;
			state = 9;
			GetNode<obj_map>("../../../../").SetZoomLevel(1);
			GetNode<obj_map>("../../../../").SetPosition();

			((AudioController)GetNode("/root/AudioController")).MusicEffect("volume", 0);
		}
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
				moves = ((obj_diceBlock)GetNode<Node2D>("../../../../obj_diceBlock")).num;
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
			//follower.ProgressRatio += delta * 0.04f;
			follower.Progress += 332 * delta;
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
		playerData.spaceColor = -1;
		byte walletID = (byte)(Player + 1);
		((ShaderMaterial)GetNode<Sprite2D>("../../../../obj_mapGUI/obj_wallet" + walletID + "/spr_wallet/spr_walletColor").Material).SetShaderParameter("walletColor", new Vector3(0.2f, 1, 0.2f));
		if(!locked && !starSpace)
		switch(lastCollision.Name.ToString().Substring(0, 6))
		{
			case "spc_st":
				HandleStar();
				locked = true;
				return;
			case "spc_bl":
				playerData.spaceColor = 0;
				((GameManager)GetNode("/root/GameManager")).playerData[Player].coins += 3;
				GetNode<obj_coinIndicator>("obj_coinIndicator/Indicator").PlayAnimation(3);
				((ShaderMaterial)GetNode<Sprite2D>("../../../../obj_mapGUI/obj_wallet" + walletID + "/spr_wallet/spr_walletColor").Material).SetShaderParameter("walletColor", new Vector3(0.2f, 0.2f, 1));
				break;
			case "spc_re":
				playerData.spaceColor = 1;
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
		playerData.progress = follower.Progress;
		collision.Disabled = true;
		state = 6;
		ZIndex--;
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

		if(area.Name.ToString().Substring(0, 6) == "spc_pa")
		{
			if(!pathReset)
			{
				pathReset = true;
				if(playerData.pathID == 1)
					return;
				
				playerData.pathID = 1;
				moves++;
				GetNode<obj_diceBlock>("../../../../obj_diceBlock").num++;
				obj_arrowRD.ConfirmChoice(1, area.GetMeta("Progress").As<float>());
			}
			return;
		}

		if(area.Name != "spc_star" && area.Name.ToString().Substring(0, 6) != "spc_ar")
		{
			moves += GetNode<obj_diceBlock>("../../../../obj_diceBlock").DecrementDice();
			if(canMove)
				lastCollision = area;
		}

		if(area.Name.ToString().Substring(0, 6) == "spc_ar" && lastCollision != area)
		{
			canMove = false;
			lastCollision = area;
			ArrowMenu(area.Name);
		}

		if(area.Name == "spc_star")
		{
			lastCollision = area;
			HandleStar();
		}
	}

	private void ArrowMenu(string name)
	{
		switch(name)
		{
			case "spc_arrowRD":
				obj_arrowRD.Visible = true;
				state = 8;
				break;
			case "spc_arrowLD":
				obj_arrowLD.Visible = true;
				state = 8;
				break;
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
