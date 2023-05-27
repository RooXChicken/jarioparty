using Godot;
using System;
using System.Collections.Generic;

public partial class MinigameBase
{
	public List<string> Abilities;

	public int MinigameIndex;
	public string MinigameName;
	public string Description;
	public int MinigameTime;
	public string Music;
	public Node2D[] Buttons;
	public int[] ButtonIndexs;
	public int[] UsableItems;
	public float Scale;

	public MinigameBase(int _MinigameIndex, string _MinigameName, string _Description, List<string> _Abilities, string _Music = "", int _MinigameTime = 60, int[] _ButtonIndexs = null, int[] _UsableItems = null, float _Scale = 3)
	{
		MinigameIndex = _MinigameIndex;

		MinigameName = _MinigameName;
		Description = _Description;
		Abilities = _Abilities;
		Music = _Music;

		MinigameTime = _MinigameTime;

		if(_ButtonIndexs != null)
		{
			ButtonIndexs = _ButtonIndexs;
			Buttons = new Node2D[ButtonIndexs.Length];
			for(int i = 0; i < Buttons.Length; i++)
			{
				Buttons[i] = GD.Load<PackedScene>("res://objects/common/buttons/obj_button.tscn").Instantiate<Node2D>();
				Buttons[i].Name = "obj_button" + i;
				Buttons[i].Position = new Vector2(646, 536 + (27 * i));
				Buttons[i].Scale = new Vector2(0.15f, 0.15f);
				Buttons[i].SetMeta("Button", ButtonIndexs[i]);
			}
		}

		if(_UsableItems != null)
			UsableItems = _UsableItems;

		Scale = _Scale;
	}

	public MinigameBase(MinigameBase _Clone) : this(_Clone.MinigameIndex, _Clone.MinigameName, _Clone.Description, _Clone.Abilities, _Clone.Music, _Clone.MinigameTime, _Clone.ButtonIndexs, _Clone.UsableItems, _Clone.Scale) {}
}
