using Godot;
using System;

public partial class ScaleCamera : Camera2D {

	[Export, ExportCategory("Screen Shake")] private float decay = 0.9f;
	[Export] private float intensity = 0.9f;

	private float shakeStrength;
	private float shakeTime = 0;
	private float intensityMod = 1;

	#region Signals
	[Signal] public delegate void OnEndScreenShakeEventHandler();
	#endregion

	public void Shake(float strength, float intensity = 1) {
		shakeStrength = Mathf.Max(shakeStrength, strength);
		intensityMod = intensity;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		UpdateScreenShake((float) delta);
	}

	private void UpdateScreenShake(float delta) {
		if (shakeStrength > 0.25f) {
			shakeTime += delta * intensity * intensityMod;

			float x = Mathf.Sin(shakeTime * 0.7f) * shakeStrength;
			float y = Mathf.Cos(shakeTime * 0.3f) * shakeStrength;

			this.Offset = new Vector2(x, y);

			shakeStrength *= decay;

			if (shakeStrength <= 0.25f) {
				shakeTime = 0;
				EmitSignal(SignalName.OnEndScreenShake);
			}
		}
	}

}
