using Godot;
using System;

public partial class TitleEffects : Label {

	[Export] private float rotationSpeed;
	[Export] private float rotationLimit;

	private float time;

	public override void _Ready() {
		base._Ready();

		this.PivotOffset = this.Size * 0.5f;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		time += (float) delta;

		this.Rotation = Mathf.Sin(time * rotationSpeed) * rotationLimit;
	}

}
