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
		AddDebrisType(new DebrisData("ALPHA_PARTICLE_A", "res://Assets/Textures/Sprites/Alpha Particle.png", 1, new Color("#A8A285"), new Color("#797979"), new Color("#E63925"), new Color("#DE0000"))).AddPassiveParticles();
		AddDebrisType(new DebrisData("ALPHA_PARTICLE_B", "res://Assets/Textures/Sprites/Alpha_Particle_B.png", 1, new Color("#FF3A3D"), new Color("#CE0003"), new Color("#3251FF"), new Color("#001BB5"))).AddPassiveParticles();

		AddDebrisType(new DebrisData("GRAIN_OF_SAND_A", "res://Assets/Textures/Sprites/Grain of Sand.png", 4, new Color("#FDEE9D")));

		AddDebrisType(new DebrisData("STONE_A", "res://Assets/Textures/Sprites/Stone.png", 16, new Color("#55B131"), new Color("#C3E3B6")));
		AddDebrisType(new DebrisData("STONE_C", "res://Assets/Textures/Sprites/rock_c.png", 16, new Color("#AA492E"), new Color("#D48B73")));

		AddDebrisType(new DebrisData("BOULDER_A", "res://Assets/Textures/Sprites/Boulder.png", 64, new Color("#71736A"), new Color("#6D6A5B")));

		AddDebrisType(new DebrisData("SPACE_DEBRIS_A", "res://Assets/Textures/Sprites/Space Debris.png", 256, new Color("#88C66F"), new Color("#6D6658"), new Color("#FEED98")));
		AddDebrisType(new DebrisData("SPACE_DEBRIS_C", "res://Assets/Textures/Sprites/space_debris_c.png", 256, new Color("#A97C7C"), new Color("#7D3A3A"), new Color("#323232")));
		AddDebrisType(new DebrisData("SPACE_DEBRIS_D", "res://Assets/Textures/Sprites/space_debris_d.png", 256, new Color("#CAE2C4"), new Color("#7EBFA1"), new Color("#707070")));
		AddDebrisType(new DebrisData("SPACE_DEBRIS_E", "res://Assets/Textures/Sprites/space_debris_e.png", 256, new Color("#E1D5CF"), new Color("#D4C3BA"), new Color("#907D73")));

		AddDebrisType(new DebrisData("METEOROID_B", "res://Assets/Textures/Sprites/Meteoroid.png", 1024, new Color("#71736A"), new Color("#6D6A5B")));
		AddDebrisType(new DebrisData("METEOROID_A", "res://Assets/Textures/Sprites/meteorite_c.png", 1024, new Color("#8A9090"), new Color("#90C6C6"), new Color("#2F7474")));

		AddDebrisType(new DebrisData("PLANETOID_A", "res://Assets/Textures/Sprites/Planetoid.png", 4096, new Color("#71736A"), new Color("#6D6A5B")));

		AddDebrisType(new DebrisData("MOON_A", "res://Assets/Textures/Sprites/Moon_A.png", 16384, new Color("#EFFFFB"), new Color("#C2CECB")));
		AddDebrisType(new DebrisData("MOON_B", "res://Assets/Textures/Sprites/Moon_B.png", 16384, new Color("#FEFAA9"), new Color("#DDCE65"), new Color("#BFA950")));
		
		AddDebrisType(new DebrisData("PLANET_A", "res://Assets/Textures/Sprites/Planet_A.png", 65536, new Color("#D37474"), new Color("#CE5454"), new Color("#774141")));
		AddDebrisType(new DebrisData("PLANET_B", "res://Assets/Textures/Sprites/Planet_B.png", 65536, new Color("#0220FF"), new Color("#3B9357"), new Color("#071EC2")));
		AddDebrisType(new DebrisData("PLANET_S", "res://Assets/Textures/Sprites/Planet_S.png", 65536, new Color("#771B00"), new Color("#FF5100"), new Color("#404040")).AddPassiveParticles(new Color("#757575"), new Color("#404040"), new Color("#585858")));

		AddDebrisType(new DebrisData("GAS_GIANT_A", "res://Assets/Textures/Sprites/Gas_Giant_A.png", 262144, new Color("#93B2FF"), new Color("#446FE5"), new Color("#3B60C6")));
		AddDebrisType(new DebrisData("GAS_GIANT_B", "res://Assets/Textures/Sprites/Gas_Giant_B.png", 262144, new Color("#02FBFF"), new Color("#28A08E"), new Color("#2E59BA")));
		AddDebrisType(new DebrisData("GAS_GIANT_C", "res://Assets/Textures/Sprites/gas_giant_c.png", 262144, new Color("#F16809"), new Color("#8B3A03"), new Color("#FFCFB0")));
		
		AddDebrisType(new DebrisData("RED_DWARF_C", "res://Assets/Textures/Sprites/Red_Dwarf_A.png", 1048576, new Color("#FF1C1C"), new Color("#FF5151"), new Color("#EF0000")).AddPassiveParticles());
		AddDebrisType(new DebrisData("RED_DWARF_A", "res://Assets/Textures/Sprites/Red_Dwarf_B.png", 1048576, new Color("#642F23"), new Color("#FFF5A8"), new Color("#FFA703")).AddPassiveParticles());
		AddDebrisType(new DebrisData("RED_DWARF_B", "res://Assets/Textures/Sprites/red_dwarf_c.png", 1048576, new Color("#FF1C1C"), new Color("#FF5151"), new Color("#EF0000")).AddPassiveParticles());
		
		AddDebrisType(new DebrisData("G_TYPE_B", "res://Assets/Textures/Sprites/G_Type_A.png", 4194304, new Color("#FDFF93"), new Color("#FFE07C"), new Color("#FFD54C")).AddPassiveParticles());
		AddDebrisType(new DebrisData("G_TYPE_A", "res://Assets/Textures/Sprites/G_Type_B.png", 4194304, new Color("#FEFED4"), new Color("#FDF282"), new Color("#FFFC96")).AddPassiveParticles());
		AddDebrisType(new DebrisData("G_TYPE_C", "res://Assets/Textures/Sprites/g_type_c.png", 4194304, new Color("#FDFF93"), new Color("#FFE07C"), new Color("#FFD54C")).AddPassiveParticles());
		
		AddDebrisType(new DebrisData("RED_GIANT_B", "res://Assets/Textures/Sprites/Red_Giant_A.png", 16777216, new Color("#FF1C1C"), new Color("#FF5151"), new Color("#EF0000")).AddPassiveParticles());
		AddDebrisType(new DebrisData("RED_GIANT_A", "res://Assets/Textures/Sprites/Red_Giant_B.png", 16777216, new Color("#D1811C"), new Color("#C90000"), new Color("#870000")).AddPassiveParticles());
		
		AddDebrisType(new DebrisData("RED_SUPERGIANT_B", "res://Assets/Textures/Sprites/Blue_Supergiant_A.png", 67108864, new Color("#4353DB"), new Color("#0012B7"), new Color("#2F3999")).AddPassiveParticles());
		AddDebrisType(new DebrisData("RED_SUPERGIANT_C", "res://Assets/Textures/Sprites/Red_Supergiant_B.png", 67108864, new Color("#F7D195"), new Color("#F7B011"), new Color("#E0B437")).AddPassiveParticles());
		AddDebrisType(new DebrisData("RED_SUPERGIANT_A", "res://Assets/Textures/Sprites/blue_supergiant_c.png", 67108864, new Color("#8AF9F1"), new Color("#8ADCFF"), new Color("#8AFFFF")).AddPassiveParticles());
		
		AddDebrisType(new DebrisData("BLACK_HOLE_A", "res://Assets/Textures/Sprites/Black_Hole_A.png", 268435456, new Color("#EFFFFB")).AddPassiveParticles(new Color("#A8A285"), new Color("#797979"), new Color("#E63925"), new Color("#DE0000")));
		AddDebrisType(new DebrisData("BLACK_HOLE_B", "res://Assets/Textures/Sprites/Black_Hole_B.png", 268435456, new Color("#EFFFFB")).AddPassiveParticles(new Color("#A8A285"), new Color("#797979"), new Color("#E63925"), new Color("#DE0000")));
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

	public bool DoesDebrisTypeExist(string debrisName) {
		return debrisTypes.ContainsKey(debrisName);
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
			{
				if (debris.Value.Data.Mass <= GameManager.CurrentMass) {
					if (debris.Value.GlobalPosition.DistanceTo(consumer.GlobalPosition) < maxDistance * consumer.TargetScale) {
						if (GD.Randf() > 1 - chance) CustomParticles.Instance.SpawnParticles(debris.Value.GlobalPosition, 1, 10, 1, debris.Value.Data.ParticleColours, consumer);
						debris.Value.Shake(5f, 100f);
					}
				} else {
					if (debris.Value.GlobalPosition.DistanceTo(consumer.GlobalPosition) < Mathf.Min(maxDistance * debris.Value.TargetScale, 500)) {
						if (GD.Randf() > 1 - chance) {
							CustomParticles.Instance.SpawnParticles(consumer.GlobalPosition, 1, 10, 1, consumer.CurrentForm.ParticleColours, debris.Value);
							consumer.ShakeCamera(5f, 100f);
							consumer.PlayGravitySFX();

							DebrisData currentFormData = consumer.CurrentForm;
							if (GameManager.CurrentMass > currentFormData.Mass) {
								GameManager.CurrentMass = Mathf.Max(GameManager.CurrentMass * (1 - GameManager.MassLossModifier), currentFormData.Mass);
							}
						}
					}
				}
			}
		}
	}

	public void ClearActiveDebris() {
		activeDebris.Clear();
	}

	public DebrisNode GetNearestConsumable(Vector2 position) {

		DebrisNode nearestDebris = null;
		float nearestDist = float.MaxValue;

		foreach (KeyValuePair<uint, DebrisNode> debris in activeDebris) {
			bool isConsumable = debris.Value.Data.Mass <= GameManager.CurrentMass;
			if (isConsumable) {
				float distance = position.DistanceSquaredTo(debris.Value.GlobalPosition);
				if (distance < nearestDist) {
					nearestDist = distance;
					nearestDebris = debris.Value;
				}
			}
		}

		return nearestDebris;
	}

	public List<DebrisNode> GetNearbyDebris(Vector2 position, float maxDistance) {

		List<DebrisNode> nearbyNodes = new List<DebrisNode>();

		foreach (KeyValuePair<uint, DebrisNode> debris in activeDebris) {
			float distance = position.DistanceSquaredTo(debris.Value.GlobalPosition);
			if (distance < maxDistance * maxDistance) {
				nearbyNodes.Add(debris.Value);
			}
		}

		return nearbyNodes;
	}

}
