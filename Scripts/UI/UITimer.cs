using Godot;
using System;

public partial class UITimer : Label {

	public override void _Process(double delta) {
		base._Process(delta);

		this.Text = $"Time: {Mathf.FloorToInt(GameManager.GameTime / 3600):D2}:{Mathf.FloorToInt(GameManager.GameTime / 60):D2}:{Mathf.FloorToInt(GameManager.GameTime % 60):D2}";
	}

}
