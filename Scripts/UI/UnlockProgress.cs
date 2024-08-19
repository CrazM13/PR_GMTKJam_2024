using Godot;
using System;

public partial class UnlockProgress : Control {

	[Export] private PlayerMovement player;
	[Export] private TextureRect[] unlocks;

	private void OnLevelUp() {

		int currentForm = player.GetFormIndex;

		for (int i = 0; i < unlocks.Length; i++) {
			(unlocks[i].Material as ShaderMaterial).SetShaderParameter("IsUnlocked", i == currentForm);
		}

	}

}
