using Godot;
using System;
using System.Collections.Generic;

public partial class obj_player_tugofjar : Node2D
{
	public int Player = 0;
	public PlayerData playerData;
	public int CharacterIndex {get {return playerData.characterIndex; }}
	public int controllerIndex = 1;

	private AnimatedSprite2D sprite;
	private Sprite2D arm;
	private Random rand;

	public int strength = 1;
	private double time;
	private Alarm t_anim;
	
	private Callable fn_pull;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite2D>("spr_character");
		arm = GetNode<Sprite2D>("spr_arm");
		fn_pull = new Callable(GetNode<Node2D>("../../../"), "Pull");

		rand = new Random();

		Variant _Player = GetMeta("Player");
		Player = _Player.As<int>() - 1;

		playerData = ((GameManager)GetNode("/root/GameManager")).playerData[Player];
		sprite.SpriteFrames = playerData.animationFrames;
		controllerIndex = playerData.controllerIndex;

		arm.Frame = playerData.characterIndex;
		sprite.Play("pull");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!((GameManager)GetNode("/root/GameManager")).MinigameStarted)
			return;

		time -= delta;
		GetControllerInput();
	}

	private void GetControllerInput()
	{
		if(controllerIndex < 0)
		{
			if(time <= 0)
			{
				time = rand.NextDouble() * 0.5;
				fn_pull.Call(strength);
			}
			return;
		}
		if(Input.IsActionJustPressed("jump" + controllerIndex))
		{
			fn_pull.Call(strength);
		}
	}

	public void Winning()
	{
		sprite.Play("pulled");
	}

	public void Losing()
	{
		sprite.Play("pull");
	}
}
