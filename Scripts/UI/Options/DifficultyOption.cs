using Godot;
using System;

public partial class DifficultyOption : OptionButton {

	public override void _Ready() {
		base._Ready();

		this.Select(OptionToIndex());
		this.ItemSelected += this.OnItemSelected;
	}

	private void OnItemSelected(long index) {

		switch (index) {
			case 0:
				GameManager.ConsumptionEfficiency = 0.5f;
				GameManager.MassLossModifier = 0.001f;
				GameManager.DifficultySetting = 0;
				break;
			case 1:
				GameManager.ConsumptionEfficiency = 0.25f;
				GameManager.MassLossModifier = 0.01f;
				GameManager.DifficultySetting = 1;
				break;
			case 2:
				GameManager.ConsumptionEfficiency = 0.125f;
				GameManager.MassLossModifier = 0.02f;
				GameManager.DifficultySetting = 2;
				break;
		}

	}

	private static int OptionToIndex() {
		return GameManager.DifficultySetting;
	}



}
