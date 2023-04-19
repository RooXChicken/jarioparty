using Godot;
using System;

public partial class obj_coinIndicator : Node2D
{
	private AnimatedSprite2D spr_indicator;
	private AnimationPlayer anim_coinIndicator;

	public bool AnimationFinished { get {return anim_coinIndicator.IsPlaying();} }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
		spr_indicator = GetNode<AnimatedSprite2D>("spr_indicator");
		anim_coinIndicator = GetNode<AnimationPlayer>("../anim_coinIndicator");
	}

	public void PlayAnimation(int coins)
	{
		GetNode<AnimatedSprite2D>("spr_numCoins").Frame = Math.Abs(coins);
		if(coins < 0)
			spr_indicator.Frame = 1;
		else
			spr_indicator.Frame = 0;
		anim_coinIndicator.Play("coinFade");
	}
}
