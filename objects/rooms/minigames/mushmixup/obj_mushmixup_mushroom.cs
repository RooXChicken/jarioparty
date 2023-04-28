using Godot;
using System;
using System.Collections.Generic;

public partial class obj_mushmixup_mushroom : Area2D
{
	public List<Area2D> collisions;
	private Sprite2D spr_mushroom;
	private AnimationPlayer anim_mushroom;
	private bool forwards = false;

	public Color color { get { spr_mushroom = GetNode<Sprite2D>("obj_sprite"); return spr_mushroom.Modulate; } set { spr_mushroom = GetNode<Sprite2D>("obj_sprite"); spr_mushroom.Modulate = value; }}

	public override void _Ready()
	{
		collisions = new List<Area2D>();
		anim_mushroom = GetNode<AnimationPlayer>("anim_mushroom");

		//color = new Color(1, 1, 1, 1);
	}

	public void SetAlpha(float alpha)
	{
		((obj_mushmixup_frog)GetNode<Node2D>("../../obj_minigame_mushmixup_frog")).invulnerable = false;
		color = new Color(color.R, color.G, color.B, alpha);
	}

	public void RandomizeAI()
	{
		((obj_mushmixup_frog)GetNode<Node2D>("../../obj_minigame_mushmixup_frog")).RandomizeAITargets();
	}

	public void PlayForwards(float multiplier)
	{
		anim_mushroom.Play("mushroom", -1, multiplier);
	}

	public void PlayBackwards(float multiplier)
	{
		anim_mushroom.Play("mushroomB", -1, multiplier);
	}

	public void StopAnimation()
	{
		anim_mushroom.Stop();
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
