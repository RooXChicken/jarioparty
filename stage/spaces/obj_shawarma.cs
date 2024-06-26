using Godot;
using System;
using System.Collections.Generic;

public partial class obj_shawarma : Node2D
{
	private AnimatedSprite2D spr_characterSprite;
	private obj_dialogueController obj_dialogueBox;
	private Callable interactionEnd;
	private PlayerData[] playerData = new PlayerData[4];

	private bool dialogue = true;
	private bool frozen = false;

	public int diceRoll = 4;
	private int type = 1;

	private int coinsTaken = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		if(((GameManager)GetNode("/root/GameManager")).playerData[0].PlayerStarted && ((GameManager)GetNode("/root/GameManager")).TurnsMax-((GameManager)GetNode("/root/GameManager")).TurnNumber != 5 && ((GameManager)GetNode("/root/GameManager")).TurnsMax+1 != ((GameManager)GetNode("/root/GameManager")).TurnNumber)
		{
			QueueFree();
			return;
		}

		GetNode<AnimationPlayer>("../obj_mapGUI/anim_transition").Play("transition");
		GetNode<obj_mapGUI>("../obj_mapGUI").HideWallets();
		playerData = ((GameManager)GetNode("/root/GameManager")).playerData;
		obj_dialogueBox = GetNode<obj_dialogueController>("obj_dialogueController");
		obj_dialogueBox.setDelay = 4;
		spr_characterSprite = GetNode<AnimatedSprite2D>("spr_shawarma");

		string[] shawarmaDialogue = new string[14];
		if(!((GameManager)GetNode("/root/GameManager")).playerData[0].PlayerStarted)
		{
			shawarmaDialogue[0] = "Hi sisters! today we r going to be hosting a LIT ASF party! It's finna be fire!\n\n\n  Continue";
			shawarmaDialogue[1] = "NGL, I'm hella hyped. What bout' u fam?\n\n\n  Yes\n  No";
			shawarmaDialogue[2] = "JK IDC what u think cuz low-key ur just salty.\n\n\n\n  Continue";
			shawarmaDialogue[3] = "K it do be dat time. the time to YEET some dice up and see who is based or nah. u ready fam?\n\n  Continue";
			shawarmaDialogue[4] = "hi";
			shawarmaDialogue[5] = "Smash da A button on ur dope asf controller, let's roll some MFing DIIIIIIIIICE!\n\n\n  <Press A to roll>";
			shawarmaDialogue[6] = "Oooh baby smack those dice!\n\n\n\n  Continue";
			shawarmaDialogue[7] = "This message should not appear. If it does, I am silly";
			shawarmaDialogue[8] = "This message should not appear. If it does, I am silly";
			shawarmaDialogue[9] = "This message should not appear. If it does, I am silly";
			shawarmaDialogue[10] = "This message should not appear. If it does, I am silly";
			shawarmaDialogue[11] = "Since I am ballin, I finna give u all 10 coins to start.\n\n\n\n  Continue";
			shawarmaDialogue[12] = "OKIII enjoy the dope party. Bye felicia!\n\n\n\n  Exit";
		}
		else if(((GameManager)GetNode("/root/GameManager")).TurnsMax-((GameManager)GetNode("/root/GameManager")).TurnNumber == 5)
		{
			type = 2;
			shawarmaDialogue[0] = "Hi sisters! the game is almost over!!!!\n\n\n  Continue";
			shawarmaDialogue[1] = "hav you been enjoying the epic game?\n\n\n  Yes\n  No";
			shawarmaDialogue[2] = "that's lit yo. with 5 turns left i'm finna give the last player some goodies\n\n\n  Continue";
			shawarmaDialogue[3] = "this should make you feel better about yourself!\n\n\n  Continue";
			shawarmaDialogue[4] = "hi";
			shawarmaDialogue[5] = "enjoy the rest of the party!!!!!\n\n\n  Exit";
		}
		else if(((GameManager)GetNode("/root/GameManager")).TurnsMax+1 == ((GameManager)GetNode("/root/GameManager")).TurnNumber)
		{
			type = 3;
			for(int i = 0; i < 4; i++)
				playerData[i].PlayerStarted = false;
			shawarmaDialogue[0] = "Hi sisters! the parti is over!!\n\n\n  Continue";
			shawarmaDialogue[1] = "i really hope u all loved it. did you love it???\n\n\n  Yes\n  No";
			shawarmaDialogue[2] = "awesome! just wanted to make sure!!!\n\n\n  Continue";
			shawarmaDialogue[3] = "it's time to announce the winners!!!\n\n\n  Continue";
			shawarmaDialogue[4] = "BUT FIRST!! the bonus stars! let's hope you all have been keeping track of every last detail!\n\n  Continue";
			shawarmaDialogue[5] = "for the category of 'Spaces Traveled', we have a winner!\n\n\n  Continue";
			shawarmaDialogue[7] = "next up, for the category of 'Coins Collected', we have a winner!\n\n\n  Continue";
			shawarmaDialogue[9] = "lastly, for the category of 'Minigames Won', we have a winner!\n\n\n  Continue";
			shawarmaDialogue[11] = "good job besties!!! we will now reveal who the most epic gamer here is!\n\n  Continue";
			shawarmaDialogue[13] = "so proud bestie. well that concludes this party! come again!!!!\n\n  Continue";
		}

		obj_dialogueBox.Init(shawarmaDialogue, new List<int>() {0, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13}, new Callable(this, "ChangeDialogue"), spr_characterSprite);
		ShowDialogue();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("skip"))
		{
			for(int i = 0; i < 4; i++)
					playerData[i].PlayerStarted = true;

				//GetNode<obj_map>("../").Initialize();
				((GameManager)GetNode("/root/GameManager")).SwitchScene("rm_map");
		}
		if(dialogue)
		{
			obj_dialogueBox.ProcessDialogue(delta);
			if(obj_dialogueBox.dialogueIndex == 5)
			{
				if(diceRoll <= 0)
				{
					obj_dialogueBox.ChangeDialogue(6);
					((AudioController)GetNode("/root/AudioController")).StopSound("diceblockRoll");

					//PlayerData[] sort = playerData;
					for(int k = 0; k < 16; k++)
					{
						for(int i = 1; i < 4; i++)
						{
							if(playerData[i-1].diceRoll < playerData[i].diceRoll)
							{
								PlayerData p0 = playerData[i-1];
								PlayerData p1 = playerData[i];

								playerData[i-1] = p1;
								playerData[i] = p0;
							}
						}
					}

					obj_dialogueBox.characterDialogue[7] = "K so " + playerData[0].characterName + " gets to go first! Yass queen!\n\n\n\n  Continue";
					obj_dialogueBox.characterDialogue[8] = "K so " + playerData[1].characterName + " gets to go second! Good job bestie!!\n\n\n\n  Continue";
					obj_dialogueBox.characterDialogue[9] = "K so " + playerData[2].characterName + " gets to go third! Mid roll.\n\n\n\n  Continue";
					obj_dialogueBox.characterDialogue[10] = "K so " + playerData[3].characterName + " gets to go fourth! L LOSER GET FORTNITED ON XD!\n\n\n  Continue";

					for(int i = 0; i < 4; i++)
						playerData[i].playerOrder = i + 1;

					GetNode<obj_mapGUI>("../obj_mapGUI").Initialize();
				}
			}
		}
	}

	private int ChangeDialogue(int index, int _dialogueIndex)
	{
		int dialogueIndex = _dialogueIndex;
		if(type == 1)
		{
			if(index == 0)
			{
				dialogueIndex = 2;
			}
			else if(index == 1)
			{
				if(dialogueIndex != 5)
					dialogueIndex++;
				if(dialogueIndex == 4)
				{
					((AudioController)GetNode("/root/AudioController")).PlaySound("diceblockRoll");
					for(int i = 0; i < 4; i++)
					{
						GetNode<obj_diceBlock>("../Setup/obj_diceBlock" + (i+1)).playSound = false;
						GetNode<obj_diceBlock>("../Setup/obj_diceBlock" + (i+1)).Initialize();
						GetNode<obj_diceBlock>("../Setup/obj_diceBlock" + (i+1)).a_spinStart(GetNode<obj_character_map>("../Paths/pt_01/pf_0" + (i+1) + "/obj_character_map"));
						GetNode<obj_character_map>("../Paths/pt_01/pf_0" + (i+1) + "/obj_character_map").canJump = true;
						GetNode<obj_character_map>("../Paths/pt_01/pf_0" + (i+1) + "/obj_character_map").isTurn = true;
						GetNode<obj_character_map>("../Paths/pt_01/pf_0" + (i+1) + "/obj_character_map").state = 1;
						GetNode<obj_character_map>("../Paths/pt_01/pf_0" + (i+1) + "/obj_character_map").EnableCollision();

						if(playerData[i].controllerIndex != -1)
							Input.ActionRelease("jump" + playerData[i].controllerIndex);
					}

					dialogueIndex = 5;
				}
				if(dialogueIndex >= 7 && dialogueIndex <= 10)
				{
					GetNode<obj_mapGUI>("../obj_mapGUI").ShowWallet(dialogueIndex-6);
				}
				if(dialogueIndex == 11)
				{
					for(int i = 0; i < 4; i++)
						playerData[i].coins += 10;
				}
				if(dialogueIndex == 13)
				{
					for(int i = 0; i < 4; i++)
						playerData[i].PlayerStarted = true;

					//GetNode<obj_map>("../").Initialize();
					((GameManager)GetNode("/root/GameManager")).SwitchScene("rm_map");
				}
			}
		}

		if(type == 2)
		{
			if(index == 0)
			{
				dialogueIndex = 2;
			}
			else if(index == 1)
			{
				if(dialogueIndex == 5)
				{
					for(int i = 0; i < 4; i++)
						playerData[i].PlayerStarted = true;
					((GameManager)GetNode("/root/GameManager")).SwitchScene("rm_map");
				}
				dialogueIndex++;
			}
		}

		if(type == 3)
		{
			if(index == 0)
			{
				dialogueIndex = 2;
			}
			else if(index == 1)
			{
				if(dialogueIndex == 5)
				{
					PlayerData[] sort = playerData;
					for(int k = 0; k < 16; k++)
					{
						for(int i = 1; i < 4; i++)
						{
							if(sort[i-1].spacesTraveled < sort[i].spacesTraveled)
							{
								PlayerData p0 = sort[i-1];
								PlayerData p1 = sort[i];

								sort[i-1] = p1;
								sort[i] = p0;
							}
						}
					}

					obj_dialogueBox.characterDialogue[6] = "the player who traveled the most spaces is " + sort[0].characterName + " with " + sort[0].spacesTraveled + " spaces traveled!!!\n\n  Continue";
					playerData[sort[0].playerOrder-1].stars++;
				}
				if(dialogueIndex == 7)
				{
					PlayerData[] sort = playerData;
					for(int k = 0; k < 16; k++)
					{
						for(int i = 1; i < 4; i++)
						{
							if(sort[i-1].coins < sort[i].coins)
							{
								PlayerData p0 = sort[i-1];
								PlayerData p1 = sort[i];

								sort[i-1] = p1;
								sort[i] = p0;
							}
						}
					}

					obj_dialogueBox.characterDialogue[8] = "the player who collected the most coins is " + sort[0].characterName + " with " + sort[0].coins + " coins!!!\n\n  Continue";
					playerData[sort[0].playerOrder-1].stars++;
				}
				if(dialogueIndex == 9)
				{
					PlayerData[] sort = playerData;
					for(int k = 0; k < 16; k++)
					{
						for(int i = 1; i < 4; i++)
						{
							if(sort[i-1].minigamesWon < sort[i].minigamesWon)
							{
								PlayerData p0 = sort[i-1];
								PlayerData p1 = sort[i];

								sort[i-1] = p1;
								sort[i] = p0;
							}
						}
					}

					obj_dialogueBox.characterDialogue[10] = "the player who won the most minigames is " + sort[0].characterName + " with " + sort[0].minigamesWon + " wins!!!\n\n  Continue";
					playerData[sort[0].playerOrder-1].stars++;
				}
				if(dialogueIndex == 11)
				{
					PlayerData[] sort = playerData;
					for(int k = 0; k < 16; k++)
					{
						for(int i = 1; i < 4; i++)
						{
							if(sort[i-1].stars < sort[i].stars)
							{
								PlayerData p0 = sort[i-1];
								PlayerData p1 = sort[i];

								sort[i-1] = p1;
								sort[i] = p0;
							}
						}
					}

					obj_dialogueBox.characterDialogue[12] = "the winner is.......... " + sort[0].characterName + " with " + sort[0].stars + " stars!!! GOOD JOB!!!!!!!\n\n  Continue";
				}

				if(dialogueIndex == 13)
				{
					((GameManager)GetNode("/root/GameManager")).SwitchScene("rm_credits");
				}
				dialogueIndex++;
			}
		}

		return dialogueIndex;
	}

	public void ShowDialogue()
	{
		obj_dialogueBox.Visible = true;
		coinsTaken = 0;
		
		obj_dialogueBox.controllerIndex = playerData[0].controllerIndex;
		obj_dialogueBox.SetText("");
		
		dialogue = true;
	}

	public void HideDialogue()
	{
		obj_dialogueBox.Visible = false;
	}
}
