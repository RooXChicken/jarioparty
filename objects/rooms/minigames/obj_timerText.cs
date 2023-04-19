using Godot;
using System;

public partial class obj_timerText : RichTextLabel
{
	public int Time = 110;
	private float x = 2.75f;

	public override void _Ready()
	{
		GetNode<AnimatedSprite2D>("../").Play();
	}

	private void FrameChanged()
	{
		Vector2 newPos;
		switch(GetNode<AnimatedSprite2D>("../").Frame)
		{
			case 0:
				Time--;
				newPos = new Vector2(x, -9.5f);
				break;
			case 1:
				newPos = new Vector2(x, -10.5f);
				break;

			default:
				newPos = new Vector2(x, -11.5f);
				break;
		}

		Position = newPos;
		Text = Time.ToString();
	}
}
