using Godot;
using System;

public partial class DebrisNode : Node2D {

	public uint DebrisID { get; set; }

	[Export] private Sprite2D sprite;
	[Export] private float speed;

	public Vector2 Velocity { get; set; }

	public DebrisData Data { get; set; }
	public float TargetScale {
		get => targetScale;
		set {
			oldScale = targetScale;
			targetScale = value;
			scaleTime = 0;
		}
	}

	private float targetScale = 1;
	private float oldScale = 0;
	private float scaleTime = 0;

	private int emmisionFrames = 0;

	#region Shake
	private float decay = 0.9f;
	private float intensity = 0.9f;

	private float shakeStrength;
	private float shakeTime = 0;
	private float intensityMod = 1;
	#endregion

	public override void _Ready() {
		base._Ready();

		Scale = new Vector2(0, 0);
		float heading = (float) GD.RandRange(-Mathf.Pi, Mathf.Pi);
		Velocity = new Vector2(Mathf.Cos(heading), Mathf.Sin(heading)) * speed;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		this.Position += Velocity * (float) delta;

		if (scaleTime < 1) {
			float currentScale = Scale.X;
			scaleTime += (float) delta;
			currentScale = Mathf.Lerp(oldScale, targetScale, scaleTime);
			Scale = new Vector2(currentScale, currentScale);
		}

		if ((float) delta != 0) emmisionFrames++;
		if (emmisionFrames == 32) {
			if (TargetScale >= 1 && Data.PassiveParticleColours != null) {
				CustomParticles.Instance.SpawnParticles(GlobalPosition, 1, 100 + (50 * TargetScale * 0.5f), Mathf.Max(1, TargetScale * 0.5f), Data.PassiveParticleColours, this);
			}

			emmisionFrames = 0;
		}

		UpdateShake((float) delta);
	}

	private void UpdateShake(float delta) {
		if (shakeStrength > 0.25f) {
			shakeTime += delta * intensity * intensityMod;

			float x = Mathf.Sin(shakeTime * 0.7f) * shakeStrength;
			float y = Mathf.Cos(shakeTime * 0.3f) * shakeStrength;

			sprite.Position = new Vector2(x, y);

			shakeStrength *= decay;

			if (shakeStrength <= 0.25f) {
				shakeTime = 0;
			}
		}
	}

	public void Shake(float strength, float intensity = 1) {
		shakeStrength = Mathf.Max(shakeStrength, strength);
		intensityMod = intensity;
	}

	public void SetDebrisType(DebrisData data) {
		sprite.Texture = ResourceLoader.Load<Texture2D>(data.TexturePath);

		Data = data;
	}

	public override void _Draw() {
		base._Draw();

		if (GameManager.DisplayHitboxSetting > 0) {
			DrawCircle(Vector2.Zero, GameManager.DefaultReachDistance, new Color(1f, 0.7f, 0.7f, 0.25f));
		}

		if (GameManager.DisplayHitboxSetting > 1) {

			float interactionDistance = GameManager.DefaultReachDistance * 4;
			float simulatedInteractionDistance = interactionDistance * TargetScale;
			if (simulatedInteractionDistance > 500) {
				interactionDistance = 500 / TargetScale;
			}

			DrawCircle(Vector2.Zero, interactionDistance, new Color(1f, 0.7f, 0.7f, 0.125f));
		}

	}
}
