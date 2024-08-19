using Godot;
using System;

public partial class PersistantMusic : AudioStreamPlayer {

	[Export] private string musicGroup;

	private static float playbackPosition;
	private static string currentGroup;

	public override void _Ready() {
		base._Ready();

		if (musicGroup == currentGroup) this.Play(playbackPosition);
		else currentGroup = musicGroup;

		this.Finished += OnMusicFinish;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		playbackPosition = this.GetPlaybackPosition();
	}

	private void OnMusicFinish() {
		Play(0);
	}

}
