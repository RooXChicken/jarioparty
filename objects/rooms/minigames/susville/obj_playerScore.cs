using Godot;
using System;

public partial class obj_playerScore : Sprite2D
{
	private int score = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PreLoad("res://sound/rooms/minigames/snd_addScore.wav", "minigame_addScore");
		Material = new ShaderMaterial() { Shader = (Material as ShaderMaterial).Shader.Duplicate() as Shader};
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void AddScore(int addition)
	{
		score += addition;
		Frame = score;

		((AudioController)GetNode("/root/AudioController")).PlaySound("minigame_addScore");
	}
}
