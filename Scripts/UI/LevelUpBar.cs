using Godot;
using System;

public partial class LevelUpBar : ProgressBar {

	[Export] private PlayerMovement player;
	[Export] private TextureRect beforeImage;
	[Export] private TextureRect afterImage;
	[Export] private float fillSpeed;

	private float targetValue;

	private DebrisData currentFormData;
	private DebrisData nextFormData;

	public override void _Ready() {
		base._Ready();

		this.MinValue = 0;
		this.MaxValue = 1;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		this.Value = Mathf.MoveToward(this.Value, targetValue, delta * fillSpeed);
	}

	private void OnLevelUp() {
		currentFormData = player.CurrentForm;
		nextFormData = player.NextForm;

		if (currentFormData != null) {
			beforeImage.Texture = ResourceLoader.Load<Texture2D>(currentFormData.TexturePath);
			beforeImage.Visible = true;
		} else {
			beforeImage.Visible = false;
		}

		if (nextFormData != null) {
			afterImage.Texture = ResourceLoader.Load<Texture2D>(nextFormData.TexturePath);
			afterImage.Visible = true;
		} else {
			afterImage.Visible = false;
		}

		GetTree().CreateTimer(0.25f).Timeout += () => OnMassChange();
	}

	private void OnMassChange() {
		if (currentFormData != null && nextFormData != null) {
			targetValue = Mathf.InverseLerp(currentFormData.Mass, nextFormData.Mass, GameManager.CurrentMass);
		} else {
			targetValue = 0;
		}
	}

}
