using Godot;
using System;
using System.Collections.Generic;
using System.Threading;

public partial class GameManager : Node
{
	public short gameState = 0;
	public short playerCount = 1;
	public int TurnNumber = 1;
	public int TurnsMax = 20;

	public int controllersConnected = -1;
	public int CurrentMinigame = 5;
	public bool MinigameStarted = false;
	public bool MinigameOver = false;
	public Vector2 StarSpacePos = new Vector2(0, 0);
	public List<int> numsLeft;

	public PlayerData[] playerData = new PlayerData[4];
	public ItemBase[] itemLookup = new ItemBase[]
		{
			new Cola(0, 7), new DoubleDice(1, 4), new ZombieKey(2, 9), new PocketDimension(3, 3)
		};
	public MinigameBase[] minigameLookup = new MinigameBase[]
		{
			new MinigameBase(0, 4, "testminigame", "testminigame", "YOU SHOULD NOT SEE THIS", new List<string>()),
			new MinigameBase(1, 4, "Mushroom Mix-Up", "mushmixup", "There are 7 mushrooms, but be careful!\nFrog will wave his flag and you must stay on the mushroom of the same color, as the others will fall!\n     - Move (Navigate mushrooms!)\n     - Jump (Save yourself!)\n     - Attack (Push players!)",
				new List<string>() {"move", "jump", "jump_bar", "punch", "win"}, "mus_minigame_savingcourage", 60, new int[] {8, 1, 3}, new int[] {}),
			new MinigameBase(2, 3, "Look Away!", "lookaway", "When the music drops, look away from the top player!\n\n\n     - Look (A different way!)",
				new List<string>() {"move"}, "mus_minigame_lookaway", -5, new int[] {8}),
			new MinigameBase(3, 4, "Jumpyrope", "jumprope", "Don't get hit by the fire! The Jumprope will speed up as it progresses. The last person standing wins!\n\n     - Jump (Dodge the fire!)",
				new List<string>() {"jump", "jump_phy", "shadow", "win"}, "mus_minigame_jumprope", 120, new int[] {1}, new int[] {}, 4),
			new MinigameBase(4, 4, "Susville", "susville", "The light are sabotaged by the imposter! You must find the imposter in the dark, and make sure not to let all of the crewmates die!\n     - Look (Find the imposter!)\n     - Shoot (Take your guess!)",
				new List<string>() {"gun", "score"}, "mus_minigame_susville", -5, new int[] {8, 7}, new int[] {}),
			new MinigameBase(5, 2, "Balley Ball", "balleyball", "It's a classic game of Balley Ball! Jump to hit the ball to the other side, and keep it from hitting the sand!\n\n     - Move (To the ball!)\n     - Jump (Hit the ball!)",
				new List<string>() {"move", "jump", "jump_balley", "score", "win"}, "mus_minigame_balleyball", -5, new int[] {8, 1}, new int[] {}),
			new MinigameBase(6, 3, "Tug-o-Jar", "tugojar", "It's a friendly game of tug-o-war! Just don't fall into the inescapable pit and die! Good luck!\n\n     - Pull (With great force!)",
				new List<string>() {"pull", "win"}, "mus_minigame_tugojar", 60, new int[] {1}, new int[] {})
		};
	public Random rand = new Random();
	public bool Paused = false;
	public int PausePlayer = 0;

	public Texture2D[] PlayerNameImages = new Texture2D[] {
		GD.Load<Texture2D>("res://sprites/gui/roundStart/spr_jario.png"), GD.Load<Texture2D>("res://sprites/gui/roundStart/spr_wooigi.png"), GD.Load<Texture2D>("res://sprites/gui/roundStart/spr_grapejuice.png"), GD.Load<Texture2D>("res://sprites/gui/roundStart/spr_josh.png")
	};

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

		ProcessMode = Node.ProcessModeEnum.Always;

		foreach(ItemBase item in itemLookup)
		{
			GetNode<Node>("/root/rm_game/Items").AddChild(item);
		}
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

		if(Paused)
		{
			if(Input.IsActionJustPressed("pause" + PausePlayer))
			{
				Paused = false;
				((obj_pause)GetNode<CanvasLayer>("/root/rm_game/obj_pause")).TogglePause();
			}
		}
		else
		{
			for(int i = 1; i <= 4; i++)
			{
				if(Input.IsActionJustPressed("pause" + i))
				{
					Paused = !Paused;
					PausePlayer = i;
					((obj_pause)GetNode<CanvasLayer>("/root/rm_game/obj_pause")).TogglePause();
				}
			}
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

		numsLeft = new List<int>();
		numsLeft.Add(1);
		numsLeft.Add(2);
		numsLeft.Add(3);
		numsLeft.Add(4);
	}

	public void LoadDefaults(int minigame)
	{
		CurrentMinigame = minigame;
		for(int i = 0; i < 4; i++)
		{
			playerData[i] = new PlayerData(-1, (ushort)i, (i >= 1));

			playerData[i].items.Add(itemLookup[0]);
			playerData[i].items.Add(itemLookup[1]);
			playerData[i].items.Add(itemLookup[2]);

			playerData[i].playerOrder = i + 1;

			//playerData[i].animationFrames.SetFrame()

			playerData[i].Load();
		}

		playerData[0].controllerIndex = 1;
	}
}
