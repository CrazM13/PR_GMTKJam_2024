using Godot;
using System;

public partial class ScaleCamera : Camera2D {

	private ShakeConfig shakeConfig;

	public override void _Ready() {
		base._Ready();

		shakeConfig = new ShakeConfig();
	}

	public void Shake(float strength, float intensity = 1) {
		shakeConfig.Shake(strength, intensity);
	}

	public override void _Process(double delta) {
		base._Process(delta);

		this.Offset = shakeConfig.UpdateShake((float) delta);
	}

}
