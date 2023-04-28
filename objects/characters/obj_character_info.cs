using Godot;
using System;

public partial class obj_character_info : Node2D
{
	public int Player = 0;
	public int controllerIndex = 1;

	private AnimatedSprite2D sprite;
	private PlayerData playerData;
	private float joyhaxis = 0;
	private float joyvaxis = 0;

	private int flip;
	private float distanceMoved = 0;
	public float scale = 4;

	public bool Animatable = false;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Variant _Player = GetMeta("Player");
		Player = _Player.As<int>() - 1;

		playerData = ((GameManager)GetNode("/root/GameManager")).playerData[Player];

		controllerIndex = playerData.controllerIndex;

		sprite = GetNode<AnimatedSprite2D>("obj_sprite");
		sprite.SpriteFrames = playerData.animationFrames;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		GetControllerInput();

		ProcessAnimations((float)delta);
	}

	private void GetControllerInput()
	{
		if(controllerIndex == -1)
		{
			if(joyhaxis != 0 || joyvaxis != 0)
			{
				joyhaxis = 0;
				joyvaxis = 0;
			}
			return;
		}

		joyhaxis = Input.GetAxis("left" + controllerIndex, "right" + controllerIndex);
		joyvaxis = Input.GetAxis("up" + controllerIndex, "down" + controllerIndex);
	}

	private void ProcessAnimations(float delta)
	{
		if(!Animatable)
		{
			joyvaxis = 0;
			joyhaxis = 1;

			distanceMoved += delta * 250;
			Position = new Vector2(Position.X + (delta * 250), Position.Y);
			
			if(distanceMoved >= 500)
				Animatable = true;
		}
		if(joyhaxis == 0 && joyvaxis == 0)
		{
			sprite.Frame = 0;
		}

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
