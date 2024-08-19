using Godot;
using System;

public class ShakeConfig {

	public float Decay { get; set; }
	public float Intensity { get; set; }


	public float ShakeTime { get; set; }
	public float IntensityMod { get; set; }

	private float shakeStrength;

	public Action<float> OnShakeTick { get; set; } = delegate { };
	public Action OnReset { get; set; } = delegate { };

	public ShakeConfig() {
		Decay = 0.9f;
		Intensity = 0.9f;

		ShakeTime = 0;
		IntensityMod = 1;
	}

	public Vector2 UpdateShake(float delta) {
		if (shakeStrength > 0.25f) {

			float settingModifier = 1f - (GameManager.ShakeReduction / 100f);

			ShakeTime += delta * Intensity * IntensityMod;

			float x = Mathf.Sin(ShakeTime * 0.7f) * shakeStrength * settingModifier;
			float y = Mathf.Cos(ShakeTime * 0.3f) * shakeStrength * settingModifier;

			shakeStrength *= Decay;

			OnShakeTick?.Invoke(ShakeTime);

			if (shakeStrength <= 0.25f) {
				ShakeTime = 0;

				OnReset?.Invoke();

				return Vector2.Zero;
			}

			return new Vector2(x, y);
		}

		return Vector2.Zero;
	}

	public void Shake(float strength, float intensity = 1) {
		shakeStrength = Mathf.Max(shakeStrength, strength);
		IntensityMod = intensity;
	}

}
