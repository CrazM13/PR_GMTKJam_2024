using Godot;
using System;

public partial class LevelNameSplash : LevelName {

	[Export] private float typeoutSpeed;
	[Export] private float deleteSpeed;
	[Export] private float showDuration;

	private float time;
	private int state = -1;

	public override void _Ready() {
		base._Ready();

		VisibleRatio = 0;
		GameManager.IsGamePaused = true;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (state == -1) {
			time += (float) delta;
			if (time >= 0.2f) {
				state = 0;
				time = 0;
			}
		} else if (state == 0) {
			time += (float) delta * typeoutSpeed;

			VisibleRatio = time;

			if (time >= 1) {
				state = 1;
				time = 0;
			}
		} else if (state == 1) {
			time += (float) delta;

			if (time >= showDuration) {
				state = 2;
				time = 1;
			}
		} else if (state == 2) {
			time -= (float) delta * deleteSpeed;

			VisibleRatio = time;

			if (time <= 0) {
				state = 3;
				GameManager.IsGamePaused = false;
			}
		}

	}

}
