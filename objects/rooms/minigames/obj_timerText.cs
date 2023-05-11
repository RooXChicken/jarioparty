using Godot;
using System;

public partial class obj_timerText : RichTextLabel
{
	public int Time = 60;
	private float x = 2.75f;

	public Callable onEnd;

	public override void _Ready()
	{
		((GameManager)GetNode("/root/GameManager")).MinigameStarted = false;
		Variant _Minigame = GetNode<Node2D>("../../").GetMeta("Minigame");
		Time = ((GameManager)GetNode("/root/GameManager")).minigameLookup[_Minigame.As<int>()].MinigameTime + 2;
		((AudioController)GetNode("/root/AudioController")).PlaySound(("minigameStart"));

		GetNode<AnimationPlayer>("../../anim_start").Play("start");
	}

	private void TimerStart()
	{
		if(Time < 0)
		{
			GetNode<AnimatedSprite2D>("../").Stop();
			GetNode<AnimatedSprite2D>("../").Visible = false;
		}
		
		if(GetNode<AnimatedSprite2D>("../").Visible)
			GetNode<AnimatedSprite2D>("../").Play();
	}

	private void FrameChanged()
	{
		if(!((GameManager)GetNode("/root/GameManager")).MinigameStarted)
		{
			GetNode<AnimatedSprite2D>("../").Stop();
		}

		Vector2 newPos;
		switch(GetNode<AnimatedSprite2D>("../").Frame)
		{
			case 0:
				Time--;
				if(Time < 20 && GetNode<AnimatedSprite2D>("../").Animation != "lowTime")
					GetNode<AnimatedSprite2D>("../").Play("lowTime");
				if(Time <= 0)
					onEnd.Call();
				newPos = new Vector2(x, -9.5f);
				break;
			case 1:
				newPos = new Vector2(x, -10.5f);
				break;

			default:
				newPos = new Vector2(x, -11.5f);
				break;
		}

		Position = newPos;
		Text = Time.ToString();
	}
}
