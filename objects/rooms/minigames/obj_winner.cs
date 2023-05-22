using Godot;
using System;
using System.Collections.Generic;

public partial class obj_winner : Node
{
	private Sprite2D[] spr_characterName;
	private Sprite2D spr_wins;
	private AnimationPlayer anim_wins;

	private List<PlayerData> places;
	private int[] coinVal;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spr_characterName = new Sprite2D[3];
		for(int i = 0; i < 3; i++)
		{
			spr_characterName[i] = GetNode<Sprite2D>("spr_characterName" + (i+1));
		}
		spr_wins = GetNode<Sprite2D>("spr_wins");
		anim_wins = GetNode<AnimationPlayer>("anim_wins");
	}

	public void EndMiniGame(int size, int[] playersAlive, List<PlayerData> _places, int[] _coinVal)
	{
		((GameManager)GetNode("/root/GameManager")).MinigameStarted = false;
		switch(size)
		{
			case 0:
				anim_wins.Play("miss");
				break;
			case 1:
				spr_characterName[1].Texture = ((GameManager)GetNode("/root/GameManager")).PlayerNameImages[playersAlive[0]];
				spr_wins.Position = new Vector2(640, 450);
				anim_wins.Play("wins1");
				break;
			case 2:
				spr_characterName[0].Texture = ((GameManager)GetNode("/root/GameManager")).PlayerNameImages[playersAlive[0]];
				spr_characterName[2].Texture = ((GameManager)GetNode("/root/GameManager")).PlayerNameImages[playersAlive[1]];
				spr_wins.Position = new Vector2(640, 450);
				anim_wins.Play("wins2");
				break;
			case 3:
				spr_characterName[0].Texture = ((GameManager)GetNode("/root/GameManager")).PlayerNameImages[playersAlive[0]];
				spr_characterName[1].Texture = ((GameManager)GetNode("/root/GameManager")).PlayerNameImages[playersAlive[1]];
				spr_characterName[2].Texture = ((GameManager)GetNode("/root/GameManager")).PlayerNameImages[playersAlive[2]];
				spr_wins.Position = new Vector2(640, 450);
				anim_wins.Play("wins3");
				break;
		}

		places = _places;
		coinVal = _coinVal;
	}

	public void ShowResults()
	{
		GetNode<obj_leaderboard>("../WinGUI").StartSequence(places, coinVal);
	}
}
