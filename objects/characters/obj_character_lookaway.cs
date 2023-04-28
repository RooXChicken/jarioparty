using Godot;
using System;
using System.Collections.Generic;

public partial class obj_character_lookaway : Node2D
{
	public int Player = 0;
	private PlayerData playerData;
	private List<string> abilities;
	public int CharacterIndex {get {return playerData.characterIndex; }}

	public int controllerIndex = 1;
	private int facing;
	private AnimatedSprite2D sprite;

	private float joyhaxis = 0;
	private float joyvaxis = 0;

	private bool cooldown = false;
	private Alarm t_cooldown;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("spr_face");
		playerData = ((GameManager)GetNode("/root/GameManager")).playerData[Player];
		abilities = ((GameManager)GetNode("/root/GameManager")).minigameLookup[((GameManager)GetNode("/root/GameManager")).CurrentMinigame].Abilities;
		sprite.SpriteFrames = playerData.animationFrames;
		
		controllerIndex = playerData.controllerIndex;
		t_cooldown = new Alarm(0.5, true, this, new Callable(this, "ResetCooldown"), false);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetControllerInput();
		ProcessAnimations();

		sprite.Frame = facing;
	}

	private void GetControllerInput()
	{
		if(controllerIndex == -1)
			return;

		if(abilities.Contains("move"))
		{
			joyhaxis = Input.GetAxis("left" + controllerIndex, "right" + controllerIndex);
			joyvaxis = Input.GetAxis("up" + controllerIndex, "down" + controllerIndex);
		}
	}

	private void ProcessAnimations()
	{
		if(cooldown)
			return;
		
		if(-joyhaxis > joyvaxis && -joyhaxis > -joyvaxis && facing != 1)
		{
			facing = 1;
			t_cooldown.Start();
			cooldown = true;
		}
		if(joyhaxis > joyvaxis && joyhaxis > -joyvaxis && facing != 2)
		{
			facing = 2;
			t_cooldown.Start();
			cooldown = true;
		}
		else if(-joyvaxis > joyhaxis && -joyvaxis > -joyhaxis && facing != 4)
		{
			facing = 4;
			t_cooldown.Start();
			cooldown = true;
		}
		else if(joyvaxis > joyhaxis && joyvaxis > -joyhaxis && facing != 3)
		{
			facing = 3;
			t_cooldown.Start();
			cooldown = true;
		}
	}

	public void ResetCooldown()
	{
		cooldown = false;
		if(joyhaxis == 0 && joyvaxis == 0 && facing != 0)
		{
			facing = 0;
			t_cooldown.Start();
			cooldown = true;
		}
	}
}
