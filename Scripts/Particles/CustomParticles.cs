using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class CustomParticles : Node2D {

	public static CustomParticles Instance { get; private set; }

	public override void _Ready() {
		base._Ready();

		Instance = this;

		particles = new List<Particle>();
		rng = new RandomNumberGenerator();
		rng.Randomize();
	}

	private class Particle {
		public Vector2 Position { get; set; }
		public Vector2 Velocity { get; set; }
		public float Scale { get; set; }

		public float Age { get; set; }
		public Color Colour { get; set; }

		public Node2D Attractor { get; set; }
	}

	private List<Particle> particles;
	private RandomNumberGenerator rng;

	private int numOfParticlesSpawned = 0;

	public void SpawnParticles(Vector2 position, int count, float speed, float scale, Color[] colors, Node2D attractor = null) {
		int cOffset = rng.RandiRange(0, colors.Length);

		for (int i = 0; i < count; i++) {
			numOfParticlesSpawned++;

			float offsetX = rng.Randf();
			float offsetY = rng.Randf();
			float angle = rng.RandfRange(-Mathf.Pi, Mathf.Pi);

			Particle particle = new Particle {
				Position = position + new Vector2(offsetX, offsetY),
				Velocity = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)) * speed,
				Scale = scale,
				Age = 0,
				Colour = colors[(cOffset + i) % colors.Length],
				Attractor = attractor
			};

			if (GameManager.ParticlesAmount != 0 && (numOfParticlesSpawned % GameManager.ParticlesAmount) == 0) particles.Add(particle);
		}

		QueueRedraw();
	}

	public override void _Process(double delta) {

		bool needsRedraw = particles.Count > 0;

		for (int i = particles.Count - 1; i >= 0; i--) {
			Particle particle = particles[i];

			if (particle.Attractor != null && IsInstanceValid(particle.Attractor)) {
				Vector2 direction = particle.Attractor.GlobalPosition - particle.Position;

				particle.Velocity += direction.Normalized() * 100 * (float) delta * (rng.Randf() + 0.5f);

				if (particle.Age > 3f) {
					particle.Velocity *= 0.99f;
					particle.Velocity += direction * (float) delta;
				}

				if (particle.Age > 6f) {
					particle.Velocity *= 0.99f;
					particle.Velocity += direction * (float) delta * 4;
				}

				if (particle.Age > 0.25f && particle.Position.DistanceSquaredTo(particle.Attractor.GlobalPosition) < 256f * particle.Attractor.GlobalScale.X) {
					particle.Age += 100f;
				}
			}

			particle.Position += particle.Velocity * (float) delta;
			particle.Age += (float) delta;

			if (particle.Age > 10f) {
				particles.RemoveAt(i);
			}
		}

		if (needsRedraw) QueueRedraw();

	}

	public override void _Draw() {
		foreach (Particle particle in particles) {
			DrawParticle(particle);
		}
	}

	private void DrawParticle(Particle particle) {
		DrawRect(new Rect2(particle.Position, Vector2.One * 4 * particle.Scale), particle.Colour.Lerp(new Color(particle.Colour.R, particle.Colour.G, particle.Colour.B, 0), Mathf.Max(0, (particle.Age - 9) / 1)));
	}

}
