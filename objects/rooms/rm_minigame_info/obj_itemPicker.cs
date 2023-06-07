using Godot;
using System;
using System.Collections.Generic;

public partial class obj_itemPicker : Node2D
{
	private List<ItemBase> items;

	private Sprite2D spr_arrowUp;
	private Sprite2D spr_arrowDown;

	private Sprite2D spr_item1;
	private Sprite2D spr_item2;
	private Sprite2D spr_item3;

	private Vector2 item1Def;
	private Vector2 item2Def;
	private Vector2 item3Def;

	private AnimationPlayer anim_icons;
	private AnimationPlayer anim_whackitu;
	private bool transitioning = false;
	private Sprite2D spr_ready;
	private int inc = 0;
	private int index = 0;

	private int[] slots;

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
		QueueFree();
		spr_item1 = GetNode<Sprite2D>("spr_mask/spr_item1");
		spr_item2 = GetNode<Sprite2D>("spr_mask/spr_item2");
		spr_item3 = GetNode<Sprite2D>("spr_mask/spr_item3");

		item1Def = spr_item1.Position;
		item2Def = spr_item2.Position;
		item3Def = spr_item3.Position;

		Variant _Player = GetMeta("Player");
		player = _Player.As<byte>();

		playerData = ((GameManager)GetNode("/root/GameManager")).playerData[player];
		controllerIndex = playerData.controllerIndex;
		
		items = new List<ItemBase>();

		//items.Add(new NoItem());

		for(int i = 0; i < playerData.items.Count; i++)
		{
			items.Add(playerData.items[i]);
		}

		slots = new int[items.Count];
		for(int i = 0; i < items.Count-1; i++)
			slots[i] = i;

		spr_arrowUp = GetNode<Sprite2D>("spr_arrowUp");
		spr_arrowDown = GetNode<Sprite2D>("spr_arrowDown");

		spr_ready = GetNode<Sprite2D>("spr_ready");

		anim_icons = GetNode<AnimationPlayer>("anim_icons");
		anim_whackitu = GetNode<AnimationPlayer>("../../anim_whackitu");

		t_arrows = new Alarm(0.15, true, this, new Callable(this, "SetArrows"));

		SetIcons();
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

		if(itemIndex > 3)
			itemIndex = 0;
		if(itemIndex < 0)
			itemIndex = 3;

		SetIcons();

		t_arrows.Start();

		if(inc > 0)
		{
			spr_arrowDown.Position = new Vector2(spr_arrowDown.Position.X, spr_arrowDown.Position.Y + 2);
			anim_icons.Play("moveItems");
			
		}
		else if(inc < 0)
		{
			spr_arrowUp.Position = new Vector2(spr_arrowUp.Position.X, spr_arrowUp.Position.Y - 2);
			anim_icons.PlayBackwards("moveItems");
			//SetIcons();
		}
	}

	public void SetIcons()
	{
		spr_item1.Modulate = new Color(1, 1, 1, 1);
		spr_item2.Modulate = new Color(1, 1, 1, 0.5f);
		spr_item3.Modulate = new Color(1, 1, 1, 0.5f);

		spr_item1.Position = item1Def;
		spr_item2.Position = item2Def;
		spr_item3.Position = item3Def;

		// if(inc < 0)
		// {
		// 	return;
		// 	anim_icons.Seek(0.5, true);
		// 	index++;
		// }
		// if(inc > 0)
		// {
			
		// 	int[] tempSlots = slots;
		// 	for(int i = 1; i < slots.Length-1; i++)
		// 	{
		// 		slots[i] = tempSlots[i+1];
		// 	}

		// 	slots[0] = tempSlots[slots.Length-1];

		// 	string stupidass = "[";
		// 	foreach(int i in slots)
		// 		stupidass += i + ", ";
		// 	stupidass += "]";
		// 	GD.Print(stupidass);

		// }

		// GD.Print(slots[0]);

		

		// if(index < 0)
		// 	itemIndex = items.Count-1;

		//confusing bullshit

		// spr_item1.Texture = items[slots[0]].Texture;
		// spr_item2.Texture = items[slots[1]].Texture;
		// spr_item3.Texture = items[slots[2]].Texture;

		if(inc > 0)
		{
			//anim_icons.Seek(0, true);
			switch(itemIndex)
			{
				case 0:
					spr_item3.Texture = items[2].Texture;
					spr_item1.Texture = items[0].Texture;
					spr_item2.Texture = items[1].Texture;

					GD.Print(items[0].ItemIndex);
					break;
				case 1:
					spr_item3.Texture = items[3].Texture;
					spr_item1.Texture = items[2].Texture;
					spr_item2.Texture = items[0].Texture;

					GD.Print(items[2].ItemIndex);
					break;
				case 2:
					spr_item3.Texture = items[1].Texture;
					spr_item1.Texture = items[3].Texture;
					spr_item2.Texture = items[2].Texture;

					GD.Print(items[3].ItemIndex);
					break;
				case 3:
					spr_item3.Texture = items[0].Texture;
					spr_item1.Texture = items[1].Texture;
					spr_item2.Texture = items[3].Texture;

					GD.Print(items[1].ItemIndex);
					break;
			}
		}
		if(inc < 0)
		{
			anim_icons.Seek(0.5, true);
			switch(itemIndex)
			{
				case 0:
					spr_item3.Texture = items[3].Texture;
					spr_item1.Texture = items[2].Texture;
					spr_item2.Texture = items[0].Texture;

					GD.Print(items[2].ItemIndex);
					break;
				case 1:
					spr_item3.Texture = items[2].Texture;
					spr_item1.Texture = items[0].Texture;
					spr_item2.Texture = items[1].Texture;

					GD.Print(items[0].ItemIndex);
					break;
				case 2:
					spr_item3.Texture = items[0].Texture;
					spr_item1.Texture = items[1].Texture;
					spr_item2.Texture = items[3].Texture;

					GD.Print(items[1].ItemIndex);
					break;
				case 3:
					spr_item3.Texture = items[1].Texture;
					spr_item1.Texture = items[3].Texture;
					spr_item2.Texture = items[2].Texture;

					GD.Print(items[3].ItemIndex);
					break;
			}
		}
	}

	public void SetArrows()
	{
		if(inc > 0)
			spr_arrowDown.Position = new Vector2(spr_arrowDown.Position.X, spr_arrowDown.Position.Y - 2);
		else if(inc < 0)
			spr_arrowUp.Position = new Vector2(spr_arrowUp.Position.X, spr_arrowUp.Position.Y + 2);
	}

	private void AnimationFinished(StringName anim_name)
	{
		//if(inc > 0)
			//SetIcons();
	}
}
