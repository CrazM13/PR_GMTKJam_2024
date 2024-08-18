using Godot;
using System;

public partial class ArrowHelper : Node2D {

	public override void _Process(double delta) {
		base._Process(delta);

		DebrisNode nearest = DebrisManager.Instance.GetNearestConsumable(GlobalPosition);

		if (nearest != null) {
			LookAt(nearest.GlobalPosition);
			Visible = true;
		} else {
			Visible = false;
		}
	}

}
