using Godot;
using System;
using System.Collections.Generic;

public partial class AbilityGravityWell : Node2D {

	[Export] private float power;

	public override void _Process(double delta) {
		base._Process(delta);

		if (!GameManager.IsGamePaused && !Input.IsActionPressed("time_sprint") && Input.IsActionPressed("time_crawl")) {
			Visible = true;

			List<DebrisNode> debrisNodes = DebrisManager.Instance.GetNearbyDebris(GlobalPosition, 256);

			foreach (DebrisNode node in debrisNodes) {

				Vector2 currentVelocity = node.Velocity;
				Vector2 targetVelocity = (GlobalPosition - node.GlobalPosition).Normalized() * (power - node.TargetScale);

				node.Velocity = (currentVelocity * 0.25f) + (targetVelocity * 0.75f);
				node.Shake(5f, 100f);
			}

		} else {
			Visible = false;
		}
	}

}
