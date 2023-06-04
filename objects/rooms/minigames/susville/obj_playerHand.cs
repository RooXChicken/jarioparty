using Godot;
using System;
using System.Collections.Generic;

public partial class obj_playerHand : RigidBody2D
{
	public int Player = 0;
	public PlayerData playerData;
	private List<string> abilities;
	public int CharacterIndex {get {return playerData.characterIndex; }}
	public int controllerIndex = 1;
	private CanvasLayer layer;
	private AnimatedSprite2D spr_hand;
	private float progress = 1;

	private List<obj_amongus> collisions;

	private float joyhaxis = 0;
	private float joyvaxis = 0;

	private float movementSpeed = 3000;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		joyhaxis = 0;
		joyvaxis = 0;

		spr_hand = GetNode<AnimatedSprite2D>("CanvasLayer/spr_hand");
		layer = GetNode<CanvasLayer>("CanvasLayer");
		collisions = new List<obj_amongus>();

		Variant _Player = GetMeta("Player");
		Player = _Player.As<int>() - 1;

		playerData = ((GameManager)GetNode("/root/GameManager")).playerData[Player];
		abilities = ((GameManager)GetNode("/root/GameManager")).minigameLookup[((GameManager)GetNode("/root/GameManager")).CurrentMinigame].Abilities;

		controllerIndex = playerData.controllerIndex;

		spr_hand.Material = new ShaderMaterial() { Shader = (spr_hand.Material as ShaderMaterial).Shader.Duplicate() as Shader};

		spr_hand.SpriteFrames = playerData.animationFrames;
		spr_hand.Play("shoot");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		layer.Offset = Position;
		((ShaderMaterial)spr_hand.Material).SetShaderParameter("progress", progress);
		if(!((GameManager)GetNode("/root/GameManager")).MinigameStarted)
		{
			return;
		}

		progress += (float)(0.2 * delta);
		progress = Mathf.Clamp(progress, 0, 1);
			
		GetControllerInput();
	}

	public override void _PhysicsProcess(double delta)
	{
		ApplyCentralImpulse(new Vector2(joyhaxis * movementSpeed, joyvaxis * movementSpeed));
	}

	private void GetControllerInput()
	{
		if(controllerIndex < 0)
			return;
		joyhaxis = Input.GetAxis("left" + controllerIndex, "right" + controllerIndex);
		joyvaxis = Input.GetAxis("up" + controllerIndex, "down" + controllerIndex);

		if(Input.IsActionJustPressed("shoot" + controllerIndex) && progress >= 1)
		{
			bool imposter = false;
			foreach(obj_amongus among in collisions)
				if(among.Imposter)
				{
					imposter = true;
					Kill(among);
					break;
				}

			if(!imposter)
			{
				int index = 0;
				while(index < collisions.Count && !Kill(collisions[index]))
					index++;
			}
			else
			{
				GetNode<obj_playerScore>("../../obj_minigameBase/Score/spr_playerBar" + (playerData.playerOrder) + "/spr_playerScore").AddScore(1, playerData);
			}
		}
	}

	private bool Kill(obj_amongus among)
	{
		if(among.Dead)
			return false;
		collisions.Remove(among);
		among.Kill();

		progress = 0;

		return true;
	}

	
	private void OnBodyEntered(Area2D body)
	{
		if(body.Name != "hitDetect")
			return;
		if(!collisions.Contains(body.GetNode<obj_amongus>("../")))
		{
			collisions.Add(body.GetNode<obj_amongus>("../"));
		}
	}


	private void OnBodyExited(Area2D body)
	{
		if(body.Name != "hitDetect")
			return;
		if(collisions.Contains(body.GetNode<obj_amongus>("../")))
		{
			collisions.Remove(body.GetNode<obj_amongus>("../"));
		}
	}
}
