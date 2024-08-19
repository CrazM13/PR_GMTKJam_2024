using Godot;
using System;

public partial class ArrowHelper : Node2D {

	[Export] private Sprite2D arrowSprite;

	public override void _Ready() {
		base._Ready();

		Visible = GameManager.ShowHelpArrow;
	}

	private float targetRotation = 0;

	private float timeElapsed = 0;

	public override void _Process(double delta) {
		if (GameManager.ShowHelpArrow) {
			timeElapsed += (float)delta;
			DebrisNode nearest = DebrisManager.Instance.GetNearestConsumable(GlobalPosition);

			if (nearest != null && GlobalPosition.DistanceSquaredTo(nearest.GlobalPosition) >= 10000) {

				Vector2 direction = nearest.GlobalPosition - GlobalPosition;
				targetRotation = Mathf.Atan2(direction.Y, direction.X);

				this.Rotation = Mathf.LerpAngle(this.Rotation, targetRotation, (float) delta * 10f);

				if (arrowSprite != null) {
					arrowSprite.Offset = new Vector2(Mathf.Sin(timeElapsed * 10f) * 3f, 0);
				}

				Visible = true;
			} else {
				Visible = false;
			}
		}
	}

}
