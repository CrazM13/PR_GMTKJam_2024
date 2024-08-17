using Godot;
using System;

public partial class DebrisRotation : Sprite2D {


	public override void _Process(double delta) {
		base._Process(delta);

		this.Rotate((float) delta);
	}

}
