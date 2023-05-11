using Godot;
using System;
using System.Collections.Generic;

public partial class obj_winner : Node
{
	private Sprite2D spr_characterName;
	private Sprite2D spr_wins;
	private AnimationPlayer anim_wins;

	private List<PlayerData> places;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spr_characterName = GetNode<Sprite2D>("spr_characterName");
		spr_wins = GetNode<Sprite2D>("spr_wins");
		anim_wins = GetNode<AnimationPlayer>("anim_wins");
	}

	public void EndMiniGame(int playersAlive, int playerAliveIndex, List<PlayerData> _places)
	{
		((GameManager)GetNode("/root/GameManager")).MinigameStarted = false;
		if(playersAlive == 1)
		{
			//obj_character_parent winner = null;
			// for(int i = 0; i < 4; i++)
			// 	if(!players[i].Lost)
			// 		winner = players[i];
			
			spr_characterName.Texture = ((GameManager)GetNode("/root/GameManager")).PlayerNameImages[playerAliveIndex];
			spr_wins.Position = new Vector2(640, 450);
			anim_wins.Play("wins");
		}
		else
		{
			anim_wins.Play("miss");
		}

		places = _places;
	}

	public void ShowResults()
	{
		GetNode<obj_leaderboard>("../WinGUI").StartSequence(places);
	}
}
