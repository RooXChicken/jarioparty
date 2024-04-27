using Godot;
using System;
using System.Data;

public partial class obj_pause : CanvasLayer
{
	private float alpha = 1;
	private float alphaMult = -0.02f;
	private int index = 0;
	private float joyvaxis;
	private sbyte dir;
	private sbyte upper = 2;
	private sbyte state = 0;

	private Vector2I[] yHeights;
	private Vector2I[] escapeHeights;
	private RichTextLabel obj_pauseText;
	private Sprite2D spr_arrow;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
		obj_pauseText = GetNode<RichTextLabel>("obj_text");
		spr_arrow = GetNode<Sprite2D>("spr_arrow");
		yHeights = new Vector2I[3] {new Vector2I(510, 350), new Vector2I(510, 410), new Vector2I(510, 466)};
		escapeHeights = new Vector2I[3] {new Vector2I(566, 350), new Vector2I(578, 410), new Vector2I(518, 465)};
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/gui/snd_common_escape.wav", "gui_escape");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/gui/snd_common_select.wav", "gui_select");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/gui/snd_common_back.wav", "gui_back");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/gui/snd_common_selectionmove.wav", "gui_selectionMove");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		joyvaxis = Input.GetAxis("down" + ((GameManager)GetNode("/root/GameManager")).PausePlayer, "up" + ((GameManager)GetNode("/root/GameManager")).PausePlayer);

		if(joyvaxis < 0)
		{
			if(dir != -1)
			{
				dir = -1;
				index++;
				((AudioController)GetNode("/root/AudioController")).PlaySound("gui_selectionMove");
			}
		}
		else if(joyvaxis > 0)
		{
			if(dir != 1)
			{
				dir = 1;
				index--;
				((AudioController)GetNode("/root/AudioController")).PlaySound("gui_selectionMove");
			}
		}
		else
			dir = 0;

		if(index < 0)
			index = upper;
		else if(index > upper)
			index = 0;

		if(state == 0)
			spr_arrow.Position = yHeights[index];
		else if(state == 1)
			spr_arrow.Position = escapeHeights[index];

		if(Input.IsActionJustPressed("jump" + ((GameManager)GetNode("/root/GameManager")).PausePlayer))
		{
			if(state == 0)
			switch(index)
			{
				case 0:
					Alarm a = new Alarm(0.00001, true, this, new Callable(this, "UnPause"));
					return;
				case 1:
					((AudioController)GetNode("/root/AudioController")).PlaySound("gui_select");
					((GameManager)GetNode("/root/GameManager")).LoadDefaults(1);
					((GameManager)GetNode("/root/GameManager")).SwitchScene("rm_minigame_info");
					return;
				case 2:
					((AudioController)GetNode("/root/AudioController")).PlaySound("gui_select");
					ChangeText("[center]Are you sure?\nYes\nNo\nMaybe", 1);
					return;
			}
			if(state == 1)
			switch(index)
			{
				case 0:
					((AudioController)GetNode("/root/AudioController")).PlaySound("gui_select");
					QuitGame();
					return;
				case 1:
					ChangeText("[center]Player " + ((GameManager)GetNode("/root/GameManager")).PausePlayer + " Paused!\nResume\nOptions\nEscape", 0);
					((AudioController)GetNode("/root/AudioController")).PlaySound("gui_back");
					state = 0;
					return;
				case 2:
					Random rand = new Random();
					if(rand.Next(0, 2) == 1)
					{
						((AudioController)GetNode("/root/AudioController")).PlaySound("gui_select");
						QuitGame();
					}
					else
						((AudioController)GetNode("/root/AudioController")).PlaySound("gui_badSelect");
					return;
			}
		}

		if(Input.IsActionJustPressed("back" + ((GameManager)GetNode("/root/GameManager")).PausePlayer))
		{
			state--;
			ChangeText("[center]Player " + ((GameManager)GetNode("/root/GameManager")).PausePlayer + " Paused!\nResume\nOptions\nEscape", -1);
			if(state < 0)
				UnPause();
			else
				((AudioController)GetNode("/root/AudioController")).PlaySound("gui_back");

		}
		
		// if(!((GameManager)GetNode("/root/GameManager")).Paused)
		// 	return;

		// alpha += alphaMult;

		// if(alpha < 0.5f || alpha > 1)
		// 	alphaMult *= -1;

		// alpha = Math.Clamp(alpha, 0, 1);

		// Modulate = new Color(1, 1, 1, alpha);
	}

	private void QuitGame()
	{
		((GameManager)GetNode("/root/GameManager")).SwitchScene("rm_title_screen");
		UnPause();
	}

	private void UnPause()
	{
		((GameManager)GetNode("/root/GameManager")).Paused = false;
		Input.ActionRelease("jump1");
		TogglePause();
	}

	private void ChangeText(string text, sbyte _state)
	{
		obj_pauseText.Text = text;
		index = 0;
		if(_state != -1)
			state = _state;
	}

	public void TogglePause()
	{
		Visible = ((GameManager)GetNode("/root/GameManager")).Paused;
		alpha = 1;

		ChangeText("[center]Player " + ((GameManager)GetNode("/root/GameManager")).PausePlayer + " Paused!\nResume\nOptions\nEscape", 0);
		//obj_pauseText.AddThemeColorOverride("font_outline_color", ((GameManager)GetNode("/root/GameManager")).playerData[((GameManager)GetNode("/root/GameManager")).PausePlayer-1].PlayerColor);

		GetTree().Paused = Visible;
		((AudioController)GetNode("/root/AudioController")).PlaySound("gui_escape", false, true);
	}
}
