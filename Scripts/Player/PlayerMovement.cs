using Godot;
using System;

public partial class PlayerMovement : Node2D {
	[Export] private float Speed = 300.0f;
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
		"ALPHA_PARTICLE_",
		"GRAIN_OF_SAND_",
		"STONE_",
		"BOULDER_",
		"SPACE_DEBRIS_",
		"METEOROID_",
		"PLANETOID_",
		"MOON_",
		"PLANET_",
		"GAS_GIANT_",
		"RED_DWARF_",
		"G_TYPE_",
		"RED_GIANT_",
		"RED_SUPERGIANT_",
		"BLACK_HOLE_"
	};
	private int currentFormIndex = 0;
	private string variant = "A";

	public DebrisData CurrentForm {
		get {
			return DebrisManager.Instance.GetDebrisType(playerForms[currentFormIndex] + variant);
		}
	}
	public DebrisData NextForm {
		get {

			int nextIndex = (currentFormIndex + 1);

			if (nextIndex < playerForms.Length) return DebrisManager.Instance.GetDebrisType(playerForms[nextIndex] + "A");
			else return null;
		}
	}
	public int GetFormIndex => currentFormIndex;

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

		GameManager.Seed = GD.Randf();

		for (int i = 1; i < playerForms.Length; i++) {
			if (NextForm != null && NextForm.Mass <= GameManager.CurrentMass) {
				currentFormIndex++;
				EmitSignal(SignalName.OnLevelUp);
			} else {
				break;
			}
		}

		if (GameManager.Seed < 0.25f && DebrisManager.Instance.DoesDebrisTypeExist(playerForms[currentFormIndex] + "B")) {
			variant = "B";
		} else {
			variant = "A";
		}

		sprite.Texture = ResourceLoader.Load<Texture2D>(CurrentForm.TexturePath);
		TargetScale = GameManager.CurrentMass / GameManager.GameScale;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		GameManager.GameTime += (float) delta;

		if (scaleTime < 1) {
			float currentScale = Scale.X;
			scaleTime += (float) delta;
			currentScale = Mathf.Lerp(oldScale, targetScale, scaleTime);
			Scale = new Vector2(currentScale, currentScale);
			oldScale = Scale.X;
		}

		if ((float) delta != 0) emmisionFrames++;
		if (emmisionFrames == 5) {
			DebrisData currentFormData = CurrentForm;
			if (currentFormData.PassiveParticleColours != null) {
				CustomParticles.Instance.SpawnParticles(GlobalPosition + (new Vector2(MathF.Cos(GameManager.GameTime * 10), Mathf.Sin(GameManager.GameTime * 10)) * 13), 1, 10, 1, currentFormData.PassiveParticleColours);
			}

			emmisionFrames = 0;
		}

		if (GameManager.AllowCheats) {
			bool isCheatKeyNowPressed = Input.IsKeyPressed(Key.Equal);
			if (isCheatKeyNowPressed && !isCheatkeyPressed) {
				AddMass(CurrentForm.Mass * GameManager.ConsumptionEfficiency);
			}
			isCheatkeyPressed = isCheatKeyNowPressed;
		}
	}
	private bool isCheatkeyPressed = false;

	public override void _PhysicsProcess(double delta) {

		if (acceptInputs) {

			if (!GameManager.IsGamePaused) {
				if (Input.IsActionPressed("time_sprint")) {
					Engine.TimeScale = 2f;
				} else if (Input.IsActionPressed("time_crawl")) {
					Engine.TimeScale = 0.5f;
				} else {
					Engine.TimeScale = 1;
				}
			}

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

			if (!GameManager.IsGamePaused) InteractWithDebris();
		}

		if (currentFormIndex == playerForms.Length - 1 && !acceptInputs) {
			CustomParticles.Instance.SpawnParticles(GlobalPosition, 100, 400, 1, CurrentForm.PassiveParticleColours);
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

		if (GameManager.Seed < 0.25f && DebrisManager.Instance.DoesDebrisTypeExist(playerForms[currentFormIndex] + "B")) {
			variant = "B";
		} else {
			variant = "A";
		}

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

		float consumedMass = DebrisManager.Instance.AttemptConsume(this, Range * TargetScale) * GameManager.ConsumptionEfficiency;
		if (consumedMass > 0) {
			ShakeCamera(100, 100);
			AddMass(consumedMass);
		}

		Node2D collider = DebrisManager.Instance.CheckCollision(this, Range);
		if (collider != null) {
			ShakeCamera(100, 100);
			PlayBreakingSFX();
			acceptInputs = false;
			Visible = false;
			CustomParticles.Instance.SpawnParticles(GlobalPosition, 100, 150, 1, CurrentForm.ParticleColours, collider);
			DebrisManager.Instance.ClearActiveDebris();

			GameManager.CurrentMass = DebrisManager.Instance.GetDebrisType(playerForms[0] + "A").Mass;
			GameManager.GameScale = GameManager.CurrentMass;

			GetTree().Root.GetChild(0).GetNode<SceneTransition>("SceneBaseResources/SceneTransition").ReloadScene(3f);
		}
	}

	private void Win() {
		Engine.TimeScale = 0.25f;
		TargetScale = 0;
		acceptInputs = false;
		GetTree().Root.GetChild(0).GetNode<SceneTransition>("SceneBaseResources/SceneTransition").ReloadScene(1f);

		GameManager.CurrentMass = DebrisManager.Instance.GetDebrisType(playerForms[0] + "A").Mass;
		GameManager.GameScale = GameManager.CurrentMass;
		GameManager.Level++;
	}

	public override void _Draw() {
		base._Draw();

		if (GameManager.DisplayHitboxSetting > 0) DrawCircle(Vector2.Zero, GameManager.DefaultReachDistance, new Color(0.7f, 0.7f, 1f, 0.25f));
		if (GameManager.DisplayHitboxSetting > 1) DrawCircle(Vector2.Zero, GameManager.DefaultReachDistance * 4, new Color(0.7f, 0.7f, 1f, 0.125f));
	}

	private void AddMass(float mass) {
		GameManager.CurrentMass += mass;
		EmitSignal(SignalName.OnMassChange);

		if (NextForm != null && GameManager.CurrentMass >= NextForm.Mass) {
			LevelUp();
		} else {
			TargetScale = GameManager.CurrentMass / GameManager.GameScale;

			if (currentFormIndex == playerForms.Length - 1 && TargetScale > 1.1f) {
				Win();
			}
		}
	}
}
