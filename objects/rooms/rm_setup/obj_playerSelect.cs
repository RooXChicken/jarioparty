using Godot;
using System;

public partial class obj_playerSelect : Node2D
{
	private short index = 0;
	private short state = 0; //0 = clouds; 1 = player select; 2 = map select; 3 = time select
	private short characterIndex = 0;
	private short controllerIndex = 0;
	private short[] controllerIndecies = new short[4] {1, 1, 1, 1};
	private bool realPlayer = true;
	private short controllersConnected = 0;
	private float joyhaxis = 0;
	private float joyvaxis = 0;
	private bool moving = false;

	private AnimatedSprite2D spr_hand;
	private Sprite2D[] clouds;
	private Node2D obj_clock;
	private AnimatedSprite2D[] players;
	private AnimationPlayer anim_transition;
	private Sprite2D[] portraits;
	private Color regular = new Color(1, 1, 1, 1);
	private Color selected = new Color(0.5f, 0.5f, 0.5f, 1);
	private Vector2[] positions = new Vector2[4];

	private AnimationPlayer animation;
	private bool ready = false;
	private bool locked = false;
	private bool isBackwards = false;

	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/rm_setup/snd_frog_scream.wav", "frogscream");
		animation = GetNode<AnimationPlayer>("anim_player");
		spr_hand = GetNode<AnimatedSprite2D>("spr_hand");
		anim_transition = GetNode<AnimationPlayer>("../anim_transition");
		obj_clock = GetNode<Node2D>("../obj_clock");
		Visible = false;

		spr_hand.Play("default");

		clouds = new Sprite2D[] { 
			GetNode<Sprite2D>("Clouds/spr_cloudp1"), GetNode<Sprite2D>("Clouds/spr_cloudp2"), GetNode<Sprite2D>("Clouds/spr_cloudp3"), GetNode<Sprite2D>("Clouds/spr_cloudp4") 
			};

		players = new AnimatedSprite2D[] { 
			GetNode<AnimatedSprite2D>("Characters/spr_character_jario"), GetNode<AnimatedSprite2D>("Characters/spr_character_wooigi"), GetNode<AnimatedSprite2D>("Characters/spr_character_grape"), GetNode<AnimatedSprite2D>("Characters/spr_character_josh") 
			};

		portraits = new Sprite2D[] { 
			GetNode<Sprite2D>("Portraits/spr_portrait_jario"), GetNode<Sprite2D>("Portraits/spr_portrait_wooigi"), GetNode<Sprite2D>("Portraits/spr_portrait_grape"), GetNode<Sprite2D>("Portraits/spr_portrait_josh") 
			};

		for(int i = 0; i < 4; i++)
		{
			positions[i] = new Vector2(players[i].Position.X, -1);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!Visible || locked)
			return;

		if(!ready)
		{
			if(!animation.IsPlaying())
			{
				for(int i = 0; i < 4; i++)
					clouds[i].Modulate = selected;
				for(int i = 0; i < ((GameManager)GetNode("/root/GameManager")).controllersConnected; i++)
					clouds[i].Modulate = regular;
				animation.Play("playerSelect");
				GetNode<Node2D>("../obj_buttonStick").Visible = true;
			}
		}

		GetControllerInput();
		if(joyhaxis > 0 && !moving)
		{
			ChangeIndex(1);
			moving = true;
		}
		if(joyhaxis < 0 && !moving)
		{
			ChangeIndex(-1);
			moving = true;
		}

		switch(state)
		{
			case 0:
				PlayerCountSelect();
				break;
			case 1:
				CharacterSelect(delta);
				break;
			case 3:
				TimeSelect();
				break;
			case 4:
				FinalStage();
				break;
		}
	}

	private void PlayerCountSelect()
	{
		for(int i = 0; i < 4; i++)
			clouds[i].Modulate = selected;
		for(int i = 0; i < ((GameManager)GetNode("/root/GameManager")).controllersConnected; i++)
			clouds[i].Modulate = regular;

		if(Input.IsActionJustPressed("back" + controllerIndecies[controllerIndex]))
		{
			animation.PlayBackwards("playerSelect");
			foreach(Sprite2D sprite in portraits)
				sprite.Visible = false;
			GetNode<Sprite2D>("Portraits/spr_frame").Visible = false;
			((AudioController)GetNode("/root/AudioController")).PlaySound("gui_back");
			SetFrogText((short)(1));
			isBackwards = true;
			locked = true;
			GetNode<Node2D>("../obj_buttonStick").Visible = false;
		}
		else if(Input.IsActionJustPressed("jump" + controllerIndecies[controllerIndex]))
		{
			((GameManager)GetNode("/root/GameManager")).playerCount = (short)(index + 1);
			state = 1;
			index = 0;
			characterIndex = 0;
			((AudioController)GetNode("/root/AudioController")).PlaySound("gui_select");
			GetNode<Node2D>("Portraits").Visible = false;
			animation.Play("cloudtoport");

			foreach(Sprite2D sprite in portraits)
				sprite.Visible = true;
			GetNode<Node2D>("Portraits/spr_frame").Visible = true; //weird
			//GetNode<Sprite2D>("Portraits/spr_frame");
			SetFrogText((short)(characterIndex + 6));
			ChangeIndex(0);

			controllerIndecies = new short[4] {1, 1, 1, 1};

			for(int i = 0; i < ((GameManager)GetNode("/root/GameManager")).controllersConnected; i++)
				controllerIndecies[i] = (short)(i+1);
		}
	}

	private void CharacterSelect(double delta)
	{
		for(int i = 0; i < 4; i++)
			portraits[i].Frame = 0;
			
		portraits[index].Frame = 1;

		if(Input.IsActionJustPressed("back" + controllerIndecies[controllerIndex]))
		{
			((AudioController)GetNode("/root/AudioController")).PlaySound("gui_back");
			characterIndex--;
			if(characterIndex < 0)
			{
				state = 0;
				for(int i = 0; i < 4; i++)
					animation.PlayBackwards("cloudtoport");
				//GetNode<Sprite2D>("Portraits/spr_frame");
				SetFrogText((short)(characterIndex + 6));
				ChangeIndex(0);

				foreach(Sprite2D sprite in portraits)
					sprite.Frame = 0;
			}
			else
			{
				players[((GameManager)GetNode("/root/GameManager")).playerData[characterIndex].characterIndex].Visible = false;
				portraits[((GameManager)GetNode("/root/GameManager")).playerData[characterIndex].characterIndex].Visible = true;
				SetFrogText((short)(characterIndex + 6));
				//((GameManager)GetNode("/root/GameManager")).playerData[characterIndex] = new PlayerData(-1, (ushort)index, (characterIndex >= ((GameManager)GetNode("/root/GameManager")).playerCount));
			}
		}
		else if(Input.IsActionJustPressed("jump" + controllerIndecies[controllerIndex]))
		if(!players[index].Visible)
		{
			((AudioController)GetNode("/root/AudioController")).PlaySound("gui_select");
			((GameManager)GetNode("/root/GameManager")).playerData[characterIndex] = new PlayerData(-1, (ushort)index, (characterIndex >= ((GameManager)GetNode("/root/GameManager")).playerCount));
			
			if(realPlayer)
				((GameManager)GetNode("/root/GameManager")).playerData[characterIndex].controllerIndex = controllerIndecies[controllerIndex];
		
			SetCharacterIndex();

			if(characterIndex == 4)
			{
				//GetNode<AnimatedSprite2D>("/root/rm_game/LoadingScreen").Visible = true;
				animation.Play("porttoclock");
				obj_clock.Visible = true;
				SetFrogText((short)(10));
				spr_hand.Visible = false;
				state = 3;
				GetNode<Node2D>("../obj_buttonDpad").Visible = true;
			}
		}
		else
			((AudioController)GetNode("/root/AudioController")).PlaySound("gui_badSelectionMove");
	}

	private void SetCharacterIndex()
	{
		portraits[index].Visible = false;
		players[index].Play("dance");
		players[index].Visible = true;

		// ((GameManager)GetNode("/root/GameManager")).playerData[characterIndex].items.Add(((GameManager)GetNode("/root/GameManager")).itemLookup[0]);
		// ((GameManager)GetNode("/root/GameManager")).playerData[characterIndex].items.Add(((GameManager)GetNode("/root/GameManager")).itemLookup[1]);
		// ((GameManager)GetNode("/root/GameManager")).playerData[characterIndex].items.Add(((GameManager)GetNode("/root/GameManager")).itemLookup[3]);

		((GameManager)GetNode("/root/GameManager")).playerData[characterIndex].playerOrder = characterIndex + 1;

		if(Input.IsActionPressed("costume" + controllerIndecies[controllerIndex]))
		{
			((GameManager)GetNode("/root/GameManager")).playerData[characterIndex].Costume = true;
			players[index].Play("danceC");
		}

		controllerIndex++;
		characterIndex++;

		SetFrogText((short)(characterIndex + 6));
	}

	private void TimeSelect()
	{
		int turns = ((obj_clock)obj_clock).Turns;
		if(turns != -1)
		{
			if(turns <= 2)
				SetFrogText((short)(11));
			else if(turns <= 10)
				SetFrogText((short)(12));
			else if(turns <= 40)
				SetFrogText((short)(13));
			else if(turns <= 55)
				SetFrogText((short)(14));
			else
				SetFrogText((short)(15));
		}
		if(Input.IsActionJustPressed("back" + 1))
		{
			animation.PlayBackwards("porttoclock");
			obj_clock.Visible = false;
			SetFrogText((short)(9));
			((AudioController)GetNode("/root/AudioController")).PlaySound("gui_back");
			characterIndex--;
			players[((GameManager)GetNode("/root/GameManager")).playerData[characterIndex].characterIndex].Visible = false;
			portraits[((GameManager)GetNode("/root/GameManager")).playerData[characterIndex].characterIndex].Visible = true;
			SetFrogText((short)(characterIndex + 6));
			//((GameManager)GetNode("/root/GameManager")).playerData[characterIndex] = new PlayerData(-1, (ushort)index, (characterIndex >= ((GameManager)GetNode("/root/GameManager")).playerCount));
			spr_hand.Visible = true;
			GetNode<Node2D>("../obj_buttonDpad").Visible = false;
			state = 1;
		}
		else if(Input.IsActionJustPressed("jump" + 1))
		{
			if(turns == -1)
				turns = 20;
			
			((GameManager)GetNode("/root/GameManager")).TurnsMax = turns;
			SetFrogText((short)(16));
			((obj_clock)obj_clock).Lock = true;
			((AudioController)GetNode("/root/AudioController")).PlaySound("gui_select");
			state = 4;
		}

		//((GameManager)GetNode("/root/GameManager")).SwitchScene("rm_map");
		//((GameManager)GetNode("/root/GameManager")).SwitchScene("rm_minigame_info");
	}

	private void FinalStage()
	{
		if(Input.IsActionJustPressed("back1"))
		{
			state = 3;
			((obj_clock)obj_clock).Lock = false;
			((AudioController)GetNode("/root/AudioController")).PlaySound("gui_back");
		}
		if(Input.IsActionJustPressed("jump" + 1))
			LoadGame();
	}

	private void LoadGame()
	{
		for(int i = 0; i < 4; i++)
		{
			((GameManager)GetNode("/root/GameManager")).playerData[i].Load();
		}

		((AudioController)GetNode("/root/AudioController")).PlaySound("frogscream");
		anim_transition.Play("transition");
		state = 4;
	}

	private void ChangeIndex(short val)
	{
		index += val;

		switch(state)
		{
			case 1:
				if(index < 0)
					index = 3;
				if(index > 3)
					index = 0;
				break;
			
			default:
				if(index < 0)
					index = (short)(((GameManager)GetNode("/root/GameManager")).controllersConnected - 1);
				if(index > (short)(((GameManager)GetNode("/root/GameManager")).controllersConnected - 1))
					index = 0;

				SetFrogText((short)(index + 2 + (4 * state)));
					break;
		}

		if(state == 0)
			if((short)(((GameManager)GetNode("/root/GameManager")).controllersConnected - 1) == 0)
				((AudioController)GetNode("/root/AudioController")).PlaySound("gui_badSelectionMove");
			else
				((AudioController)GetNode("/root/AudioController")).PlaySound("gui_selectionMove");
				
		else if(state < 3)
			((AudioController)GetNode("/root/AudioController")).PlaySound("gui_selectionMove");

		switch(index)
		{
			case 0:
				spr_hand.Position = new Vector2(0, -76);
				break;
			case 1:
				spr_hand.Position = new Vector2(102, -76);
				break;
			case 2:
				spr_hand.Position = new Vector2(204, -76);
				break;
			case 3:
				spr_hand.Position = new Vector2(306, -76);
				break;
		}
	}

	private void SetFrogText(short _index)
	{
		((obj_frog)GetNode<AnimatedSprite2D>("../obj_frog")).dialogueIndex = _index;
		((obj_frog)GetNode<AnimatedSprite2D>("../obj_frog")).UpdateTextbox();
	}

	private void GetControllerInput()
	{
		((GameManager)GetNode("/root/GameManager")).controllersConnected = Input.GetConnectedJoypads().Count;
		//GD.Print(((GameManager)GetNode("/root/GameManager")).controllersConnected);

		if(joyhaxis == 0)
			moving = false;

		controllerIndex = characterIndex;
		if(controllerIndex < 0)
			controllerIndex = 0;

		if(controllerIndex >= ((GameManager)GetNode("/root/GameManager")).playerCount)
		{
			controllerIndex = 0;
			realPlayer = false;
		}

		joyhaxis = Input.GetAxis("left" + controllerIndecies[controllerIndex], "right" + controllerIndecies[controllerIndex]);
		joyvaxis = Input.GetAxis("up" + controllerIndecies[controllerIndex], "down" + controllerIndecies[controllerIndex]);
	}
	
	private void OnAnimationFinished(StringName anim_name)
	{
		ready = true;
		if(isBackwards)
		{
			isBackwards = false;
			Visible = false;
			((obj_frog)GetNode<AnimatedSprite2D>("../obj_frog")).locked = false;
			ready = false;
			locked = false;
		}

	}

	private void FrogTransition(StringName anim_name)
	{
		((GameManager)GetNode("/root/GameManager")).SwitchScene("rm_map");
	}
}
