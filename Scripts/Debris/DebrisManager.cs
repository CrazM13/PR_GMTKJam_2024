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
		AddDebrisType(new DebrisData("ALPHA_PARTICLE", "res://Assets/Textures/Sprites/Alpha Particle.png", 100, new Color("#A8A285"), new Color("#797979"), new Color("#E63925"), new Color("#DE0000")));
		AddDebrisType(new DebrisData("GRAIN_OF_SAND", "res://Assets/Textures/Sprites/16x16Placeholder.png", 400, Colors.Purple));
		AddDebrisType(new DebrisData("STONE", "res://Assets/Textures/Sprites/16x16Placeholder.png", 1600, Colors.Purple));
		AddDebrisType(new DebrisData("BOULDER", "res://Assets/Textures/Sprites/16x16Placeholder.png", 6400, Colors.Purple));
		AddDebrisType(new DebrisData("SPACE_DEBRIS", "res://Assets/Textures/Sprites/16x16Placeholder.png", 25600, Colors.Purple));
		AddDebrisType(new DebrisData("METEOROID", "res://Assets/Textures/Sprites/16x16Placeholder.png", 102400, Colors.Purple));
		AddDebrisType(new DebrisData("PLANETOID", "res://Assets/Textures/Sprites/16x16Placeholder.png", 409600, Colors.Purple));
		AddDebrisType(new DebrisData("MOON", "res://Assets/Textures/Sprites/Moon_A.png", 1638400, new Color("#EFFFFB"), new Color("#C2CECB")));
		AddDebrisType(new DebrisData("PLANET", "res://Assets/Textures/Sprites/Planet_A.png", 6553600, new Color("#D37474"), new Color("#CE5454"), new Color("#774141")));
		AddDebrisType(new DebrisData("GAS_GIANT", "res://Assets/Textures/Sprites/Gas_Giant_A.png", 26214400, new Color("#93B2FF"), new Color("#446FE5"), new Color("#3B60C6")));
		AddDebrisType(new DebrisData("RED_DWARF", "res://Assets/Textures/Sprites/Red_Dwarf_A.png", 104857600, new Color("#FF1C1C"), new Color("#FF5151"), new Color("#EF0000")).AddPassiveParticles(new Color("#FF1C1C"), new Color("#FF5151"), new Color("#EF0000")));
		AddDebrisType(new DebrisData("G_TYPE", "res://Assets/Textures/Sprites/G_Type_A.png", 419430400, new Color("#FDFF93"), new Color("#FFE07C"), new Color("#FFD54C")).AddPassiveParticles(new Color("#FDFF93"), new Color("#FFE07C"), new Color("#FFD54C")));
		AddDebrisType(new DebrisData("RED_GIANT", "res://Assets/Textures/Sprites/Red_Giant_A.png", 1677721600, new Color("#FF1C1C"), new Color("#FF5151"), new Color("#EF0000")).AddPassiveParticles(new Color("#FF1C1C"), new Color("#FF5151"), new Color("#EF0000")));
		AddDebrisType(new DebrisData("RED_SUPERGIANT", "res://Assets/Textures/Sprites/Red_Supergiant_A.png", 6710886400, new Color("#FF1C1C"), new Color("#FF5151"), new Color("#EF0000")).AddPassiveParticles(new Color("#FF1C1C"), new Color("#FF5151"), new Color("#EF0000")));
		AddDebrisType(new DebrisData("BLACK_HOLE", "res://Assets/Textures/Sprites/16x16Placeholder.png", 26843545600, new Color("#EFFFFB")).AddPassiveParticles(new Color("#A8A285"), new Color("#797979"), new Color("#E63925"), new Color("#DE0000")));
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
			CustomParticles.Instance.SpawnParticles(activeDebris[key].GlobalPosition, Mathf.CeilToInt(100 * activeDebris[key].TargetScale), 100, activeDebris[key].Data.ParticleColours, consumer);

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
