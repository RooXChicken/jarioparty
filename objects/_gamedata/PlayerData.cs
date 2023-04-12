using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerData : Node
{
	public short controllerIndex = 1; //1 = player 1
	public ushort characterIndex = 1; //1 = jario; 2 = wooigi; 3 = grape; 4 = josh
	public string characterName = "NULL";
	public bool ai = false;

	//game data
	public ushort coins = 0;
	public ushort stars = 0;

	public float speedMult = 1; //speed
	public float weightMult = 1; //density
	public float strengthMult = 1; //punch strength

	public List<ItemBase> items;
	public SpriteFrames animationFrames;

	public PlayerData(short _controllerIndex, ushort _characterIndex, bool _ai = false)
	{
		controllerIndex = _controllerIndex;
		characterIndex = _characterIndex;
		characterName = GetCharacterNameFromIndex();
		ai = _ai;
		items = new List<ItemBase>();
	}

	private string GetCharacterNameFromIndex()
	{
		switch(characterIndex)
		{
			case 0:
				coins = 420;
				stars = 69;
				return "Jario";
			case 1:
				coins = 0;
				stars = 0;
				return "Wooigi";
			case 2:
				coins = 9523;
				stars = 97;
				return "Grape";
			case 3:
				coins = 17;
				stars = 4;
				return "Josh";

			default:
				return "NULL";
		}
	}

	public void Load()
	{
		animationFrames = _GetSpriteFrames();
	}

	private SpriteFrames _GetSpriteFrames()
	{
		SpriteFrames spr = null;
		switch(characterIndex)
		{
			case 0:
				spr = GD.Load<SpriteFrames>("res://sprites/characters/playable/spr_jario.tres");
				break;
			case 1:
				spr = GD.Load<SpriteFrames>("res://sprites/characters/playable/spr_wooigi.tres");
				break;
			case 2:
				spr = GD.Load<SpriteFrames>("res://sprites/characters/playable/spr_grape.tres");
				break;
			case 3:
				spr = GD.Load<SpriteFrames>("res://sprites/characters/playable/spr_josh.tres");
				break;
		}

		return spr;
	}
}
