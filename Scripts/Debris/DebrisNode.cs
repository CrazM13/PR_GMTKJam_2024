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

		if (Data.PassiveParticleColours != null) {
			CustomParticles.Instance.SpawnParticles(GlobalPosition, 1, 10, Data.PassiveParticleColours);
		}
	}

	public void SetDebrisType(DebrisData data) {
		sprite.Texture = ResourceLoader.Load<Texture2D>(data.TexturePath);

		Data = data;
	}

}
