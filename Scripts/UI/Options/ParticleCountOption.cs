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
				GameManager.particlesAmount = 1;
				break;
			case 1:
				GameManager.particlesAmount = 5;
				break;
			case 2:
				GameManager.particlesAmount = 0;
				break;
		}

	}

	private int OptionToIndex() {
		int option = GameManager.particlesAmount;

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
