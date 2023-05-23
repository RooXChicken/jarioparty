using Godot;
using System;
using System.Collections.Generic;

public partial class spc_star : Area2D
{
	private AnimatedSprite2D spr_characterSprite;
	private obj_dialogueController obj_dialogueBox;
	private Callable interactionEnd;
	private AnimationPlayer anim_starGet;

	private double delay = 0.08;
	private double interactDelay = 1.4;

	private bool dialogue = false;
	private bool frozen = false;
	private PlayerData playerData;
	private bool buyingStar = false;

	private int coinsTaken = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		obj_dialogueBox = GetNode<obj_dialogueController>("spr_toomba/obj_dialogueController");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://stage/sound/snd_starGet.wav", "starGet");
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://stage/sound/snd_starCollect.wav", "starCollect");
		anim_starGet = GetNode<AnimationPlayer>("spr_toomba/anim_starGet");
		spr_characterSprite = GetNode<AnimatedSprite2D>("spr_toomba");

		string[] toombaDialogue = new string[10];
		toombaDialogue[0] = "Hello young lad. How are you on this fine evening?\n\n\n  Great\n  Horrible";
		toombaDialogue[1] = "That is great to hear. I was wondering if you would like engage in a little trade.\n\n  Yes\n  No";
		toombaDialogue[2] = "WELL YOU KNOW WHAT YOU DuhMB WahNKa, YOU'RE JUST A bahMY BLuhDY Sh-chuPID IDIOT.\n  Calm down\n  Screw off!";
		toombaDialogue[3] = "That sounds like a good idea. I was wondering if you would like engage in a little trade.\n  Yes\n  No";
		toombaDialogue[4] = "How about you give me 20 coins and I give you a star! It's a blimey good deal!\n\n  Yes\n  No";
		toombaDialogue[5] = "THEN SCREW YOU DuhMB MuhPPehT! I DON'T WANT TO DO BUISNESS WITH YOU nohBBIN. PISS ohFF!\n\n  Leave";
		toombaDialogue[6] = "YOU DUMB MuhPPehT! YOU LACK THE AMOUNT OF CURRENCY NEEDED!!!! PISS ohFF\n\n  Leave";
		toombaDialogue[7] = "Excellent, I will process the payment pronto!\n\n\n\n  Pay Toomba";
		toombaDialogue[8] = "Thank you for your purchase. I look forward to future buisness.\n\n\n  Exit";

		obj_dialogueBox.Init(toombaDialogue, new List<int>() {5, 6, 7, 8}, new Callable(this, "ChangeDialogue"), spr_characterSprite);
		obj_dialogueBox.setDelay = 2.3;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(dialogue)
			obj_dialogueBox.ProcessDialogue(delta);

		if(buyingStar)
			BuyStar(delta);
	}

	private int ChangeDialogue(int index, int _dialogueIndex)
	{
		int dialogueIndex = _dialogueIndex;
		if(index == 0)
		{
			switch(dialogueIndex)
			{
				case 0:
					dialogueIndex = 1;
					break;
				case 1:
					dialogueIndex = 4;
					break;
				case 2:
					dialogueIndex = 3;
					break;
				case 3:
					dialogueIndex = 4;
					break;
				case 4:
					if(playerData.coins >= 20)
						dialogueIndex = 7;
					else
						dialogueIndex = 6;
					break;
			}
		}
		else if(index == 1)
		{
			switch(dialogueIndex)
			{
				case 0:
					dialogueIndex = 2;
					break;
				case 1:
					dialogueIndex = 5;
					break;
				case 2:
					dialogueIndex = 5;
					break;
				case 3:
					dialogueIndex = 5;
					break;
				case 5:
					interactionEnd.Call(false);
					break;
				case 6:
					interactionEnd.Call(false);
					break;
				case 7:
					if(!buyingStar)
						buyingStar = true;
					break;
				case 8:
					GetNode<Transition>("../obj_mapGUI/Transition").transitionEnd = new Callable(this, "ShowNextStar");
					GetNode<Transition>("../obj_mapGUI/Transition").state = 5;
					//interactionEnd.Call(true);
					break;
			}
		}

		return dialogueIndex;
	}

	public void ShowDialogue(PlayerData _playerData, Callable _interactionEnd)
	{
		obj_dialogueBox.dialogueIndex = 0;
		obj_dialogueBox.Visible = true;
		playerData = _playerData;
		interactionEnd = _interactionEnd;
		coinsTaken = 0;
		obj_dialogueBox.controllerIndex = playerData.controllerIndex;

		buyingStar = false;

		dialogue = true;
	}

	public void HideDialogue()
	{
		obj_dialogueBox.Visible = false;
	}

	private void BuyStar(double delta)
	{
		delay -= delta;
		if(delay <= 0)
		{
			delay = 0.08;
			if(coinsTaken < 20)
			{
				coinsTaken++;
				playerData.coins--;
				((AudioController)GetNode("/root/AudioController")).PlaySound("minigame_coin");
			}
			else
			{
				if(!anim_starGet.IsPlaying())
					{
					anim_starGet.Play("starget");
					((AudioController)GetNode("/root/AudioController")).PlaySound("starCollect");
					((AudioController)GetNode("/root/AudioController")).MusicEffect("volume", -100);
				}
				buyingStar = false;
			}
		}
	}

	private void GiveStar()
	{
		playerData.stars++;
	}

	private void PlaySound()
	{
		((AudioController)GetNode("/root/AudioController")).PlaySound("starGet");
	}

	public void ShowNextStar()
	{
		
	}

	private void FinishStarAnimation()
	{
		obj_dialogueBox.dialogueIndex = 8;
		((AudioController)GetNode("/root/AudioController")).MusicEffect("volume", 0);
	}
}
