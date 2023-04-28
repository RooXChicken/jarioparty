using Godot;
using System;

public partial class obj_start : Sprite2D
{
	public Callable onStart;

	private void StartMinigame(StringName anim_name)
	{
		if(anim_name != "start")
			return;

		((AudioController)GetNode("/root/AudioController")).PlayMusic(((GameManager)GetNode("/root/GameManager")).minigameLookup[((GameManager)GetNode("/root/GameManager")).CurrentMinigame].Music);
		((GameManager)GetNode("/root/GameManager")).MinigameStarted = true;

		onStart.Call();

		GetNode<AnimationPlayer>("../anim_start").Play("fadeout");
	}
}
