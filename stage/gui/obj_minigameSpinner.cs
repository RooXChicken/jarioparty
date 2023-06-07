using Godot;
using System;
using System.Collections.Generic;

public partial class obj_minigameSpinner : Node2D
{
	private Node2D spr_arrows;
	private float[] positions;
	private int index = 0;
	private Random rand;
	List<MinigameBase> minigameSelection;

	private bool spinning = false;
	private double mult = 0;
	private double setDelay = 0.001;
	private double delay = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://stage/sound/snd_spin.wav", "spinner_spin");
		minigameSelection = new List<MinigameBase>();

		spr_arrows = GetNode<Node2D>("SelectionArrows");
		positions = new float[] {
			0, 110, 220
		};

		rand = new Random();
		List<MinigameBase> minigames = new List<MinigameBase>();

		int redColors = 0;
		int blueColors = 0;

		for(int i = 0; i < 4; i++)
		{
			if(((GameManager)GetNode("/root/GameManager")).playerData[i].spaceColor == 0)
				blueColors++;
			else if(((GameManager)GetNode("/root/GameManager")).playerData[i].spaceColor == 1)
				redColors++;
			else
			{
				if(rand.Next(0, 2) == 0)
					blueColors++;
				else
					redColors++;
			}
		}

		GD.Print("Blue:" + blueColors + " Red: " + redColors);

		int added = 0;

		foreach(MinigameBase minigame in ((GameManager)GetNode("/root/GameManager")).minigameLookup)
		{
			if((minigame.MinigameType == blueColors || minigame.MinigameType == redColors) && minigame.MinigameName != "testminigame")
			{
				added++;
				minigames.Add(minigame);
			}
		}

		foreach(MinigameBase minigame in ((GameManager)GetNode("/root/GameManager")).minigameLookup)
		{
			if(added >= 3)
				break;
			
			if(!minigames.Contains(minigame) && minigame.MinigameType == 4 && minigame.MinigameName != "testminigame")
			{
				added++;
				minigames.Add(minigame);
			}
		}

		for(int i = 1; i <= 3; i++)
		{
			MinigameBase game = minigames[rand.Next(0, minigames.Count)];
			GetNode<RichTextLabel>("spr_spinner/spr_text" + i + "/obj_text").Text = "[center]" + game.MinigameName;
			minigames.Remove(game);
			minigameSelection.Add(game);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(spinning)
		{
			delay -= delta;
			mult += delta;
			if(delay <= 0)
			{
				((AudioController)GetNode("/root/AudioController")).PlaySound("spinner_spin");
				setDelay += 0.4 * delta;
				delay = setDelay;

				index++;

				if(index > 2)
					index = 0;
			}

			if(mult > 3 + (rand.NextDouble() * 2))
			{
				spinning = false;
				((GameManager)GetNode("/root/GameManager")).CurrentMinigame = minigameSelection[index].MinigameIndex;
			}
		}

		spr_arrows.Position = new Vector2(0, positions[index]);
	}

	public void Spin()
	{
		spinning = true;
	}
}
