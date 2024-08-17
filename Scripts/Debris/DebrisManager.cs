using Godot;
using System;
using System.Collections.Generic;

public class DebrisManager {

	private const string PATH_TO_PREFAB = "res://Assets/Prefabs/base_debris.tscn";

	private static uint nextId = 0;
	public static uint GetNewDebrisID() => nextId++;

	private static DebrisManager instance;
	public static DebrisManager Instance {
		get {
			instance ??= new DebrisManager();

			return instance;
		}
	}

	private DebrisManager() {
		debrisTypes = new Dictionary<string, DebrisData>();
		activeDebris = new Dictionary<uint, DebrisNode>();

		InitDebrisTypes();
	}

	private void InitDebrisTypes() {
		AddDebrisType(new DebrisData("ALPHA_PARTICLE", "res://Assets/Textures/Sprites/16x16Placeholder.png", 100, Colors.Purple));
		AddDebrisType(new DebrisData("GRAIN_OF_SAND", "res://Assets/Textures/Sprites/16x16Placeholder.png", 400, Colors.Purple));
		AddDebrisType(new DebrisData("STONE", "res://Assets/Textures/Sprites/16x16Placeholder.png", 1600, Colors.Purple));
		AddDebrisType(new DebrisData("BOULDER", "res://Assets/Textures/Sprites/16x16Placeholder.png", 6400, Colors.Purple));
		AddDebrisType(new DebrisData("SPACE_DEBRIS", "res://Assets/Textures/Sprites/16x16Placeholder.png", 25600, Colors.Purple));
		AddDebrisType(new DebrisData("METEOROID", "res://Assets/Textures/Sprites/16x16Placeholder.png", 102400, Colors.Purple));
		AddDebrisType(new DebrisData("PLANETOID", "res://Assets/Textures/Sprites/16x16Placeholder.png", 409600, Colors.Purple));
		AddDebrisType(new DebrisData("MOON", "res://Assets/Textures/Sprites/Moon_A.png", 1638400, new Color("#EFFFFB"), new Color("#C2CECB")));
		AddDebrisType(new DebrisData("PLANET", "res://Assets/Textures/Sprites/16x16Placeholder.png", 6553600, Colors.Purple));
		AddDebrisType(new DebrisData("GAS_GIANT", "res://Assets/Textures/Sprites/16x16Placeholder.png", 26214400, Colors.Purple));
		AddDebrisType(new DebrisData("RED_DWARF", "res://Assets/Textures/Sprites/16x16Placeholder.png", 104857600, Colors.Purple));
		AddDebrisType(new DebrisData("G_TYPE", "res://Assets/Textures/Sprites/16x16Placeholder.png", 419430400, Colors.Purple));
		AddDebrisType(new DebrisData("RED_GIANT", "res://Assets/Textures/Sprites/16x16Placeholder.png", 1677721600, Colors.Purple));
		AddDebrisType(new DebrisData("RED_SUPERGIANT", "res://Assets/Textures/Sprites/16x16Placeholder.png", 6710886400, Colors.Purple));
		AddDebrisType(new DebrisData("BLACK_HOLE", "res://Assets/Textures/Sprites/16x16Placeholder.png", 26843545600, Colors.Purple));
	}

	private Dictionary<string, DebrisData> debrisTypes;
	private Dictionary<uint, DebrisNode> activeDebris;

	public DebrisData AddDebrisType(DebrisData debris) {
		if (debrisTypes.ContainsKey(debris.Name)) {
			debrisTypes[debris.Name] = debris;
		} else {
			debrisTypes.Add(debris.Name, debris);
		}

		return debris;
	}

	public DebrisData GetDebrisType(string debrisName) {
		if (debrisTypes.ContainsKey(debrisName)) {
			return debrisTypes[debrisName];
		}

		return null;
	}

	public DebrisData GetDebrisType(int index) {
		index %= debrisTypes.Count;
		string[] keys = new string[debrisTypes.Count];
		debrisTypes.Keys.CopyTo(keys, 0);
		return debrisTypes[keys[index]];
	}

	public void CullDebris(Vector2 center, float maxDistance) {
		List<uint> toRemove = new List<uint>();

		foreach (KeyValuePair<uint, DebrisNode> debris in activeDebris) {

			if (debris.Value.TargetScale <= 0.25f || debris.Value.TargetScale >= 25f || debris.Value.GlobalPosition.DistanceSquaredTo(center) > maxDistance * maxDistance) {
				toRemove.Add(debris.Key);
			}
		}

		foreach (uint key in toRemove) {
			activeDebris[key].QueueFree();
			activeDebris.Remove(key);
		}
	}

	public DebrisNode SpawnDebris(string type) {
		return SpawnDebris(GetDebrisType(type));
	}

	public DebrisNode SpawnDebris(DebrisData type) {
		PackedScene prefab = ResourceLoader.Load<PackedScene>(PATH_TO_PREFAB);

		DebrisNode node = prefab.Instantiate<DebrisNode>();

		node.DebrisID = GetNewDebrisID();
		node.SetDebrisType(type);

		activeDebris.Add(node.DebrisID, node);

		return node;
	}

	public int GetActiveDebrisCount() => activeDebris.Count;

	public void ScaleDebris() {
		foreach (KeyValuePair<uint, DebrisNode> debris in activeDebris) {
			debris.Value.TargetScale = debris.Value.Data.Mass / GameManager.GameScale;
		}
	}

	public float AttemptConsume(Node2D consumer, float maxDistance) {
		float consumedMass = 0;

		List<uint> toRemove = new List<uint>();

		foreach (KeyValuePair<uint, DebrisNode> debris in activeDebris) {
			if (debris.Value.Data.Mass <= GameManager.GameScale && debris.Value.GlobalPosition.DistanceTo(consumer.GlobalPosition) < maxDistance) {
				toRemove.Add(debris.Key);
				consumedMass += debris.Value.Data.Mass;
			}
		}

		foreach (uint key in toRemove) {
			CustomParticles.Instance.SpawnParticles(activeDebris[key].GlobalPosition, 100, 100, activeDebris[key].Data.ParticleColours, consumer);

			activeDebris[key].QueueFree();
			activeDebris.Remove(key);
		}

		return consumedMass;
	}

	public Node2D CheckCollision(PlayerMovement consumer, float maxDistance) {

		foreach (KeyValuePair<uint, DebrisNode> debris in activeDebris) {
			if (debris.Value.Data.Mass > GameManager.GameScale && debris.Value.GlobalPosition.DistanceTo(consumer.GlobalPosition) < Mathf.Min(maxDistance * debris.Value.TargetScale, 500)) {
				return debris.Value;
			}
		}

		return null;
	}

	public void DisplayNearbyEffect(PlayerMovement consumer, float maxDistance, float chance) {
		foreach (KeyValuePair<uint, DebrisNode> debris in activeDebris) {
			if (GD.Randf() > 1 - chance) {
				if (debris.Value.Data.Mass <= GameManager.GameScale) {
					if (debris.Value.GlobalPosition.DistanceTo(consumer.GlobalPosition) < maxDistance) {
						CustomParticles.Instance.SpawnParticles(debris.Value.GlobalPosition, 1, 10, debris.Value.Data.ParticleColours, consumer);
					}
				} else {
					if (debris.Value.GlobalPosition.DistanceTo(consumer.GlobalPosition) < Mathf.Min(maxDistance * debris.Value.TargetScale, 500)) {
						CustomParticles.Instance.SpawnParticles(consumer.GlobalPosition, 1, 10, consumer.CurrentForm.ParticleColours, debris.Value);
						consumer.ShakeCamera(5f, 100f);
					}
				}
			}
		}
	}

	public void ClearActiveDebris() {
		activeDebris.Clear();
	}

}
