using Godot;
using System;
using System.Collections.Generic;

public partial class obj_itemPicker : Node2D
{
	private List<Sprite2D> items;

	private Sprite2D spr_arrowUp;
	private Sprite2D spr_arrowDown;

	private Vector2 item1Def;
	private Vector2 item2Def;
	private Vector2 item3Def;

	private AnimationPlayer anim_icons;
	private AnimationPlayer anim_whackitu;
	private bool transitioning = false;
	private Sprite2D spr_ready;
	private int inc = 0;
	private int index = 0;

	public bool ready = false;

	private byte player;
	private PlayerData playerData;
	private int controllerIndex;

	private float joyhaxis = 0;
	private float joyvaxis = 0;

	private Alarm t_arrows;
	
	private int itemIndex = 0;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		// spr_noitem = GetNode<Sprite2D>("spr_mask/spr_noitem");
		// spr_item1 = GetNode<Sprite2D>("spr_mask/spr_item1");
		// spr_item2 = GetNode<Sprite2D>("spr_mask/spr_item2");
		// spr_item3 = GetNode<Sprite2D>("spr_mask/spr_item3");

		spr_arrowUp = GetNode<Sprite2D>("spr_arrowUp");
		spr_arrowDown = GetNode<Sprite2D>("spr_arrowDown");

		spr_ready = GetNode<Sprite2D>("spr_ready");

		// item1Def = spr_item1.Position;
		// item2Def = spr_item2.Position;
		// item3Def = spr_item3.Position;

		anim_icons = GetNode<AnimationPlayer>("anim_icons");
		anim_whackitu = GetNode<AnimationPlayer>("../../anim_whackitu");

		Variant _Player = GetMeta("Player");
		player = _Player.As<byte>();

		playerData = ((GameManager)GetNode("/root/GameManager")).playerData[player];
		controllerIndex = playerData.controllerIndex;

		// spr_item1.Texture = playerData.items[0].Texture;
		// spr_item2.Texture = playerData.items[1].Texture;
		// spr_item3.Texture = playerData.items[2].Texture;

		t_arrows = new Alarm(0.15, true, this, new Callable(this, "SetArrows"));
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{				
		GetControllerInput();

		if(!ready && joyvaxis > 0 && anim_icons.CurrentAnimation == "")
			ChangeItemIndex(1);
		if(!ready && joyvaxis < 0 && anim_icons.CurrentAnimation == "")
			ChangeItemIndex(-1);
	}

	private void GetControllerInput()
	{
		if(controllerIndex == -1)
			return;
		
		joyhaxis = Input.GetAxis("left" + controllerIndex, "right" + controllerIndex);
		joyvaxis = Input.GetAxis("up" + controllerIndex, "down" + controllerIndex);

		if(!anim_whackitu.IsPlaying() && Input.IsActionJustPressed("pause" + controllerIndex))
		{
			anim_whackitu.Play("transition");
			transitioning = true;
		}

		if(Input.IsActionJustPressed("jump" + controllerIndex))
		{
			ready = true;
			spr_ready.Visible = true;
		}
		if(Input.IsActionJustPressed("back" + controllerIndex))
		{
			ready = false;
			spr_ready.Visible = false;
		}
	}

	private void ChangeItemIndex(int _inc)
	{
		((AudioController)GetNode("/root/AudioController")).PlaySound("itemPickerMove");
		inc = _inc;
		itemIndex += inc;

		if(itemIndex > 2)
			itemIndex = 0;
		if(itemIndex < 0)
			itemIndex = 2;

		SetIcons();

		// anim_icons.CurrentAnimation = "item1";
		// anim_icons.

		t_arrows.Start();

		if(inc > 0)
		{
			spr_arrowDown.Position = new Vector2(spr_arrowDown.Position.X, spr_arrowDown.Position.Y + 2);
			anim_icons.Play("item1");
		}
		else if(inc < 0)
		{
			spr_arrowUp.Position = new Vector2(spr_arrowUp.Position.X, spr_arrowUp.Position.Y - 2);
			anim_icons.PlayBackwards("item1");
		}
	}

	public void SetIcons()
	{
		// anim_icons.CurrentAnimation = "[stop]";
		// // spr_item1.Position = item1Def;
		// // spr_item1.Modulate = new Color(1, 1, 1, 1);
		// // spr_item2.Position = item2Def;
		// // spr_item2.Modulate = new Color(1, 1, 1, 0.5f);
		// // spr_item3.Position = item3Def;
		// // spr_item3.Modulate = new Color(1, 1, 1, 0.5f);
		// if(inc > 0)
		// {
		// 	switch(itemIndex)
		// 	{
		// 		case 0:
		// 			spr_item1.Texture = playerData.items[0].Texture;
		// 			spr_item2.Texture = playerData.items[1].Texture;
		// 			spr_item3.Texture = playerData.items[2].Texture;
		// 			break;
		// 		case 1:
		// 			spr_item1.Texture = playerData.items[2].Texture;
		// 			spr_item2.Texture = playerData.items[0].Texture;
		// 			spr_item3.Texture = playerData.items[1].Texture;
		// 			break;
		// 		case 2:
		// 			spr_item1.Texture = playerData.items[1].Texture;
		// 			spr_item2.Texture = playerData.items[2].Texture;
		// 			spr_item3.Texture = playerData.items[0].Texture;
		// 			break;
		// 	}
		// }
		// if(inc < 0)
		// {
		// 	anim_icons.Seek(0.5, true);
		// 	switch(itemIndex)
		// 	{
		// 		case 1:
		// 			spr_item1.Texture = playerData.items[1].Texture;
		// 			spr_item2.Texture = playerData.items[2].Texture;
		// 			spr_item3.Texture = playerData.items[0].Texture;
		// 			break;
		// 		case 2:
		// 			spr_item1.Texture = playerData.items[0].Texture;
		// 			spr_item2.Texture = playerData.items[1].Texture;
		// 			spr_item3.Texture = playerData.items[2].Texture;
		// 			break;
		// 		case 0:
		// 			spr_item1.Texture = playerData.items[2].Texture;
		// 			spr_item2.Texture = playerData.items[0].Texture;
		// 			spr_item3.Texture = playerData.items[1].Texture;
		// 			break;
		// 	}
		// }
	}

	public void SetArrows()
	{
		if(inc > 0)
			spr_arrowDown.Position = new Vector2(spr_arrowDown.Position.X, spr_arrowDown.Position.Y - 2);
		else if(inc < 0)
			spr_arrowUp.Position = new Vector2(spr_arrowUp.Position.X, spr_arrowUp.Position.Y + 2);
	}
}
