using Godot;
using System;

public partial class UIZoomLevel : Label {

	[Export] private PlayerMovement player;

	private string[] zoomScales = {
		"TINY",
		"LESS TINY",
		"SMALL",
		"MEH",
		"LARGE",
		"VERY LARGE",
		"OKAY",
		"BIG ENOUGH",
		"STOP",
		"OK REALLY",
		"STOOOP",
		"TOO BIG",
		"U FAT",
		"U REALLY FAT",
		"YO MAMA"
	};

	private void OnLevelUp() {
		this.Text = $"Zoom: {zoomScales[player.GetFormIndex]}";
	}

}
