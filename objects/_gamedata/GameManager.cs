using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class GameManager : Node
{
	public short gameState = 0;
	public short playerCount = 1;

	public int controllersConnected = 0;

	public PlayerData[] playerData = new PlayerData[4];
	public ItemBase[] itemLookup = new ItemBase[]
		{
			new ItemBase(0, 7), new ItemBase(1, 4), new ItemBase(2, 9)
		};
	public Random rand = new Random();
	
	private bool DebugMode = true;
	private string lastScene = "";
	private string currentScene = "";
	private bool autoComplete = true;
	public override void _Ready()
	{
		//GetNode<AnimatedSprite2D>("/root/rm_game/LoadingScreen").Visible = false;
		//GetNode<AnimatedSprite2D>("/root/rm_game/LoadingScreen").Play("default");
		//DisplayServer.WindowSetPosition(new Vector2I((DisplayServer.ScreenGetSize().X - 1280) / 2, (DisplayServer.ScreenGetSize().Y - 720) / 2));
		//DisplayServer.WindowSetSize(new Vector2I(1280, 720));
	}

	public override void _Process(double delta)
	{
		if(Input.IsActionJustPressed("DB_restartroom"))
		{
			SwitchScene(currentScene);
		}
	}

	public void SwitchScene(string scene, bool _autoComplete = true, bool autoDisableLoadingScreen = true)
	{
		GetNode<AnimatedSprite2D>("/root/rm_game/spr_load").Visible = true;
		((AudioController)GetNode("/root/AudioController")).StopAllSounds();
		lastScene = currentScene;
		currentScene = scene;
		autoComplete = _autoComplete;

		GetNode("/root/rm_game").RemoveChild(GetNode("/root/rm_game/ActiveScene"));
		PackedScene newScene;
		if(autoComplete)
			newScene = GD.Load<PackedScene>("res://scenes/" + currentScene + ".tscn");
		else
			newScene = GD.Load<PackedScene>(currentScene);

		var instance = newScene.Instantiate();
		instance.Name = "ActiveScene";
		GetNode("/root/rm_game").AddChild(instance);

		GetNode<AnimatedSprite2D>("/root/rm_game/spr_load").Visible = false;
	}
}
