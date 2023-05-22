using Godot;
using System;

public partial class obj_map : Node2D
{
	public int Turn = 0;
	public int PlayerGoing = 1;

	private Camera2D obj_camera;
	private PathFollow2D playerGoing;
	public Vector2 offset = new Vector2(0, 0);
	private Alarm t_zoomIn;

	private double delta = 0;
	private Vector2 zoomLevel = new Vector2(1, 1);
	public bool cameraStarted = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("res://sound/rooms/maps/mus_sushroom.wav");

		obj_camera = GetNode<Camera2D>("obj_camera");
		playerGoing = GetNode<PathFollow2D>("Paths/pt_01/pf_0" + PlayerGoing);

		Initialize();
	}

	public void Initialize()
	{
		obj_camera.Position = new Vector2(0, 0);
		obj_camera.Zoom = new Vector2(0.35f, 0.35f);
		if(!((GameManager)GetNode("/root/GameManager")).playerData[0].PlayerStarted)
		{
			
			obj_camera.Position = new Vector2(-864, 400);
			obj_camera.Zoom = new Vector2(1, 1);
		}
		else
		{
			GetNode<obj_playerStart>("obj_mapGUI/PlayerStart").StartAnimation(playerGoing.GetNode<obj_character_map>("obj_character_map").CharacterIndex + 1, playerGoing.GetNode<obj_character_map>("obj_character_map"));
			t_zoomIn = new Alarm(1, true, this, new Callable(this, "ZoomIn"));
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.delta = delta;
		if(cameraStarted)
			ZoomIn();
	}

	private void ZoomIn()
	{
		Vector2 camPos;
		camPos = playerGoing.Position;
		obj_camera.Zoom = obj_camera.Zoom.Lerp(zoomLevel, (float)delta * 3f);
		obj_camera.Position = obj_camera.Position.Lerp(camPos + offset, (float)delta * 3f);

		cameraStarted = true;
	}

	public void SetPlayer(int _player)
	{
		playerGoing.GetNode<obj_character_map>("obj_character_map").isTurn = false;
		PlayerGoing = _player;
		playerGoing = GetNode<PathFollow2D>("Paths/pt_01/pf_0" + PlayerGoing);
		playerGoing.GetNode<obj_character_map>("obj_character_map").isTurn = true;
		if(((GameManager)GetNode("/root/GameManager")).playerData[0].PlayerStarted)
			GetNode<obj_playerStart>("obj_mapGUI/PlayerStart").StartAnimation(playerGoing.GetNode<obj_character_map>("obj_character_map").CharacterIndex + 1, playerGoing.GetNode<obj_character_map>("obj_character_map"));
		GetNode<Transition>("obj_mapGUI/Transition").snap = true;
		//GD.Print("Player " + _player);
	}

	public void SetZoomLevel(float _zoom) { zoomLevel = new Vector2(_zoom, _zoom); }

	public void Snap()
	{
		obj_camera.Zoom = zoomLevel;
		obj_camera.Position = playerGoing.Position;
	}
}
