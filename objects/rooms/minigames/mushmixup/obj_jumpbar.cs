using Godot;
using System;

public partial class obj_jumpbar : Node2D
{
	public float JumpTime {get {return (value / MaxJumpValue);} }
	public float Jump {get {float v = value / MaxJumpValue; value = 0; return v;} }
	public bool Punch {get {bool result = false; if(value >= 40f) {result = true; value -= 40f;} return result;} }
	private float value = 0;
	private Sprite2D bar;

	private float MinJumpValue = 10;
	private float MaxJumpValue = 100;
	private float RefillRate = 20f;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bar = GetNode<Sprite2D>("spr_bar");
		bar.Material = new ShaderMaterial() { Shader = (bar.Material as ShaderMaterial).Shader.Duplicate() as Shader};

		Variant _Min = GetMeta("Min");
		Variant _Max = GetMeta("Max");
		Variant _RefillRate = GetMeta("RefillRate");

		MinJumpValue = _Min.As<float>();
		MaxJumpValue = _Max.As<float>();
		RefillRate = _RefillRate.As<float>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if(value < MaxJumpValue)
			value += (float)delta * RefillRate;
		((ShaderMaterial)bar.Material).SetShaderParameter("size", value / MaxJumpValue);
	}
}
