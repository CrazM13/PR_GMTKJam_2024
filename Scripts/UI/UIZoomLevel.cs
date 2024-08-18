using Godot;
using System;

public partial class UIZoomLevel : Label {

	[Export] private PlayerMovement player;

	private string[] zoomScales = {
		"10^-14.5",
		"10^-3.3",
		"10^-0.7",
		"10^0.7",
		"10^2.7",
		"10^3.8",
		"10^4.6",
		"10^6.5",
		"10^7.1",
		"10^8.3",
		"10^9.1",
		"10^9.7",
		"10^11.0",
		"10^12.5",
		"10^13.3"
	};

	private void OnLevelUp() {
		this.Text = $"Zoom: {zoomScales[player.GetFormIndex]}";
	}

}
