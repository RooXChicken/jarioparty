using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class GameManager : Node
{
	public short gameState = 0;
	public short playerCount = 1;

	public int controllersConnected = 0;
	public int CurrentMinigame = 1;

	public PlayerData[] playerData = new PlayerData[4];
	public ItemBase[] itemLookup = new ItemBase[]
		{
			new ItemBase(0, 7), new ItemBase(1, 4), new ItemBase(2, 9)
		};
	public MinigameBase[] minigameLookup = new MinigameBase[]
		{
			new MinigameBase(0, "testminigame", "YOU SHOULD NOT SEE THIS"),
			new MinigameBase(1, "mushmixup", "There are 7 mushrooms, but be careful!\nFrog will wave his flag and you must stay on the mushroom of the same color, as the others will fall!\n     - Move (Navigate mushrooms!)\n     - Jump (Save yourself!)\n     - Attack (Push players!)",
			60, new int[] {8, 1, 3})
		};
	public Random rand = new Random();

	public bool Fullscreen = false;
	
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

		if(Input.IsActionJustPressed("fullscreen"))
		{
			if(Fullscreen)
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Windowed);
			else
				DisplayServer.WindowSetMode(DisplayServer.WindowMode.Fullscreen);

			Fullscreen = !Fullscreen;
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
