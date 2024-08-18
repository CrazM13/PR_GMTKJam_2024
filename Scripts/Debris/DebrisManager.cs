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
		AddDebrisType(new DebrisData("ALPHA_PARTICLE_A", "res://Assets/Textures/Sprites/Alpha Particle.png", 1, new Color("#A8A285"), new Color("#797979"), new Color("#E63925"), new Color("#DE0000")));

		AddDebrisType(new DebrisData("GRAIN_OF_SAND_A", "res://Assets/Textures/Sprites/Grain of Sand.png", 4, new Color("#FDEE9D")));

		AddDebrisType(new DebrisData("STONE_A", "res://Assets/Textures/Sprites/Stone.png", 16, new Color("#55B131"), new Color("#C3E3B6")));

		AddDebrisType(new DebrisData("BOULDER_A", "res://Assets/Textures/Sprites/Boulder.png", 64, new Color("#71736A"), new Color("#6D6A5B")));

		AddDebrisType(new DebrisData("SPACE_DEBRIS_A", "res://Assets/Textures/Sprites/Space Debris.png", 256, new Color("#88C66F"), new Color("#6D6658"), new Color("#FEED98")));

		AddDebrisType(new DebrisData("METEOROID_A", "res://Assets/Textures/Sprites/Meteoroid.png", 1024, new Color("#71736A"), new Color("#6D6A5B")));

		AddDebrisType(new DebrisData("PLANETOID_A", "res://Assets/Textures/Sprites/Planetoid.png", 4096, new Color("#71736A"), new Color("#6D6A5B")));

		AddDebrisType(new DebrisData("MOON_A", "res://Assets/Textures/Sprites/Moon_A.png", 16384, new Color("#EFFFFB"), new Color("#C2CECB")));
		AddDebrisType(new DebrisData("MOON_B", "res://Assets/Textures/Sprites/Moon_B.png", 16384, new Color("#FEFAA9"), new Color("#DDCE65"), new Color("#BFA950")));
		
		AddDebrisType(new DebrisData("PLANET_A", "res://Assets/Textures/Sprites/Planet_A.png", 65536, new Color("#D37474"), new Color("#CE5454"), new Color("#774141")));
		AddDebrisType(new DebrisData("PLANET_B", "res://Assets/Textures/Sprites/Planet_B.png", 65536, new Color("#0220FF"), new Color("#3B9357"), new Color("#071EC2")));
		
		AddDebrisType(new DebrisData("GAS_GIANT_A", "res://Assets/Textures/Sprites/Gas_Giant_A.png", 262144, new Color("#93B2FF"), new Color("#446FE5"), new Color("#3B60C6")));
		
		AddDebrisType(new DebrisData("RED_DWARF_A", "res://Assets/Textures/Sprites/Red_Dwarf_A.png", 1048576, new Color("#FF1C1C"), new Color("#FF5151"), new Color("#EF0000")).AddPassiveParticles(new Color("#FF1C1C"), new Color("#FF5151"), new Color("#EF0000")));
		
		AddDebrisType(new DebrisData("G_TYPE_A", "res://Assets/Textures/Sprites/G_Type_A.png", 4194304, new Color("#FDFF93"), new Color("#FFE07C"), new Color("#FFD54C")).AddPassiveParticles(new Color("#FDFF93"), new Color("#FFE07C"), new Color("#FFD54C")));
		
		AddDebrisType(new DebrisData("RED_GIANT_A", "res://Assets/Textures/Sprites/Red_Giant_A.png", 16777216, new Color("#FF1C1C"), new Color("#FF5151"), new Color("#EF0000")).AddPassiveParticles(new Color("#FF1C1C"), new Color("#FF5151"), new Color("#EF0000")));
		
		AddDebrisType(new DebrisData("RED_SUPERGIANT_A", "res://Assets/Textures/Sprites/Red_Supergiant_A.png", 67108864, new Color("#FF1C1C"), new Color("#FF5151"), new Color("#EF0000")).AddPassiveParticles(new Color("#FF1C1C"), new Color("#FF5151"), new Color("#EF0000")));
		
		AddDebrisType(new DebrisData("BLACK_HOLE_A", "res://Assets/Textures/Sprites/Black_Hole_A.png", 268435456, new Color("#EFFFFB")).AddPassiveParticles(new Color("#A8A285"), new Color("#797979"), new Color("#E63925"), new Color("#DE0000")));
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

	public int GetDebrisTypeCount() => debrisTypes.Count;

	public void CullDebris(Vector2 center, float maxDistance) {
		List<uint> toRemove = new List<uint>();

		foreach (KeyValuePair<uint, DebrisNode> debris in activeDebris) {

			if (debris.Value.TargetScale <= 0.1f || debris.Value.TargetScale >= 25f || debris.Value.GlobalPosition.DistanceSquaredTo(center) > maxDistance * maxDistance) {
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
		node.TargetScale = node.Data.Mass / GameManager.GameScale;

		activeDebris.Add(node.DebrisID, node);

		return node;
	}

	public int GetActiveDebrisCount() => activeDebris.Count;

	public void ScaleDebris() {
		foreach (KeyValuePair<uint, DebrisNode> debris in activeDebris) {
			debris.Value.TargetScale = debris.Value.Data.Mass / GameManager.GameScale;
		}
	}

	public float AttemptConsume(PlayerMovement consumer, float maxDistance) {
		float consumedMass = 0;

		List<uint> toRemove = new List<uint>();

		foreach (KeyValuePair<uint, DebrisNode> debris in activeDebris) {
			if (debris.Value.Data.Mass <= GameManager.CurrentMass && debris.Value.GlobalPosition.DistanceTo(consumer.GlobalPosition) < maxDistance) {
				toRemove.Add(debris.Key);
				consumedMass += debris.Value.Data.Mass;
			}
		}

		foreach (uint key in toRemove) {
			CustomParticles.Instance.SpawnParticles(activeDebris[key].GlobalPosition, Mathf.CeilToInt(50 * activeDebris[key].TargetScale), 100, 1, activeDebris[key].Data.ParticleColours, consumer);

			consumer.PlayBreakingSFX();

			activeDebris[key].QueueFree();
			activeDebris.Remove(key);
		}

		return consumedMass;
	}

	public Node2D CheckCollision(PlayerMovement consumer, float maxDistance) {

		foreach (KeyValuePair<uint, DebrisNode> debris in activeDebris) {
			if (debris.Value.Data.Mass > GameManager.CurrentMass && debris.Value.GlobalPosition.DistanceTo(consumer.GlobalPosition) < Mathf.Min(maxDistance * debris.Value.TargetScale, 500)) {
				return debris.Value;
			}
		}

		return null;
	}

	public void DisplayNearbyEffect(PlayerMovement consumer, float maxDistance, float chance) {
		foreach (KeyValuePair<uint, DebrisNode> debris in activeDebris) {
			if (GD.Randf() > 1 - chance) {
				if (debris.Value.Data.Mass <= GameManager.CurrentMass) {
					if (debris.Value.GlobalPosition.DistanceTo(consumer.GlobalPosition) < maxDistance * consumer.TargetScale) {
						CustomParticles.Instance.SpawnParticles(debris.Value.GlobalPosition, 1, 10, 1, debris.Value.Data.ParticleColours, consumer);
					}
				} else {
					if (debris.Value.GlobalPosition.DistanceTo(consumer.GlobalPosition) < Mathf.Min(maxDistance * debris.Value.TargetScale, 500)) {
						CustomParticles.Instance.SpawnParticles(consumer.GlobalPosition, 1, 10, 1, consumer.CurrentForm.ParticleColours, debris.Value);
						consumer.ShakeCamera(5f, 100f);

						DebrisData currentFormData = consumer.CurrentForm;
						if (GameManager.CurrentMass > currentFormData.Mass) {
							GameManager.CurrentMass = Mathf.Max(GameManager.CurrentMass - 0.01f, currentFormData.Mass);
						}
					}
				}
			}
		}
	}

	public void ClearActiveDebris() {
		activeDebris.Clear();
	}

}
