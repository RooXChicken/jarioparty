using Godot;
using System;

public partial class rm_minigame_info : Node2D
{
	private AnimationPlayer anim_whackitu;
	private Node2D spr_whackitu;
	private RichTextLabel obj_text;
	private bool itemWindowShown = false;
	private string nextMinigame;
	private Alarm t_music;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		int minigame = ((GameManager)GetNode("/root/GameManager")).CurrentMinigame;
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/mus_playMinigamesIntro.wav", "mus_playMinigamesIntro");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/snd_itemPickerMove.wav", "itemPickerMove");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/mus_playMinigames.wav", "mus_playMinigames");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/snd_start.wav", "minigameStart");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/mus_results.wav", "minigameResults");

		MinigameBase game = ((GameManager)GetNode("/root/GameManager")).minigameLookup[minigame];

		anim_whackitu = GetNode<AnimationPlayer>("anim_whackitu");
		obj_text = GetNode<RichTextLabel>("obj_text");
		obj_text.Text = game.Description;
		nextMinigame = "rm_minigame_" + game.MinigameRoom;
		spr_whackitu = GetNode<Sprite2D>("spr_itemWindow");
		spr_whackitu.Visible = false;

		if(game.ButtonIndexs != null)
		{
			for(int i = 0; i < game.ButtonIndexs.Length; i++)
			{
				Node2D button = GD.Load<PackedScene>("res://objects/common/buttons/obj_button.tscn").Instantiate<Node2D>();
				button.Name = "obj_button" + i;
				button.Position = new Vector2(646, 536 + (27 * i));
				button.Scale = new Vector2(0.15f, 0.15f);
				button.SetMeta("Button", game.ButtonIndexs[i]);

				GetNode<Node2D>("Buttons").AddChild(button);
			}
		}

		((AudioController)GetNode("/root/AudioController")).PlaySound("mus_playMinigamesIntro");

		t_music = new Alarm(0.72, true, this, new Callable(this, "PlayMusic"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("pause1") && !GetNode<AnimationPlayer>("anim_whackitu").IsPlaying())
		{
			((AudioController)GetNode("/root/AudioController")).PlaySound("transition");
			GetNode<AnimationPlayer>("anim_whackitu").Play("transition");
		}
		// if(!itemWindowShown && Input.IsActionJustPressed("pause1"))
		// 	ShowItemWindow();
		// if(itemWindowShown && Input.IsActionJustPressed("pause1") && !GetNode<AnimationPlayer>("anim_whackitu").IsPlaying())
		// {
		// 	((AudioController)GetNode("/root/AudioController")).PlaySound("transition");
		// 	GetNode<AnimationPlayer>("anim_whackitu").Play("transition");
		// }
	}

	private void ShowItemWindow()
	{
		itemWindowShown = true;

		anim_whackitu.CurrentAnimation = "itemWindow";
		anim_whackitu.Play();

		spr_whackitu.Visible = true;
	}

	private void TransitionFinished(StringName anim_name)
	{
		if(anim_name == "transition")
		{
			((GameManager)GetNode("/root/GameManager")).SwitchScene("minigames/" + nextMinigame);
		}
	}


	public void PlayMusic()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("mus_playMinigames");
	}
}
