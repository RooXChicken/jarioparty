using Godot;
using System;
using System.Collections.Generic;

public partial class obj_dialogueController : Node2D
{
	private AnimatedSprite2D spr_characterSprite;
	private Sprite2D spr_dialogueBox;
	private Sprite2D spr_arrow;
	private RichTextLabel obj_text;
	public double setDelay = 1.4;

	public int controllerIndex = -2;
	public string[] characterDialogue;
	public List<int> lockedNumbers = new List<int>();
	private int selection = 0;
	public int dialogueIndex = 0;
	private int index = 0;
	private int arrowIndex = 0;
	private double delay = 0.02;
	private double interactDelay = 1.4;
	private bool frozen = false;

	public void SetText(string text) { obj_text.Text = text; }

	private Callable changeDialogue;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		spr_dialogueBox = GetNode<Sprite2D>("spr_dialogueBox");
		spr_arrow = GetNode<Sprite2D>("spr_arrow");
		obj_text = GetNode<RichTextLabel>("obj_text");
	}

	public void Init(string[] _characterDialogue, List<int> _lockedNumbers, Callable _changeDialogue, AnimatedSprite2D _spr_characterSprite)
	{
		dialogueIndex = 0;
		index = 0;
		arrowIndex = 0;

		spr_characterSprite = _spr_characterSprite;

		spr_characterSprite.Animation = "default";
		spr_characterSprite.Frame = 0;
		
		characterDialogue = _characterDialogue;
		lockedNumbers = _lockedNumbers;
		changeDialogue = _changeDialogue;
		spr_characterSprite = _spr_characterSprite;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public void ProcessDialogue(double delta)
	{
		delay -= delta;
		if(delay <= 0)
		{
			delay = 0.01;
			if(index < characterDialogue[dialogueIndex].Length)
			{
				frozen = true;
				spr_arrow.Visible = false;
				index++;
				if(spr_characterSprite.Animation != "speak")
					spr_characterSprite.Animation = "speak";
				obj_text.Text = characterDialogue[dialogueIndex].Substring(0, index);
			}
			else
			{
				spr_arrow.Visible = true;
				if(spr_characterSprite.Animation != "default")
					spr_characterSprite.Animation = "default";
			}
		}

		if(lockedNumbers.Contains(dialogueIndex))
		{
			arrowIndex = 1;
			frozen = true;
		}

		if(controllerIndex == -1)
		{
			interactDelay -= delta;
			if(interactDelay <= 0)
			{
				interactDelay = setDelay;
				ChangeDialogue();
			}
		}
		else
		{
			if(!frozen && Input.GetAxis("up" + controllerIndex, "down" + controllerIndex) != 0)
			{
				arrowIndex = 1 - arrowIndex;
				((AudioController)GetNode("/root/AudioController")).PlaySound("gui_selectionMove");
				frozen = true;
			}
			else if(Input.GetAxis("up" + controllerIndex, "down" + controllerIndex) == 0)
				frozen = false;

			if(Input.IsActionJustPressed("jump" + controllerIndex))
				ChangeDialogue();

			if(Input.IsActionJustPressed("punch" + controllerIndex))
				dialogueIndex = 0;
		}

		spr_arrow.Position = new Vector2(17.5f, -6 - (arrowIndex * -5.5f));
	}

	public void ChangeDialogue()
	{
		int tempDialogue = changeDialogue.Call(arrowIndex, dialogueIndex).As<int>();
		if(tempDialogue == dialogueIndex)
			return;

		((AudioController)GetNode("/root/AudioController")).PlaySound("gui_select");

		dialogueIndex = tempDialogue;
		arrowIndex = 0;
		index = 0;
		spr_characterSprite.Animation = "speak";
		spr_characterSprite.Frame = 0;
	}
}
