using Godot;
using System;

public partial class obj_diceRB : RigidBody2D
{
	public Vector2 newPosition;
	public bool shouldSnap = false;

	public override void _IntegrateForces(PhysicsDirectBodyState2D state)
	{
		if(shouldSnap)
			state.Transform = new Transform2D(0, new Vector2(1, 1), 0, newPosition);
		shouldSnap = false;
		base._IntegrateForces(state);
	}
}
