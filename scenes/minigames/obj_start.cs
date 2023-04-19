using Godot;
using System;

public partial class obj_start : Sprite2D
{
	private void StartMinigame(StringName anim_name)
	{
		if(anim_name != "start")
			return;

		((AudioController)GetNode("/root/AudioController")).PlayMusic(((GameManager)GetNode("/root/GameManager")).minigameLookup[((GameManager)GetNode("/root/GameManager")).CurrentMinigame].Music);

		GetNode<obj_mushmixup_frog>("../../obj_minigame_mushmixup_frog").invulnerable = false;
		GetNode<obj_mushmixup_frog>("../../obj_minigame_mushmixup_frog").state = 0;

		((GameManager)GetNode("/root/GameManager")).MinigameStarted = true;

		GetNode<AnimationPlayer>("../anim_start").Play("fadeout");
	}
}
