using Godot;
using System;

public partial class obj_GREENGUY : Sprite2D
{
	private Random rand = new Random();

	private new void FrameChanged()
	{
		Visible = false;
		if(rand.Next(0, 1000) == 2) //this is a reference
		{
			Visible = true;
			Position = new Vector2(rand.Next(0, 1280), rand.Next(0, 720));
		}
	}
}
