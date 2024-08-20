using Godot;
using System;

public partial class GameAudioChange : PersistantMusic {

	private int currentStage = 0;

	[Export] private AudioStream advancedMusic;

	private void OnLevelUp() {
		currentStage++;

		if (currentStage == 8) {
			this.Stream = advancedMusic;
			this.Play(0);
		}
	}

}
