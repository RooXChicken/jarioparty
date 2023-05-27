using Godot;
using System;
using System.Collections.Generic;

public partial class obj_handManager : Node2D
{
	private List<RigidBody2D> hands;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		hands = new List<RigidBody2D>();
		for(int i = 1; i <= 4; i++)
			hands.Add(GetNode<RigidBody2D>("obj_playerHand" + i));
	}

	public void HideLights()
	{
		foreach(RigidBody2D hand in hands)
		{
			hand.GetNode<Light2D>("lgt_hand").Visible = false;
		}
	}
	public void ShowLights()
	{
		foreach(RigidBody2D hand in hands)
		{
			hand.GetNode<Light2D>("lgt_hand").Visible = true;
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
