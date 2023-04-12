using Godot;
using System;

public partial class rm_title_screen : Node2D
{
	private Sprite2D obj_logo;
	private Sprite2D spr_bg;
	private Sprite2D spr_arrowLeft;

	private Alarm t_startAnimation;
	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("res://sound/rooms/rm_title_screen/mus_main.wav");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/gui/snd_title_select.wav", "gui_titleScreenSelect");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/gui/snd_common_selectionmove.wav", "gui_selectionMove");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/gui/snd_common_select.wav", "gui_select");

		obj_logo = GetNode<Sprite2D>("obj_logo");
		obj_logo.Visible = false;
		spr_bg = GetNode<Sprite2D>("spr_bg");
		spr_bg.Visible = false;
		spr_arrowLeft = GetNode<Sprite2D>("obj_logo/spr_arrowLeft");
		spr_arrowLeft.Visible = false;
		//obj_startButtons = GetNode<Node2D>("obj_startButtons");

		t_startAnimation = new Alarm(1.7, true, this, new Callable(this, "StartAnimation"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void StartAnimation()
	{
		obj_logo.Visible = true;
		spr_bg.Visible = true;
		spr_arrowLeft.Visible = true;
		//obj_startButtons.Visible = true;
		//obj_startButtons.GetNode<Sprite2D>("obj_pressStart").Visible = true;
	}
}
