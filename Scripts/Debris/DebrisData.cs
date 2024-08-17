using Godot;
using System;
using System.Collections.Generic;

public class DebrisData {

	public string Name { get; set; }
	public string TexturePath { get; set; }
	public Color[] ParticleColours { get; set; }
	public float Mass { get; set; }

	public DebrisData(string name, string texturePath, float mass, params Color[] colours) {
		
		Name = name;
		TexturePath = texturePath;
		Mass = mass;
		ParticleColours = colours;

	}

}
