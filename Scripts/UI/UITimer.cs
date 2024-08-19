using Godot;
using System;

public partial class UITimer : Label {

	private Vector2 startingPosition;

	private ShakeConfig shakeConfig;

	public override void _Ready() {
		base._Ready();

		shakeConfig = new ShakeConfig();

		startingPosition = Position;

		shakeConfig.OnShakeTick += (float shakeTime) => {
			if (Engine.TimeScale > 1) this.Modulate = Color.FromHsv((shakeTime * 0.01f) % 1, 1, 1);
			else if (Engine.TimeScale < 1) this.Modulate = Colors.Gray.Lerp(Colors.DarkGray, Mathf.Sin(shakeTime));
		};

		shakeConfig.OnReset += () => {
			Position = startingPosition;
			this.Modulate = Colors.White;
		};

	}

	public override void _Process(double delta) {
		base._Process(delta);

		this.Text = $"Time: {Mathf.FloorToInt(GameManager.GameTime / 3600):D2}:{Mathf.FloorToInt(GameManager.GameTime / 60):D2}:{Mathf.FloorToInt(GameManager.GameTime % 60):D2}";

		if (Engine.TimeScale > 1) {
			shakeConfig.Shake(2f, 100f);
		} else if (Engine.TimeScale < 1) {
			shakeConfig.Shake(1f, 50f);
		}

		Position = startingPosition + shakeConfig.UpdateShake((float) delta);
	}

}
