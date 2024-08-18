using Godot;
using System;

public partial class ArrowHelper : Node2D {

	public override void _Ready() {
		base._Ready();

		Visible = GameManager.showHelpArrow;
	}

	public override void _Process(double delta) {
		if (GameManager.showHelpArrow) {
			DebrisNode nearest = DebrisManager.Instance.GetNearestConsumable(GlobalPosition);

			if (nearest != null && GlobalPosition.DistanceSquaredTo(nearest.GlobalPosition) >= 10000) {
				LookAt(nearest.GlobalPosition);
				Visible = true;
			} else {
				Visible = false;
			}
		}
	}

}
