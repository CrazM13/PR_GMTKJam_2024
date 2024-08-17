using Godot;
using System;

public partial class PlayerMovement : Node2D {
	[Export] private float Speed = 300.0f;
	[Export] private float consumptionEffeciency;
	[Export] private ScaleCamera camera;

	public float Range { get; set; } = 25;

	private Vector2 velocity;

	public override void _PhysicsProcess(double delta) {
		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero) {

			float newX = direction.X * Speed;
			float newY = direction.Y * Speed;

			velocity.X = Mathf.MoveToward(velocity.X, newX, Speed);
			velocity.Y = Mathf.MoveToward(velocity.Y, newY, Speed);
		} else {
			velocity.X *= 0.9f;
			velocity.Y *= 0.9f;
		}

		Position += velocity * (float) delta;
		DebrisManager.Instance.DisplayNearbyEffect(this, Range * 4, 0.1f);
		float consumedMass = DebrisManager.Instance.AttemptConsume(this, Range) * consumptionEffeciency;
		if (consumedMass > 0) {
			camera.Shake(100, 100);
			GameManager.GameScale += consumedMass;
		}

		QueueRedraw();
	}
}
