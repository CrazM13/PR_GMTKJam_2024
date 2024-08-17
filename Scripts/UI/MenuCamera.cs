using Godot;
using System;

public partial class MenuCamera : Camera2D {

	[Export] private float speed;

	public override void _Process(double delta) {
		base._Process(delta);

		Position += new Vector2((float) delta * speed, 0);

	}

}
