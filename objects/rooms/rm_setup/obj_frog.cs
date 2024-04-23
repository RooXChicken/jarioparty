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
	public short highestDialogueIndex = 0;

	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("res://sound/rooms/rm_setup/mus_main.wav");

		Play("default");
		dialogue = new string[21];
		dialogue[0] = "before your adventure begins, I am going to need to know some basic information";
		dialogue[1] = "how many people are playing on this fine evening";
		dialogue[2] = "1-player game\n1 Player | 3 CPUs";
		dialogue[3] = "2-player game\n2 Players | 2 CPUs";
		dialogue[4] = "3-player game\n3 Players | 1 CPU";
		dialogue[5] = "4-player game \n4 Players";
		dialogue[6] = "who are you chosing to suffer as";
		dialogue[7] = "second player";
		dialogue[8] = "third person";
		dialogue[9] = "fourth peoples";
		dialogue[10] = "how long will you suffer for";
		dialogue[11] = "why are you even playing?";
		dialogue[12] = "why not suffer longer?";
		dialogue[13] = "I see no problems with how long you will suffer";
		dialogue[14] = "You must love suffering";
		dialogue[15] = "Woah. That's a long time.";
		dialogue[16] = "now let the sufferage start";
		dialogue[17] = "This message should not appear.";
		dialogue[19] = "Why did you just go through the entire game setup JUST to leave at the end?? Just who do you think you are? This is MY game, I control everything. YOU CANNOT ESCAPE ME!!!";
		dialogue[20] = "wait nvm";

		obj_dialoguebox = GetNode<Node2D>("../obj_dialoguebox");
		obj_bg = (cm_bg)GetNode<Sprite2D>("../obj_bg");
		UpdateTextbox();
		//stupid = new Alarm(0.1, true, this, new Callable(this, "GoToMinigame"));
		//GoToMinigame();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("back1") && !locked)
		{
			if(dialogueIndex > 0 && dialogueIndex != 19)
			{
				((AudioController)GetNode("/root/AudioController")).PlaySound("gui_back");
				dialogueIndex--;
			}
			else
			{
				if(highestDialogueIndex <= 15)
				{
					((AudioController)GetNode("/root/AudioController")).PlaySound("gui_escape");
					GetNode<Sprite2D>("../obj_fadeout").Visible = true;
				}
				
				else
				{
					if(dialogueIndex == 19)
					{
						dialogueIndex = 20;
						UpdateTextbox();
						((AudioController)GetNode("/root/AudioController")).PlaySound("gui_escape");
						GetNode<Sprite2D>("../obj_fadeout").Visible = true;
					}
					else
					{
						dialogueIndex = 19;
						UpdateTextbox();
						((AudioController)GetNode("/root/AudioController")).PlaySound("gui_back");
					}
				}
			}


			UpdateTextbox();
		}
		if(Input.IsActionJustPressed("jump1") && !locked)
		{
			if(dialogueIndex == 19)
				dialogueIndex = -1;
			dialogueIndex++;
			obj_bg.scrollSpeed *= 1.2f;
			((AudioController)GetNode("/root/AudioController")).PlaySound("dialog_advance");
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
		highestDialogueIndex = Math.Max(dialogueIndex, highestDialogueIndex);
		obj_dialoguebox.GetNode<RichTextLabel>("obj_text").Text = dialogue[dialogueIndex];
	}
}
