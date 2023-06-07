using Godot;
using System;
using System.Collections.Generic;

public partial class MinigameBase : Node
{
	public List<string> Abilities;

	public int MinigameIndex;
	public int MinigameType;
	public string MinigameName;
	public string MinigameRoom;
	public string Description;
	public int MinigameTime;
	public string Music;
	//public Node2D[] Buttons;
	public int[] ButtonIndexs;
	public int[] UsableItems;
	public float Scale;

	public MinigameBase(int _MinigameIndex, int _MinigameType, string _MinigameName, string _MinigameRoom, string _Description, List<string> _Abilities, string _Music = "", int _MinigameTime = 60, int[] _ButtonIndexs = null, int[] _UsableItems = null, float _Scale = 3)
	{
		MinigameIndex = _MinigameIndex;
		MinigameType = _MinigameType;

		MinigameName = _MinigameName;
		MinigameRoom = _MinigameRoom;
		Description = _Description;
		Abilities = _Abilities;
		Music = _Music;
		ButtonIndexs = _ButtonIndexs;

		MinigameTime = _MinigameTime;

		if(_UsableItems != null)
			UsableItems = _UsableItems;

		Scale = _Scale;
	}

	public MinigameBase(MinigameBase _Clone) : this(_Clone.MinigameIndex, _Clone.MinigameType, _Clone.MinigameName, _Clone.MinigameRoom, _Clone.Description, _Clone.Abilities, _Clone.Music, _Clone.MinigameTime, _Clone.ButtonIndexs, _Clone.UsableItems, _Clone.Scale) {}
}
