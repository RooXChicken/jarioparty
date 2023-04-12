using Godot;
using System;

public partial class obj_map : Node2D
{
	public int Turn = 0;
	public int PlayerGoing = 1;

	private Camera2D obj_camera;
	private Node2D playerGoing;
	private Alarm t_zoomIn;

	private double delta = 0;
	private Vector2 zoomLevel = new Vector2(1, 1);
	private bool cameraStarted = false;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		((AudioController)GetNode("/root/AudioController")).PlayMusic("res://sound/rooms/maps/mus_sushroom.wav");

		obj_camera = GetNode<Camera2D>("obj_camera");
		playerGoing = GetNode<Node2D>("Paths/pt_01/pf_0" + PlayerGoing);

		obj_camera.Position = new Vector2(0, 0);
		obj_camera.Zoom = new Vector2(0.35f, 0.35f);

		t_zoomIn = new Alarm(2, true, this, new Callable(this, "ZoomIn"));
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
		obj_camera.Zoom = obj_camera.Zoom.Lerp(zoomLevel, (float)delta * 3f);
		obj_camera.Position = obj_camera.Position.Lerp(playerGoing.Position, (float)delta * 3f);

		cameraStarted = true;
	}

	public void SetPlayer(int _player)
	{
		playerGoing.GetNode<obj_character_map>("obj_character_map").isTurn = false;
		PlayerGoing = _player;
		playerGoing = GetNode<Node2D>("Paths/pt_01/pf_0" + PlayerGoing);
		playerGoing.GetNode<obj_character_map>("obj_character_map").isTurn = true;
		GD.Print("Player " + _player);
	}

	public void SetZoomLevel(float _zoom) { zoomLevel = new Vector2(_zoom, _zoom); }

	public void Snap()
	{
		obj_camera.Zoom = zoomLevel;
		obj_camera.Position = playerGoing.Position;
	}
}
