using Godot;
using System;

public partial class UIZoomLevel : Label {

	[Export] private PlayerMovement player;
	[Export] private float zoomSpeed;

	private float[] zoomScales = {
		-14.5f,
		-3.3f,
		-0.7f,
		0.7f,
		2.7f,
		3.8f,
		4.6f,
		6.5f,
		7.1f,
		8.3f,
		9.1f,
		9.7f,
		11.0f,
		12.5f,
		13.3f
	};

	private float currentScale = 0f;
	private float targetScale = 0f;

	private void OnLevelUp() {
		targetScale = zoomScales[player.GetFormIndex];
	}

	public override void _Process(double delta) {
		base._Process(delta);

		currentScale = Mathf.MoveToward(currentScale, targetScale, (float) delta * zoomSpeed);
		this.Text = currentScale.ToString("F1");
	}

}
