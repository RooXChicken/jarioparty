using Godot;
using System;
using System.Collections.Generic;

public partial class obj_mushmixup_frog : Node2D
{
	public bool invulnerable = true;
	public List<Color> colors;

	//private 
	private int mushroomIndex = 0;
	private obj_mushmixup_mushroom[] mushrooms;
	private obj_character_parent[] players;
	private obj_mushmixup_mushroom mush;
	private Random rand;

	private AnimatedSprite2D obj_sprite;
	private AnimatedSprite2D obj_flag;

	private float startDelay = 1;
	
	public short state = -1;
	private Alarm t_goDown;
	private Alarm t_goUp;

	public override void _Ready()
	{
		obj_sprite = GetNode<AnimatedSprite2D>("obj_sprite");
		obj_flag = GetNode<AnimatedSprite2D>("obj_flag");

		obj_sprite.Play("idle");
		obj_flag.Play();
		obj_flag.Visible = false;

		rand = new Random();
		colors = new List<Color>()
		{ GMLColor(219, 43, 43), GMLColor(51, 100, 236), GMLColor(37, 215, 73), GMLColor(246, 202, 56), GMLColor(246, 147, 34), GMLColor(110, 52, 226), GMLColor(229, 135, 247) };

		mushrooms = new obj_mushmixup_mushroom[7];
		for(int k = 0; k < 100; k++)
		{
			Color c = colors[rand.Next(0, 7)];
			colors.Remove(c);
			colors.Add(c);
		}
		for(int i = 1; i <= 7; i++)
		{
			mushrooms[i - 1] = (obj_mushmixup_mushroom)GetNode<Area2D>("../mushrooms/obj_mushmixup_mushroom" + i);
			mushrooms[i - 1].color = colors[i - 1];
		}

		GetNode<obj_mushmixup_mushroom>("obj_mushmixup_mushroom").color = GMLColor(255, 255, 255);

		players = new obj_character_parent[4];
		for(int i = 1; i <= 4; i++)
		{
			players[i - 1] = (obj_character_parent)GetNode<RigidBody2D>("../players/obj_character_parent" + i);
			players[i - 1].scale = 3;
		}

		t_goDown = new Alarm(2.5 + startDelay, true, this, new Callable(this, "ShroomsGoDown"));
		t_goUp = new Alarm(4 + startDelay, true, this, new Callable(this, "ShroomsGoUp"), false);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		switch(state)
		{
			case 0:
				if(!invulnerable)
				{
				for(int k = 0; k < 4; k++)
				{
					bool playerOut = true;
					for(int i = 0; i < 7; i++)
					{
						if(players[k].Lost)
						{
							playerOut = false;
							return;
						}
						if(_CheckMushCollision(players[k], mushrooms[i]))
							playerOut = false;
					}

					if(playerOut)
						KillPlayer(k);
				}
				}
				break;
			case 1:
				if(!invulnerable)
				{
				for(int k = 0; k < 4; k++)
				{
					if(!players[k].Lost && !_CheckMushCollision(players[k], mush))
						KillPlayer(k);
				}
				}
				break;
		}
	}

	private void KillPlayer(int player)
	{
		players[player].Lost = true;
		GetNode<obj_splash>("../splashes/obj_splash" + (player + 1)).Position = players[player].Position;
		GetNode<obj_splash>("../splashes/obj_splash" + (player + 1)).Splash();
		players[player].Position = new Vector2(1000000, 0);
	}

	private bool _CheckMushCollision(obj_character_parent character, obj_mushmixup_mushroom _mush)
	{
		if(_mush.collisions.Contains(character.GetNode<Area2D>("obj_feetArea")))
			return true;
		
		return false;
	}

	private Color GMLColor(float r, float g, float b)
	{
		return new Color(r / 255, g / 255, b / 255, 1);
	}

	public void ShroomsGoDown()
	{
		for(int i = 0; i < 7; i++)
		{
			mushrooms[i].state = 1;
		}

		state = 1;

		invulnerable = true;
		
		mush = mushrooms[rand.Next(0, 7)];
		mush.state = 0;
		GetNode<obj_mushmixup_mushroom>("obj_mushmixup_mushroom").color = mush.color;
		GetNode<AnimatedSprite2D>("obj_flag").Modulate = mush.color;
		obj_sprite.Play("flagShown");
		obj_flag.Visible = true;

		t_goUp.WaitTime -= 0.1;
		t_goUp.Start();
	}

	public void ShroomsGoUp()
	{
		for(int i = 0; i < 7; i++)
		{
			mushrooms[i].state = 2;
			mushrooms[i].mult += 0.02f;
		}

		obj_sprite.Play("idle");
		obj_flag.Visible = false;

		state = 0;

		invulnerable = true;
		
		t_goDown.WaitTime -= 0.1;
		t_goDown.Start();
	}
}
