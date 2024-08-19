using Godot;
using System;

public partial class ShakeOption : OptionButton {

	public override void _Ready() {
		base._Ready();

		this.Select(OptionToIndex());
		this.ItemSelected += this.OnItemSelected;
	}

	private void OnItemSelected(long index) {
		
		switch (index) {
			case 0:
				GameManager.ShakeReduction = 0;
				break;
			case 1:
				GameManager.ShakeReduction = 50;
				break;
			case 2:
				GameManager.ShakeReduction = 100;
				break;
		}

	}

	private static int OptionToIndex() {
		int option = GameManager.ShakeReduction;

		return option switch {
			0 => 0,
			100 => 2,
			_ => 1,
		};
	}
}
