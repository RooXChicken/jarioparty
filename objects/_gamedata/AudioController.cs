using Godot;
using System;
using System.Collections.Generic;

public partial class AudioController : Node2D
{
	private const ushort MAX_SOUNDS = 32;
	private List<AudioStreamPlayer> pool;
	private Dictionary<string, AudioStream> loadedSounds;
	private Dictionary<string, int> playingSoundIndex;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		pool = new List<AudioStreamPlayer>();
		loadedSounds = new Dictionary<string, AudioStream>();
		playingSoundIndex = new Dictionary<string, int>();
		for(int i = 0; i < MAX_SOUNDS; i++)
		{
			AudioStreamPlayer player = new AudioStreamPlayer();
			GetNode("/root/rm_game/AudioPool").AddChild(player);
			pool.Add(player);
		}
	}

	public void PreLoad(string path, string name = "")
	{
		if(loadedSounds.ContainsKey(path) || loadedSounds.ContainsKey(name))
			return;
			
		AudioStream stream = GD.Load<AudioStream>(path);
		if(name != "")
			loadedSounds.Add(name, stream);
		else
			loadedSounds.Add(path, stream);
	}

	public void PlaySound(string path, bool loop = false)
	{
		int index = 1;
		while(pool[index].Playing)
			index++;

		if(index > MAX_SOUNDS)
			return;

		if(!loadedSounds.ContainsKey(path))
			loadedSounds.Add(path, GD.Load<AudioStream>(path));

		pool[index].Stream = loadedSounds[path];
		pool[index].Play();

		if(!playingSoundIndex.ContainsKey(path))
			playingSoundIndex.Add(path, index);
	}

	public void PlayMusic(string path)
	{
		if(!loadedSounds.ContainsKey(path))
			loadedSounds.Add(path, GD.Load<AudioStream>(path));

		pool[0].Stream = loadedSounds[path];
		pool[0].Play();
	}
	
	public bool StopSound(string path)
	{
		pool[playingSoundIndex[path]].Stop();
		return playingSoundIndex.Remove(path);
	}

	public void StopMusic()
	{
		pool[0].Stop();
	}

	public void StopAllSounds()
	{
		for(int i = 0; i < MAX_SOUNDS; i++)
		{
			pool[i].Stop();
		}

		playingSoundIndex.Clear();
	}
}
