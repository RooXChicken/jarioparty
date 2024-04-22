using Godot;
using System;

public partial class cm_fadeAlpha : Sprite2D
{
	private float start = 0f; //starting alpha
	private float end = 1f; //ending alpha
	private float alpha = 0f; //current alpha
	private bool state = false; //false = increasing, true = decreasing (bool to save memory)
	public float alphaMultiplier = 1.5f; //multiplier
	private bool loop = false;

	public override void _Ready()
	{
		if(HasMeta("Start"))
		{
			Variant _start = GetMeta("Start");
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

		if(HasMeta("Loop"))
		{
			Variant _loop = GetMeta("Loop");
			loop = _loop.As<bool>();
		}

		Modulate = new Color(1, 1, 1, alpha);
	}

	public override void _Process(double delta)
	{
		if(!Visible)
			return;
		// Variant _start = GetMeta("Start");
		// start = _start.As<float>();
		
		// Variant _end = GetMeta("End");
		// end = _end.As<float>();

		// Variant _mult = GetMeta("Multiplier");
		// alphaMultiplier = _mult.As<float>();
		if(!state)
		{
			alpha += alphaMultiplier * (float)delta;
			if(alpha > end)
			{
				alpha = end;
				state = true;
			}
		}
		else if(state && loop)
		{
			alpha -= alphaMultiplier * (float)delta;
			if(alpha < start)
			{
				alpha = start;
				state = false;
			}
		}

		Modulate = new Color(1, 1, 1, alpha);
	}
}
