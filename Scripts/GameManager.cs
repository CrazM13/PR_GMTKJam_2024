using Godot;
using System;

public static class GameManager {

	public static float CurrentMass = 1;

	public static float GameScale = 1;

	public static float GameTime = 0;


	public static float Seed = 0;

	public static int ParticlesAmount = 1;
	public static int DisplayHitboxSetting = 0;
	public static int ShakeReduction = 0;
	
	public static bool AllowCheats = false;
	public static bool ShowHelpArrow = true;

	public static float DefaultReachDistance = 20;

	public static bool IsGamePaused = false;

	public static int DifficultySetting = 1;
	public static float ConsumptionEfficiency = 0.25f;
	public static float MassLossModifier = 0.01f;

	public static int Level = 1;

}
