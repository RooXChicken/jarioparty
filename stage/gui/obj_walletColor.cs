using Godot;
using System;

public partial class obj_walletColor : Sprite2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Material = new ShaderMaterial() { Shader = (Material as ShaderMaterial).Shader.Duplicate() as Shader};
		((ShaderMaterial)Material).SetShaderParameter("walletColor", new Vector3(1, 1, 1));
		((ShaderMaterial)Material).SetShaderParameter("alpha", 1);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
