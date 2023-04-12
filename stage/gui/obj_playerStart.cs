using Godot;
using System;

public partial class obj_playerStart : Node2D
{
	private Sprite2D spr_charName;
	private Sprite2D spr_start;

	private Texture2D[] playerNames;
	//private Vector2[] startPositions;
	private Vector3[] startColors;
	private Alarm t_vanish;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerNames = new Texture2D[] {
			GD.Load<Texture2D>("res://sprites/gui/roundStart/spr_jario.png"), GD.Load<Texture2D>("res://sprites/gui/roundStart/spr_wooigi.png"), GD.Load<Texture2D>("res://sprites/gui/roundStart/spr_grapejuice.png"), GD.Load<Texture2D>("res://sprites/gui/roundStart/spr_josh.png")
		};

		startColors = new Vector3[] {
			new Vector3(239 / 255f, 228 / 255f, 176 / 255f), new Vector3(112 / 255f, 146 / 255f, 190 / 255f), new Vector3(163 / 255f, 73 / 255f, 164 / 255f), new Vector3(1, 174 / 255f, 201 / 255f)
		};

		spr_charName = GetNode<Sprite2D>("spr_charName");
		spr_start = GetNode<Sprite2D>("spr_start");

		t_vanish = new Alarm(1.5, true, this, new Callable(this, "Vanish"), false);
	}

	public void StartAnimation(int _characterIndex)
	{
		Modulate = new Color(1, 1, 1, 1);
		spr_charName.Texture = playerNames[_characterIndex - 1];
		spr_start.Position = new Vector2((playerNames[_characterIndex - 1].GetWidth() / 1.5f) + 760, 345);
		((ShaderMaterial)spr_start.Material).SetShaderParameter("input", startColors[_characterIndex - 1]);
		t_vanish.Start();
	}

	public void Vanish()
	{
		Modulate = new Color(1, 1, 1, 0);
	}
}
