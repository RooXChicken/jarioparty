using Godot;
using System;
using System.Collections.Generic;

public partial class obj_character_lookaway : Node2D
{
	public int Player = 0;
	public PlayerData playerData;
	private List<string> abilities;
	public int CharacterIndex {get {return playerData.characterIndex; }}
	public bool Lost = false;

	public int controllerIndex = 1;
	private int facing;
	public int DirectionFacing {get {return facing;}}
	private AnimationPlayer anim_face;
	private AnimatedSprite2D sprite;

	private float joyhaxis = 0;
	private float joyvaxis = 0;
	public bool Locked = false;
	public bool Out = false;

	private double timerReset = 0.5;
	private double timer = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Variant _Player = GetMeta("Player");
		Player = _Player.As<int>() - 1;

		anim_face = GetNode<AnimationPlayer>("anim_face");
		sprite = GetNode<AnimatedSprite2D>("spr_face");
		playerData = ((GameManager)GetNode("/root/GameManager")).playerData[Player];
		abilities = ((GameManager)GetNode("/root/GameManager")).minigameLookup[((GameManager)GetNode("/root/GameManager")).CurrentMinigame].Abilities;
		sprite.SpriteFrames = playerData.animationFrames;
		
		controllerIndex = playerData.controllerIndex;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!((GameManager)GetNode("/root/GameManager")).MinigameStarted)
		{
			joyhaxis = 0;
			joyvaxis = 0;
			return;
		}

		GetControllerInput();
		ProcessAnimations();

		sprite.Frame = facing;
		timer -= delta;
	}

	public void StartAnimation()
	{
		anim_face.Play("rotation");
	}

	private void GetControllerInput()
	{
		if(controllerIndex == -1)
		{
			return;
		}

		if(abilities.Contains("move"))
		{
			joyhaxis = Input.GetAxis("left" + controllerIndex, "right" + controllerIndex);
			joyvaxis = Input.GetAxis("up" + controllerIndex, "down" + controllerIndex);
		}
	}

	public void Shrink()
	{
		if(!Out)
		{
			Out = true;
			anim_face.Play("shrink");
		}
	}

	private void ProcessAnimations()
	{
		if(timer > 0 || Locked || Out)
			return;

		if(controllerIndex == -1)
		{
			ProcessAI();
		}
		
		if(-joyhaxis > joyvaxis && -joyhaxis > -joyvaxis && facing != 1)
		{
			facing = 1;
			timer = timerReset;
		}
		else if(joyhaxis > joyvaxis && joyhaxis > -joyvaxis && facing != 2)
		{
			facing = 2;
			timer = timerReset;
		}
		else if(-joyvaxis > joyhaxis && -joyvaxis > -joyhaxis && facing != 4)
		{
			facing = 4;
			timer = timerReset;
		}
		else if(joyvaxis > joyhaxis && joyvaxis > -joyhaxis && facing != 3)
		{
			facing = 3;
			timer = timerReset;
		}

		if(timer <= 0 && joyhaxis == 0 && joyvaxis == 0)
		{
			facing = 0;
			if(controllerIndex == -1)
				timer = timerReset;
		}
	}

	public void ProcessAI()
	{
		joyhaxis = (float)(((GameManager)GetNode("/root/GameManager")).rand.NextDouble() * 2) - 1;
		joyvaxis = (float)(((GameManager)GetNode("/root/GameManager")).rand.NextDouble() * 2) - 1;

		if(joyhaxis > -0.2f && joyhaxis < 0.2f)
			joyhaxis = 0;
		if(joyvaxis > -0.2f && joyvaxis < 0.2f)
			joyvaxis = 0;
	}
}
