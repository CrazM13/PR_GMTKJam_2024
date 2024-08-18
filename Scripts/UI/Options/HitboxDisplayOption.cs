using Godot;
using System;

public partial class HitboxDisplayOption : OptionButton {

	public override void _Ready() {
		base._Ready();

		this.Select(GameManager.DisplayHitboxSetting);
		this.ItemSelected += this.OnItemSelected;
	}

	private void OnItemSelected(long index) {
		GameManager.DisplayHitboxSetting = (int)index;
	}
}