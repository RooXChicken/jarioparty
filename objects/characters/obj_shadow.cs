using Godot;
using System;

public partial class obj_shadow : Sprite2D
{
	private RigidBody2D player;

	public float floor;
	public float pPos;
	private float offset = 312;
	private float shadOffset = -1.4f;
	private float posOffset = -48;

	private Vector2 prevPos;

	public override void _Ready()
	{
		player = GetNode<RigidBody2D>("../../");
		prevPos = player.Position;
		floor = player.Position.Y;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position -= player.Position - prevPos;
		prevPos = player.Position;

		//posOffset += (float)delta * (5 * Input.GetAxis("left1", "right1")) * 25;
		//GD.Print(posOffset);

		float sk = -(0.2f * (player.Position.Y / floor));
		Skew = shadOffset - (sk);
		Position = new Vector2(posOffset + (sk * 1000) + offset, Position.Y);
	}
}
