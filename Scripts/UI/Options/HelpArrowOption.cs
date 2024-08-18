using Godot;
using System;

public partial class HelpArrowOption : OptionButton {

	public override void _Ready() {
		base._Ready();

		this.Select(GameManager.showHelpArrow ? 0 : 1);
		this.ItemSelected += this.OnItemSelected;
	}

	private void OnItemSelected(long index) {
		GameManager.showHelpArrow = index == 0;
	}
}