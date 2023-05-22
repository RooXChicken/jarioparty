using Godot;
using System;

public partial class obj_frog : AnimatedSprite2D
{
	private string[] dialogue;
	public bool locked = false;

	private Node2D obj_dialoguebox;
	private cm_bg obj_bg;

	private float floatSpeed = 50f;
	private short state = 0;
	public short dialogueIndex = 0;

	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("res://sound/rooms/rm_setup/mus_main.wav");

		Play("default");
		dialogue = new string[16];
		dialogue[0] = "before your adventure begins, I am going to need to know some basic information";
		dialogue[1] = "how many people are playing on this fine evening";
		dialogue[2] = "1-player game\n1 Player vs. 3 CPU";
		dialogue[3] = "2-player game\n2 Players vs. 2 CPU";
		dialogue[4] = "3-player game\n3 Players vs. 1 CPU";
		dialogue[5] = "4-player game \n4 Players";
		dialogue[6] = "who are you chosing to suffer as";
		dialogue[7] = "second player";
		dialogue[8] = "third person";
		dialogue[9] = "fourth peoples";
		dialogue[10] = "now let the sufferage start";

		obj_dialoguebox = GetNode<Node2D>("../obj_dialoguebox");
		obj_bg = (cm_bg)GetNode<Sprite2D>("../obj_bg");
		UpdateTextbox();
		//stupid = new Alarm(0.1, true, this, new Callable(this, "GoToMinigame"));
		//GoToMinigame();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("jump1") && !locked)
		{
			dialogueIndex++;
			obj_bg.scrollSpeed *= 1.2f;
			if(dialogueIndex == 2)
			{
				GetNode<Node2D>("../obj_playerSelect").Visible = true;
				locked = true;
			}
			UpdateTextbox();
		}
		// switch(state)
		// {
		// 	case 0:
		// 		Position = new Vector2(Position.X, Position.Y + (floatSpeed * (float)delta));
		// 		if(Position.Y >= 600)
		// 			state = 1;
		// 		break;
		// 	case 1:
		// 		Position = new Vector2(Position.X, Position.Y - (floatSpeed * (float)delta));
		// 		if(Position.Y <= 560)
		// 			state = 0;
		// 		break;
		// }
	}

	public void UpdateTextbox()
	{
		obj_dialoguebox.GetNode<RichTextLabel>("obj_text").Text = dialogue[dialogueIndex];
	}

	public void GoToMinigame()
	{
		((GameManager)GetNode("/root/GameManager")).SwitchScene("minigames/rm_minigame_mushmixup");
	}
}
