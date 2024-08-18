using Godot;
using System;

public partial class ParticleCountOption : OptionButton {

	public override void _Ready() {
		base._Ready();

		this.Select(OptionToIndex());
		this.ItemSelected += this.OnItemSelected;
	}

	private void OnItemSelected(long index) {
		
		switch (index) {
			case 0:
				GameManager.ParticlesAmount = 1;
				break;
			case 1:
				GameManager.ParticlesAmount = 5;
				break;
			case 2:
				GameManager.ParticlesAmount = 0;
				break;
		}

	}

	private int OptionToIndex() {
		int option = GameManager.ParticlesAmount;

		switch (option) {
			case 0:
				return 2;
			case 1:
				return 0;
			default:
				return 1;
		}
	}
}
