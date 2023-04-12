using Godot;
using System;

public partial class rm_minigame_info : Node2D
{
	private AnimationPlayer anim_whackitu;
	private Node2D spr_whackitu;
	private bool itemWindowShown = false;
	Alarm t_music;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/mus_playMinigamesIntro.wav", "mus_playMinigamesIntro");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/snd_itemPickerMove.wav", "itemPickerMove");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/mus_playMinigames.wav", "mus_playMinigames");
		anim_whackitu = GetNode<AnimationPlayer>("anim_whackitu");
		spr_whackitu = GetNode<Sprite2D>("spr_itemWindow");
		spr_whackitu.Visible = false;

		((AudioController)GetNode("/root/AudioController")).PlaySound("mus_playMinigamesIntro");

		t_music = new Alarm(0.72, true, this, new Callable(this, "PlayMusic"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(!itemWindowShown && Input.IsActionJustPressed("jump1"))
			ShowItemWindow();
	}

	private void ShowItemWindow()
	{
		itemWindowShown = true;

		anim_whackitu.CurrentAnimation = "itemWindow";
		anim_whackitu.Play();

		spr_whackitu.Visible = true;
	}

	public void PlayMusic()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("mus_playMinigames");
	}
}
