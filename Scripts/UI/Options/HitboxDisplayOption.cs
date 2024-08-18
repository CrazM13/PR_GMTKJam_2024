using Godot;
using System;

public partial class HitboxDisplayOption : OptionButton {

	public override void _Ready() {
		base._Ready();

		this.Select(GameManager.displayHitboxSetting);
		this.ItemSelected += this.OnItemSelected;
	}

	private void OnItemSelected(long index) {
		GameManager.displayHitboxSetting = (int)index;
	}
}