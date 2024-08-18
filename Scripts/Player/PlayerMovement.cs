using Godot;
using System;

public partial class PlayerMovement : Node2D {
	[Export] private float Speed = 300.0f;
	[Export] private float consumptionEffeciency;
	[Export] private Sprite2D sprite;
	[Export] private ScaleCamera camera;
	[Export] private AudioStreamPlayer2D audio;

	public float Range { get; set; } = 16;
	public float TargetScale {
		get => targetScale;
		set {
			oldScale = Scale.X;
			targetScale = value;
			scaleTime = 0;
		}
	}

	private float targetScale = 1;
	private float oldScale = 0;
	private float scaleTime = 0;

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

	public DebrisData CurrentForm {
		get {
			return DebrisManager.Instance.GetDebrisType(playerForms[currentFormIndex]);
		}
	}
	public DebrisData NextForm {
		get {

			int nextIndex = (currentFormIndex + 1);

			if (nextIndex < playerForms.Length) return DebrisManager.Instance.GetDebrisType(playerForms[nextIndex]);
			else return null;
		}
	}

	private int emmisionFrames = 0;

	#region Signals
	[Signal] public delegate void OnLevelUpEventHandler();
	[Signal] public delegate void OnMassChangeEventHandler();
	#endregion

	public override void _Ready() {
		base._Ready();

		Scale = new Vector2(0, 0);
		DebrisManager.Instance.ClearActiveDebris();

		EmitSignal(SignalName.OnLevelUp);
		EmitSignal(SignalName.OnMassChange);
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (scaleTime < 1) {
			float currentScale = Scale.X;
			scaleTime += (float) delta;
			currentScale = Mathf.Lerp(oldScale, targetScale, scaleTime);
			Scale = new Vector2(currentScale, currentScale);
			oldScale = Scale.X;
		}

		emmisionFrames++;
		if (emmisionFrames == 10) {
			DebrisData currentFormData = CurrentForm;
			if (currentFormData.PassiveParticleColours != null) {
				CustomParticles.Instance.SpawnParticles(GlobalPosition, 1, 10, 1, currentFormData.PassiveParticleColours);
			}

			emmisionFrames = 0;
		}

		bool isCheatKeyNowPressed = Input.IsKeyPressed(Key.Equal);
		if (isCheatKeyNowPressed && !isCheatkeyPressed) {
			GameManager.CurrentMass += CurrentForm.Mass * consumptionEffeciency;
			EmitSignal(SignalName.OnMassChange);

			DebrisData nextForm = NextForm;
			if (nextForm != null && GameManager.CurrentMass >= NextForm.Mass) {
				LevelUp();
			} else {
				TargetScale = GameManager.CurrentMass / GameManager.GameScale;
			}
		}
		isCheatkeyPressed = isCheatKeyNowPressed;
	}
	private bool isCheatkeyPressed = false;

	public override void _PhysicsProcess(double delta) {

		if (acceptInputs) {
			// Get the input direction and handle the movement/deceleration.
			// As good practice, you should replace UI actions with custom gameplay actions.
			Vector2 direction = Input.GetVector("move_left", "move_right", "move_up", "move_down");
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

			InteractWithDebris();
		}
	}

	public void ShakeCamera(float strength, float intencity) {
		camera.Shake(strength, intencity);
	}

	public void PlayBreakingSFX() {
		audio.Play();
	}

	private void LevelUp() {
		TargetScale = 1;
		GameManager.GameScale = NextForm.Mass;
		DebrisManager.Instance.ScaleDebris();
		DebrisData currentFormData = CurrentForm;

		Color[] colours = currentFormData.ParticleColours;
		CustomParticles.Instance.SpawnParticles(GlobalPosition, 100, 100, 1, colours, this);
		currentFormIndex++;
		sprite.Texture = ResourceLoader.Load<Texture2D>(CurrentForm.TexturePath);

		EmitSignal(SignalName.OnLevelUp);
	}

	private void InteractWithDebris() {
		float currentMass = GameManager.CurrentMass;
		DebrisManager.Instance.DisplayNearbyEffect(this, Range * 4, 0.1f);
		if (currentMass != GameManager.CurrentMass) {
			EmitSignal(SignalName.OnMassChange);
			TargetScale = GameManager.CurrentMass / GameManager.GameScale;
		}

		float consumedMass = DebrisManager.Instance.AttemptConsume(this, Range * TargetScale) * consumptionEffeciency;
		if (consumedMass > 0) {
			ShakeCamera(100, 100);
			GameManager.CurrentMass += consumedMass;
			EmitSignal(SignalName.OnMassChange);

			if (GameManager.CurrentMass >= NextForm.Mass) {
				LevelUp();
			} else {
				TargetScale = GameManager.CurrentMass / GameManager.GameScale;
			}
		}

		Node2D collider = DebrisManager.Instance.CheckCollision(this, Range);
		if (collider != null) {
			ShakeCamera(100, 100);
			PlayBreakingSFX();
			acceptInputs = false;
			Visible = false;
			CustomParticles.Instance.SpawnParticles(GlobalPosition, 100, 10, 1, CurrentForm.ParticleColours, collider);
			DebrisManager.Instance.ClearActiveDebris();

			GameManager.CurrentMass = DebrisManager.Instance.GetDebrisType(playerForms[0]).Mass;
			GameManager.GameScale = GameManager.CurrentMass;

			GetTree().Root.GetChild(0).GetNode<SceneTransition>("SceneBaseResources/SceneTransition").ReloadScene(3f);
		}
	}
}
