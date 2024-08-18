using Godot;
using System;

public partial class VolumeSlider : HSlider {

	[Export] private string busName;

	public override void _Ready() {
		base._Ready();

		DisplayVolume();
		ValueChanged += (double _) => SetVolume();
	}

	private void SetVolume() {
		AudioServer.SetBusVolumeDb(AudioServer.GetBusIndex(busName), Mathf.LinearToDb((float) this.Value));
	}

	private void DisplayVolume() {
		float db = AudioServer.GetBusVolumeDb(AudioServer.GetBusIndex(busName));
		this.Value = Mathf.DbToLinear(db);
	}

}
