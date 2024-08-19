using Godot;
using System;

public partial class ButtonEffects : Control {

	private bool isMouseHover = false;

	private float time;

	public override void _Ready() {
		base._Ready();

		this.MouseEntered += () => isMouseHover = true;
		this.MouseExited += () => isMouseHover = false;

		
	}

	public override void _Process(double delta) {
		base._Process(delta);

		this.PivotOffset = this.Size * 0.5f;

		time += (float) delta;

		if (Engine.TimeScale == 0) {
			time = Time.GetTicksMsec() * 0.001f;
		}

		if (isMouseHover) {
			this.Rotation = Mathf.Sin(time * 10) * 0.01f;
		} else {
			this.Rotation = 0;
		}
	}

}
