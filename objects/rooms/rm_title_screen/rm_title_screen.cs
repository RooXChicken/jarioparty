using Godot;
using System;

public partial class rm_title_screen : Node2D
{
	Sprite2D obj_logo;
	Sprite2D obj_bg;
	Node2D obj_startButtons;
	Alarm t_startAnimation;
	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("res://sound/rooms/rm_title_screen/mus_main.wav");

		obj_logo = GetNode<Sprite2D>("obj_logo");
		obj_bg = GetNode<Sprite2D>("obj_bg");
		obj_startButtons = GetNode<Node2D>("obj_startButtons");

		t_startAnimation = new Alarm(1.7, true, this, new Callable(this, "StartAnimation"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	private void StartAnimation()
	{
		obj_logo.Visible = true;
		obj_bg.Visible = true;
		obj_startButtons.Visible = true;
		obj_startButtons.GetNode<Sprite2D>("obj_pressStart").Visible = true;
	}
}
