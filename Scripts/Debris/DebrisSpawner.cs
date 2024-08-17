using Godot;
using System;

public partial class DebrisSpawner : Timer {

	[Export] private float gridScale = 1f;
	[Export] private int maxDebris = 100;
	[Export] private Node2D camera;

	private RandomNumberGenerator rng;

	public override void _Ready() {
		base._Ready();

		rng = new RandomNumberGenerator();
	}

	private Vector2 GetSpawnPosition() {

		float angle = rng.RandfRange(-Mathf.Pi, Mathf.Pi);
		Vector2 direction = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle));


		return camera.GlobalPosition + (direction * 750);

	}

	private void AttemptSpawn() {

		Vector2 spawnPosition = GetSpawnPosition();

		DebrisManager manager = DebrisManager.Instance;

		Node2D newNode = manager.SpawnDebris(manager.GetDebrisType(rng.RandiRange(0, 100)));
		AddChild(newNode);
		newNode.GlobalPosition = spawnPosition;
	}

	private void UpdateDebris() {
		if (DebrisManager.Instance.GetActiveDebrisCount() < maxDebris) AttemptSpawn();
		DebrisManager.Instance.ScaleDebris();
		DebrisManager.Instance.CullDebris(camera.GlobalPosition, 1500);
	}

}
