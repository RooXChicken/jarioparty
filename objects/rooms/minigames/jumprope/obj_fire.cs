using Godot;
using System;

public partial class obj_fire : AnimatedSprite2D
{
	private float speed = 1;
	private AnimatedSprite2D spr_bg;
	private obj_character_parent[] players;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).MusicEffect("pitch", 0.93f + (speed * 0.06f));
		spr_bg = GetNode<AnimatedSprite2D>("../spr_bg");

		players = new obj_character_parent[4];
		for(int i = 0; i < 4; i++)
		{
			players[i] = (obj_character_parent)GetNode<RigidBody2D>("../Players/obj_character_parent" + (i + 1));
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	private void FrameChanged()
	{
		if(Frame == 9)
		{
			speed += 0.03f;

			SpeedScale = speed;
			spr_bg.SpeedScale = speed;
			((AudioController)GetNode("/root/AudioController")).MusicEffect("pitch", 0.93f + (speed * 0.06f));
		}
		if(Frame >= 8 && Frame <= 10)
		{
			((ShaderMaterial)Material).SetShaderParameter("canDie", true);
			ZIndex = 1;

			for(int i = 0; i < 4; i++)
			{
				if(players[i].Position.Y > 450)
				{
					players[i].Burn();
				}
			}
		}
		else if(Frame == 11)
			((ShaderMaterial)Material).SetShaderParameter("canDie", false);
		else if(Frame == 15)
		{
			ZIndex = 3;
		}
	}

}
