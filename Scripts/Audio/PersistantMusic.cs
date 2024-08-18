using Godot;
using System;

public partial class PersistantMusic : AudioStreamPlayer {

	private static float playbackPosition;

	public override void _Ready() {
		base._Ready();

		this.Seek(playbackPosition);

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
