using Godot;
using System;

public partial class LevelUpBar : ProgressBar {

	[Export] private PlayerMovement player;
	[Export] private TextureRect beforeImage;
	[Export] private TextureRect afterImage;
	[Export] private Label title;
	[Export] private float fillSpeed;

	private float targetValue;

	private DebrisData currentFormData;
	private DebrisData nextFormData;

	private string[] formNames = {
		"ALPHA PARTICLE",
		"GRAIN OF SAND",
		"STONE",
		"BOULDER",
		"SPACE DEBRIS",
		"METEOROID",
		"PLANETOID",
		"MOON",
		"PLANET",
		"GAS GIANT",
		"RED DWARF",
		"G TYPE STAR",
		"RED GIANT",
		"SUPERGIANT",
		"BLACK HOLE"
	};

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
		}

		if (nextFormData != null) {
			afterImage.Texture = ResourceLoader.Load<Texture2D>(nextFormData.TexturePath);
		}

		title.Text = formNames[player.GetFormIndex];

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
