using Godot;
using System;

public partial class obj_cloudTransition : Node2D
{
	private AnimationPlayer anim_cloudTransition;
	public void PlayAnimation() { anim_cloudTransition.Play("clouds"); }
	public bool AnimationFinished { get {return anim_cloudTransition.IsPlaying(); } }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		anim_cloudTransition = GetNode<AnimationPlayer>("anim_cloudTransition");
		Variant _Backwards = GetMeta("Backwards");
		if(_Backwards.As<bool>())
		{
			Visible = true;
			anim_cloudTransition.Seek(1.5, true);
			anim_cloudTransition.PlayBackwards("clouds");
			return;
		}
	}
}
