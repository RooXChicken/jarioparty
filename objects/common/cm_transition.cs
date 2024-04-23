using Godot;
using System;
using System.Diagnostics;

public partial class cm_transition : Sprite2D
{
	private float start = 0f; //starting alpha
	private float end = 1f; //ending alpha
	private float alpha = 0f; //current alpha
	public float alphaMultiplier = 1.5f; //multiplier
	public string nextRoom = "";
	public bool done = false;
	Alarm t_nextRoom;

	public override void _Ready()
	{
		if(HasMeta("start"))
		{
			Variant _start = GetMeta("start");
			start = _start.As<float>();
		}
		
		if(HasMeta("End"))
		{
			Variant _end = GetMeta("End");
			end = _end.As<float>();
		}

		if(HasMeta("Multiplier"))
		{
			Variant _mult = GetMeta("Multiplier");
			alphaMultiplier = _mult.As<float>();
		}

		if(HasMeta("NextRoom"))
		{
			Variant _nextRoom = GetMeta("NextRoom");
			nextRoom = _nextRoom.As<String>();
		}

		alpha = start;
		Modulate = new Color(0, 0, 0, alpha);
	}

	public override void _Process(double delta)
	{
		if(!Visible)
			return;
		
		alpha += alphaMultiplier * (float)delta;
		alpha = Math.Clamp(alpha, 0, 1);

		Modulate = new Color(0, 0, 0, alpha);

		if(alpha == end)
		{
			done = true;
			t_nextRoom = new Alarm(0.5, true, this, new Callable(this, "NextRoom"));
		}
	}

	public void NextRoom()
	{
		if(nextRoom != "")
			((GameManager)GetNode("/root/GameManager")).SwitchScene(nextRoom);
	}
}
