using Godot;
using System;

public partial class spr_bagPaper : Sprite2D
{
	public float Speed = 150f;
	
	private Vector2 start;
	private Vector2 end;
	private float alpha;

	public byte state = 0;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		start = new Vector2(0, Position.Y);
		end = Position;

		alpha = 1;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		switch(state)
		{
			case 0:
				Position = Position.Lerp(end, (float)delta * 6);

				if(alpha > 0.7f)
					alpha -= (float)delta * 2;
				break;
			case 1:
				Position = Position.Lerp(start, (float)delta * 6);
				if(alpha < 1)
					alpha += (float)delta * 2;
				break;
		}

		GetNode<Node2D>("../../").Modulate = new Color(1, 1, 1, alpha);
		((ShaderMaterial)GetNode<Sprite2D>("../../spr_wallet/spr_walletColor").Material).SetShaderParameter("alpha", alpha);
	}

	public void ChangeItems(PlayerData playerData)
	{
		if(playerData.items.Count <= 0)
			state = 1;
		for(int i = 0; i < playerData.items.Count; i++)
			GetNode<Sprite2D>("Items/spr_item" + (i+1)).Texture = playerData.items[i].Texture;
	}
}
