using Godot;
using System;
using System.Collections.Generic;

public partial class obj_mushmixup_mushroom : Area2D
{
	public List<Area2D> collisions;
	private Sprite2D obj_sprite;
	private Sprite2D obj_spots;

	public Color color;

	public short state = 0;

	private float alpha = 1;
	private float scale = 1.75f;
	public float mult = 0.37f;

	//Alarm t_anim;

	public override void _Ready()
	{
		collisions = new List<Area2D>();
		obj_sprite = GetNode<Sprite2D>("obj_sprite");
		obj_spots = GetNode<Sprite2D>("obj_spots");

		//t_anim = new Alarm(1, true, this, new Callable(this, ))
	}

	public override void _Process(double delta)
	{
		obj_sprite.Modulate = new Color(color.R, color.G, color.B, alpha);
		switch(state)
		{
			case 0:
				return;
				break;
			case 1:
				if(scale > 1.25f)
				{
					scale -= (mult * 1.65f) * (float)delta;
				}
				else
				{
					scale = 1.25f;
					((obj_mushmixup_frog)GetNode<Node2D>("../../obj_minigame_mushmixup_frog")).invulnerable = false;
					state = 3;
				}
				break;

			case 2:
				alpha = 1;
				((obj_mushmixup_frog)GetNode<Node2D>("../../obj_minigame_mushmixup_frog")).invulnerable = false;
				state = 4;
				// if(alpha < 1f)
				// {
				// 	alpha += mult * (float)delta * 2.5f * 2;
				// }
				// else
				// {
				// 	alpha = 1;
				// 	((obj_mushmixup_frog)GetNode<Node2D>("../../obj_minigame_mushmixup_frog")).invulnerable = false;
				// 	state = 4;
				// }
				break;

			case 3:
				alpha = 0.6f;
				state = 0;
				// if(alpha > 0.6f)
				// {
				// 	alpha -= mult * (float)delta * 3;
				// }
				// else
				// {
				// 	alpha = 0.6f;
				// 	state = 0;
				// }
				break;

			case 4:
				if(scale < 1.75f)
				{
					scale += (mult * 1.65f) * (float)delta * 2.5f;
				}
				else
				{
					scale = 1.75f;
					state = 0;
				}
				break;
		}

		Scale = new Vector2(scale, scale);
	}

	
	private void OnBodyEntered(Area2D body)
	{
		if(!collisions.Contains(body))
			collisions.Add(body);
	}

	private void OnBodyExit(Area2D body)
	{
		if(collisions.Contains(body))
			collisions.Remove(body);
	}

}
