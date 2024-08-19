using Godot;
using System;

public partial class CheatOption : OptionButton {

	public override void _Ready() {
		base._Ready();

		this.Select(OptionToIndex());
		this.ItemSelected += this.OnItemSelected;
	}

	private void OnItemSelected(long index) {

		GameManager.AllowCheats = (int) index == 1;

	}

	private static int OptionToIndex() {
		return GameManager.AllowCheats ? 1 : 0;
	}

}
