using Godot;
using System;
using System.Collections.Generic;

public partial class Alarm : Timer
{
	public Alarm(double waitTime, bool oneShot, Node parent, Callable callable, bool autoStart = true)
	{
		WaitTime = waitTime;
		OneShot = oneShot;
		Connect("timeout", callable);
		parent.AddChild(this);
		if(autoStart)
			Start();
	}
}
