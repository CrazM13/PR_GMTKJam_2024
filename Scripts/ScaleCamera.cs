using Godot;
using System;

public partial class ScaleCamera : Camera2D {

	[Export] private float minZoom;
	[Export] private float maxZoom;

	private float zoom = 1;

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

		if (Input.IsActionPressed("zoom_in") || Input.IsActionJustReleased("zoom_in")) {
			zoom += (float) delta;

			if (zoom > maxZoom) zoom = maxZoom;

			this.Zoom = Vector2.One * zoom;
		} else if (Input.IsActionPressed("zoom_out") || Input.IsActionJustReleased("zoom_out")) {
			zoom -= (float) delta;

			if (zoom < minZoom) zoom = minZoom;

			this.Zoom = Vector2.One * zoom;
		}
	}

}
