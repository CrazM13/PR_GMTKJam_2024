using Godot;
using System;

public partial class DebrisRotation : Sprite2D {

	[Export] private float speed = 1;

	public override void _Process(double delta) {
		base._Process(delta);

		this.Rotate((float) delta * speed);
	}

}
