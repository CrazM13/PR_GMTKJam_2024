using Godot;
using System;

public partial class PlayerMovement : Node2D {
	[Export] private float Speed = 300.0f;
	[Export] private float consumptionEffeciency;
	[Export] private Sprite2D sprite;
	[Export] private ScaleCamera camera;

	public float Range { get; set; } = 10;

	private Vector2 velocity;

	private bool acceptInputs = true;

	private string[] playerForms = {
		"ALPHA_PARTICLE",
		"GRAIN_OF_SAND",
		"STONE",
		"BOULDER",
		"SPACE_DEBRIS",
		"METEOROID",
		"PLANETOID",
		"MOON",
		"PLANET",
		"GAS_GIANT",
		"RED_DWARF",
		"G_TYPE",
		"RED_GIANT",
		"RED_SUPERGIANT",
		"BLACK_HOLE"
	};
	private int currentFormIndex = 0;

	public DebrisData CurrentForm => DebrisManager.Instance.GetDebrisType(playerForms[currentFormIndex]);
	public DebrisData NextForm => DebrisManager.Instance.GetDebrisType(playerForms[currentFormIndex + 1]);

	public override void _Ready() {
		base._Ready();

		DebrisManager.Instance.ClearActiveDebris();
	}

	public override void _PhysicsProcess(double delta) {

		if (acceptInputs) {
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
				ShakeCamera(100, 100);
				GameManager.GameScale += consumedMass;

				if (GameManager.GameScale >= NextForm.Mass) {
					DebrisData currentFormData = CurrentForm;

					Color[] colours = currentFormData.ParticleColours;
					CustomParticles.Instance.SpawnParticles(GlobalPosition, 100, 10, colours, this);
					currentFormIndex++;
					sprite.Texture = ResourceLoader.Load<Texture2D>(CurrentForm.TexturePath);
				}
			}

			Node2D collider = DebrisManager.Instance.CheckCollision(this, Range);
			if (collider != null) {
				ShakeCamera(100, 100);
				acceptInputs = false;
				Visible = false;
				CustomParticles.Instance.SpawnParticles(GlobalPosition, 100, 10, CurrentForm.ParticleColours, collider);
				DebrisManager.Instance.ClearActiveDebris();

				GetTree().Root.GetChild(0).GetNode<SceneTransition>("SceneBaseResources/SceneTransition").ReloadScene(3f);
			}
		}
	}

	public void ShakeCamera(float strength, float intencity) {
		camera.Shake(strength, intencity);
	}
}
