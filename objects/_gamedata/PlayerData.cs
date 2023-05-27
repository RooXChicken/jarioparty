using Godot;
using System;
using System.Collections.Generic;

public partial class PlayerData : Node
{
	public short controllerIndex = 1; //1 = player 1
	public ushort characterIndex = 1; //1 = jario; 2 = wooigi; 3 = grape; 4 = josh
	public string characterName = "NULL";
	public bool ai = false;
	public int diceRoll = 0;

	//game data
	public short coins = 0;
	public short stars = 0;

	public float speedMult = 1; //speed
	public float weightMult = 1; //density
	public float strengthMult = 1; //punch strength

	public float progress = 0.1765f;
	public bool PlayerStarted = false;
	public Color PlayerColor;

	public List<ItemBase> items;
	public SpriteFrames animationFrames;

	public PlayerData(short _controllerIndex, ushort _characterIndex, bool _ai = false)
	{
		controllerIndex = _controllerIndex;
		characterIndex = _characterIndex;
		characterName = GetCharacterNameFromIndex();
		ai = _ai;
		items = new List<ItemBase>();

		Vector3[] startColors = new Vector3[] {
			new Vector3(239 / 255f, 228 / 255f, 176 / 255f), new Vector3(112 / 255f, 146 / 255f, 190 / 255f), new Vector3(163 / 255f, 73 / 255f, 164 / 255f), new Vector3(1, 174 / 255f, 201 / 255f)
		};

		PlayerColor = new Color(startColors[characterIndex].X, startColors[characterIndex].Y, startColors[characterIndex].Z, 1);
	}

	private string GetCharacterNameFromIndex()
	{
		switch(characterIndex)
		{
			case 0:
				return "Jario";
			case 1:
				return "Wooigi";
			case 2:
				return "Grape";
			case 3:
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
		spr = GD.Load<SpriteFrames>("res://sprites/characters/playable/spr_" + characterName.ToLower() + ".tres");

		return spr;
	}
}
