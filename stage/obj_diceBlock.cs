using Godot;
using System;

public partial class obj_diceBlock : Node2D
{
	private AnimatedSprite2D spr_diceblock;
	private AnimatedSprite2D spr_diceHit;
	private AnimatedSprite2D spr_number;
	private Alarm t_spinCycle;
	private obj_character_map player;
	private AnimationPlayer anim_items;
	public int num = -1;
	private int extra = 0;

	private bool diceHit = false;
	private bool numberAnimation = false;
	public byte numState = 0;
	private float fadeAlpha = 1;
	public bool playSound = true;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(ZIndex == 2)
			if(((GameManager)GetNode("/root/GameManager")).playerData[0].PlayerStarted)
			{
				QueueFree();
				return;
			}
		
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://stage/sound/snd_diceblockRoll.wav", "diceblockRoll");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://stage/sound/snd_diceblockHit.wav", "diceblockHit");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://stage/sound/snd_diceblockNumber.wav", "diceblockNumber");
		spr_diceblock = GetNode<AnimatedSprite2D>("obj_rb/spr_diceblock");
		anim_items = GetNode<AnimationPlayer>("anim_items");
		spr_diceblock.Material = new ShaderMaterial() { Shader = (spr_diceblock.Material as ShaderMaterial).Shader.Duplicate() as Shader};
		spr_diceHit = GetNode<AnimatedSprite2D>("spr_diceHit");
		spr_number = GetNode<AnimatedSprite2D>("obj_rb/spr_number");
		t_spinCycle = new Alarm(0.2 * 3, true, this, new Callable(this, "a_spinCycle"), false);

		Initialize();
	}

	public void Initialize()
	{
		((ShaderMaterial)spr_diceblock.Material).SetShaderParameter("alpha", 0);
		spr_diceHit.Visible = false;
		spr_number.Visible = false;

		GetNode<RigidBody2D>("obj_rb").Freeze = true;
		GetNode<RigidBody2D>("obj_rb").Sleeping = true;

		num = -1;
		extra = 0;
		diceHit = false;
		numberAnimation = false;
		numState = 0;
		fadeAlpha = 1;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		//GD.Print("Dice pos: " + Position + " | RB pos: " + GetNode<RigidBody2D>("obj_rb").Position);
		if(numberAnimation)
			NumberAnimation((float)delta);
	}

	public void a_spinStart(obj_character_map _player)
	{
		player = _player;
		GetNode<obj_diceRB>("obj_rb").newPosition = new Vector2(Position.X, Position.Y - 14);
		GetNode<obj_diceRB>("obj_rb").shouldSnap = true;
		GetNode<RigidBody2D>("obj_rb").Freeze = false;
		GetNode<RigidBody2D>("obj_rb").Sleeping = false;
		
		if(playSound)
			((AudioController)GetNode("/root/AudioController")).PlaySound("diceblockRoll");
		spr_diceblock.Play("spinStart");
		t_spinCycle.WaitTime = 0.2 * 3;
		t_spinCycle.Start();

		if(player.playerData.PowerupState == 1)
		{
			GetNode<Node2D>("Plus3").Visible = true;
			extra = 3;
		}
	}

	public void Show()
	{
		Visible = true;
		((ShaderMaterial)spr_diceblock.Material).SetShaderParameter("alpha", 1);
	}

	public void a_spinCycle()
	{
		player.canJump = true;
		spr_diceblock.Play("spinning");
	}

	private void DiceHit()
	{
		diceHit = true;
		spr_diceHit.Visible = true;
		spr_diceHit.Play();
		spr_diceblock.Play("spinEnd");
		if(playSound)
			((AudioController)GetNode("/root/AudioController")).StopSound("diceblockRoll");
		
		((AudioController)GetNode("/root/AudioController")).PlaySound("diceblockHit");
		num = ((GameManager)GetNode("/root/GameManager")).rand.Next(1, 9);
		spr_number.Frame = num;
		player.playerData.diceRoll = num;
	}

	private void NumberAnimation(float delta)
	{
		spr_number.Visible = true;
		switch(numState)
		{
			case 0:
				GetNode<RigidBody2D>("obj_rb").Freeze = true;
				spr_number.Scale = spr_number.Scale.Lerp(new Vector2(9, 9), delta * 6f);
				if(spr_number.Scale.X > 8.8f)
					numState = 1;
				break;
			case 1:
				spr_number.Scale = spr_number.Scale.Lerp(new Vector2(3, 3), delta * 6f);
				if(spr_number.Scale.X < 3.2f)
				{
					numState = 2;
				}
				break;
			case 2:
				fadeAlpha -= (float)delta * 1.4f;
				((ShaderMaterial)spr_diceblock.Material).SetShaderParameter("alpha", fadeAlpha);
				if(fadeAlpha <= 0)
				{
					//Visible = false;
					numState = 3;
				}
				break;
		}
	}

	private void bodyEntered(Rid body_rid, Node body, long body_shape_index, long local_shape_index)
	{
		if(body.Name == "obj_character_map" && !diceHit)
			DiceHit();
		if(!numberAnimation && diceHit && body.Name == "obj_floor")
		{
			GetNode<CollisionShape2D>("obj_floor/obj_hitbox").SetDeferred("Disabled", true);
			GetNode<CollisionShape2D>("obj_pfloor/obj_hitbox").SetDeferred("Disabled", true);
			numberAnimation = true;
			((AudioController)GetNode("/root/AudioController")).PlaySound("diceblockNumber");
		}
	}

	public int DecrementDice()
	{
		if(extra > 0)
		{
			extra--;
			GetNode<AnimatedSprite2D>("Plus3/spr_number").Frame--;

			return 0;
		}
		else
		{
			GetNode<Node2D>("Plus3").Visible = false;
			player.playerData.PowerupState = 0;
		}

		num--;
		spr_number.Frame = num;
		
		return -1;
	}
}
