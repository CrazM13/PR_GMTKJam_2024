using Godot;
using System;
using System.Security.AccessControl;

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


		return camera.GlobalPosition + (direction * 1750);

	}

	private void AttemptSpawn() {

		Vector2 spawnPosition = GetSpawnPosition();

		DebrisManager manager = DebrisManager.Instance;

		int maxWeight = 0;
		int maxOptions = manager.GetDebrisTypeCount();

		int[] weights = new int[maxOptions];

		for (int i = 0; i < maxOptions; i++) {

			DebrisData currentOption = manager.GetDebrisType(i);

			if (currentOption.Mass < GameManager.CurrentMass * 4f && currentOption.Mass > GameManager.CurrentMass * 0.25f) {
				weights[i] = 10;
			}
			else if(currentOption.Mass < GameManager.CurrentMass * 8f && currentOption.Mass > GameManager.CurrentMass * 0.0625f) {
				weights[i] = 1;
			} else {
				weights[i] = 0;
			}
			
			maxWeight += weights[i];
		}

		int rand = rng.RandiRange(0, maxWeight - 1);
		int selected = -1;
		for (int i = 0; i < maxOptions; i++) {
			if (rand < weights[i]) {
				selected = i;
				break;
			}

			rand -= weights[i];
		}

		Node2D newNode = manager.SpawnDebris(manager.GetDebrisType(selected));
		AddChild(newNode);
		newNode.GlobalPosition = spawnPosition;
	}

	private void UpdateDebris() {
		if (DebrisManager.Instance.GetActiveDebrisCount() < maxDebris) AttemptSpawn();
		DebrisManager.Instance.CullDebris(camera.GlobalPosition, 2500);
	}

}
