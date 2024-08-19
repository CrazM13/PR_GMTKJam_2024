using Godot;
using System;
using System.Collections;
using System.Collections.Generic;

public partial class Trail : Line2D {

	[Export] private Node2D target;
	[Export] private int maxPositions = 10;
	[Export] private int minPositions = 10;
	[Export] private float minDistance = 10;

	[Export] private AudioStreamPlayer trailSFX;

	private List<Vector2> pastPositions;

	public override void _Ready() {
		base._Ready();

		pastPositions = new List<Vector2>();
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (pastPositions.Count == 0 || pastPositions[^1].DistanceSquaredTo(target.GlobalPosition) > minDistance * minDistance) pastPositions.Add(target.GlobalPosition);
		if (Engine.TimeScale > 1) {
			if (pastPositions.Count > maxPositions) {
				pastPositions.RemoveAt(0);
			}
		} else {
			if (pastPositions.Count >= minPositions) {
				pastPositions.RemoveAt(0);
			}
			if (pastPositions.Count >= minPositions) {
				pastPositions.RemoveAt(0);
			}
		}

		Visible = target.Visible && pastPositions.Count > minPositions;
		if (Visible) {
			trailSFX.Play();
		} else {
			trailSFX.Stop();
		}
		this.Width = 32 * target.Scale.X;

		this.Points = pastPositions.ToArray();
		this.Points[^1] = target.GlobalPosition;
	}

}
