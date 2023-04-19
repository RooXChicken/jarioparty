using Godot;
using System;

public partial class obj_playerSelect : Node2D
{
	private short index = 0;
	private short state = 0; //0 = clouds; 1 = player select; 2 = map select; 3 = time select
	private short characterIndex = 0;
	private short controllerIndex = 1;
	private bool realPlayer = true;
	private short controllersConnected = 0;
	private float joyhaxis = 0;
	private float joyvaxis = 0;
	private bool moving = false;

	private AnimatedSprite2D spr_hand;
	private Sprite2D[] clouds;
	private AnimatedSprite2D[] players;
	private Color regular = new Color(1, 1, 1, 1);
	private Color selected = new Color(0.5f, 0.5f, 0.5f, 1);
	private Vector2[] positions = new Vector2[4];

	public override void _Ready()
	{
		spr_hand = GetNode<AnimatedSprite2D>("spr_hand");
		Visible = false;

		spr_hand.Play("default");

		clouds = new Sprite2D[] { 
			GetNode<Sprite2D>("spr_cloudp1"), GetNode<Sprite2D>("spr_cloudp2"), GetNode<Sprite2D>("spr_cloudp3"), GetNode<Sprite2D>("spr_cloudp4") 
			};

		players = new AnimatedSprite2D[] { 
			GetNode<AnimatedSprite2D>("spr_character_jario"), GetNode<AnimatedSprite2D>("spr_character_wooigi"), GetNode<AnimatedSprite2D>("spr_character_grape"), GetNode<AnimatedSprite2D>("spr_character_josh") 
			};

		for(int i = 0; i < 4; i++)
		{
			positions[i] = new Vector2(players[i].Position.X, -1);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!Visible)
			return;
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
				MapSelect();
				break;
		}
	}

	private void PlayerCountSelect()
	{
		for(int i = 0; i < 4; i++)
			clouds[i].Modulate = selected;
		for(int i = 0; i < ((GameManager)GetNode("/root/GameManager")).controllersConnected; i++)
			clouds[i].Modulate = regular;

		if(Input.IsActionJustPressed("jump" + controllerIndex))
		{
			((GameManager)GetNode("/root/GameManager")).playerCount = (short)(index + 1);
			GD.Print(index);
			state = 1;
			index = 0;
			players[0].Animation = "dance";
			players[0].Play();
			((AudioController)GetNode("/root/AudioController")).PlaySound("gui_select");
			for(int i = 0; i < 4; i++)
			{
				clouds[i].Visible = false;
				players[i].Visible = true;
			}
			SetFrogText((short)(characterIndex + 6));
			ChangeIndex(0);
		}
	}

	private void CharacterSelect(double delta)
	{
		for(int i = 0; i < 4; i++)
		{
			if(220 + players[i].Position.Y < 210)
				players[i].Position = players[i].Position.Lerp(positions[i], (float)(delta * 2));
			else
				players[i].Position = players[i].Position.Lerp(positions[i], (float)(delta * (((GameManager)GetNode("/root/GameManager")).rand.NextDouble())));
			if(i != index && players[i].Modulate != selected)
				players[i].Animation = "idle";
		}
		
		if(players[index].Animation != "dance")
		{
			
			players[index].Animation = "dance";
			players[index].Play();
		}

		if(Input.IsActionJustPressed("jump" + controllerIndex) && players[index].Modulate != selected)
		{
			((AudioController)GetNode("/root/AudioController")).PlaySound("gui_select");
			((GameManager)GetNode("/root/GameManager")).playerData[characterIndex] = new PlayerData(-1, (ushort)index, (characterIndex >= ((GameManager)GetNode("/root/GameManager")).playerCount));
			
			if(realPlayer)
				((GameManager)GetNode("/root/GameManager")).playerData[characterIndex].controllerIndex = controllerIndex;
			
			players[index].Modulate = selected;
		

			((GameManager)GetNode("/root/GameManager")).playerData[characterIndex].items.Add(new ItemBase(((GameManager)GetNode("/root/GameManager")).itemLookup[0]));
			((GameManager)GetNode("/root/GameManager")).playerData[characterIndex].items.Add(new ItemBase(((GameManager)GetNode("/root/GameManager")).itemLookup[1]));
			((GameManager)GetNode("/root/GameManager")).playerData[characterIndex].items.Add(new ItemBase(((GameManager)GetNode("/root/GameManager")).itemLookup[2]));

			controllerIndex++;
			characterIndex++;

			SetFrogText((short)(characterIndex + 6));

			if(characterIndex == 4)
			{
				//GetNode<AnimatedSprite2D>("/root/rm_game/LoadingScreen").Visible = true;
				MapSelect();
			}
		}
	}

	private void MapSelect()
	{
		for(int i = 0; i < 4; i++)
		{
			((GameManager)GetNode("/root/GameManager")).playerData[i].Load();
			GD.Print(i + " loaded");
		}

		((GameManager)GetNode("/root/GameManager")).SwitchScene("rm_map");
		//((GameManager)GetNode("/root/GameManager")).SwitchScene("rm_minigame_info");
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

		((AudioController)GetNode("/root/AudioController")).PlaySound("gui_selectionMove");

		switch(index)
		{
			case 0:
				spr_hand.Position = new Vector2(0, -76);
				break;
			case 1:
				spr_hand.Position = new Vector2(100, -76);
				break;
			case 2:
				spr_hand.Position = new Vector2(200, -76);
				break;
			case 3:
				spr_hand.Position = new Vector2(300, -76);
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
		//((GameManager)GetNode("/root/GameManager")).controllersConnected = 4;
		if(joyhaxis == 0)
			moving = false;

		if(characterIndex >= ((GameManager)GetNode("/root/GameManager")).playerCount)
		{
			controllerIndex = 1;
			realPlayer = false;
		}

		joyhaxis = Input.GetAxis("left" + controllerIndex, "right" + controllerIndex);
		joyvaxis = Input.GetAxis("up" + controllerIndex, "down" + controllerIndex);
	}
}
