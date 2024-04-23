using Godot;
using System;

public partial class bg_cannotescape : Sprite2D
{
	Alarm t_exitHelp;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/rm_quit/mus_cannotescape.wav", "mus_cannotescape");

		((AudioController)GetNode("/root/AudioController")).PlayMusic("mus_cannotescape");
		t_exitHelp = new Alarm(12, true, this, new Callable(this, "ShowHelp"));

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsAnythingPressed())
		{
			GetTree().Quit();
		}
	}

	public void ShowHelp()
	{
		GetNode<RichTextLabel>("../obj_text").Visible = true;
	}
}
