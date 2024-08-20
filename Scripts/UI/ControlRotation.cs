using Godot;
using System;

public partial class ControlRotation : Control {

	[Export] private float speed = 1;

	public override void _Process(double delta) {
		base._Process(delta);

		this.PivotOffset = this.Size / 2f;
		this.Rotation += ((float) delta * speed);
	}


}
