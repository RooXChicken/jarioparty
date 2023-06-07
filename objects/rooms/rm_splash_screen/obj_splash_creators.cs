using Godot;
using System;

public partial class obj_splash_creators : Sprite2D
{
	private float scale = 0f;
	public float scaleMultiplier = 2f;
	private Alarm t_transition;
	private bool animationFinished = false;

	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/snd_coin.wav", "minigame_coin");
		t_transition = new Alarm(2.4, true, this, new Callable(this, "GoToTitleScreen"));

		GetNode<AnimationPlayer>("../anim_splash").CurrentAnimation = "anim_splash";
		
		GetNode<AnimationPlayer>("../anim_splash").Play();
		((AudioController)GetNode("/root/AudioController")).PlaySound("minigame_coin");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("jump1"))
			GoToTitleScreen();
		// if(scale < 1.19f)
		// 	scale += scaleMultiplier * (float)delta;
		// else
		// {
		// 	scale = 1.2f;
		// 	if(!animationFinished)
		// 	{
		// 		animationFinished = true;
		// 		t_transition.Start();
		// 	}
		// }

		// Scale = new Vector2(scale, scale);
	}

	public void GoToTitleScreen()
	{
		GetNode<AnimatedSprite2D>("/root/rm_game/spr_load").Play();
		//((GameManager)GetNode("/root/GameManager")).LoadDefaults();
		//((GameManager)GetNode("/root/GameManager")).SwitchScene("rm_minigame_info");
		((GameManager)GetNode("/root/GameManager")).SwitchScene("rm_credits");
		//((GameManager)GetNode("/root/GameManager")).SwitchScene("rm_title_screen");
	}
}
