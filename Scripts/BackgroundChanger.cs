using Godot;
using System;

public partial class BackgroundChanger : Node {

	[Export] private int defaultBG;
	[Export] private ParallaxLayer[] backgrounds;

	[Export] private int[] backgroundIndex;

	int currentIndex = -1;
	int selectedBG = 2;

	private static readonly Color TRANSPARENT = new Color(1, 1, 1, 0);

	public override void _Ready() {
		base._Ready();

		for (int i = 0; i < backgrounds.Length; i++) {
			if (i == defaultBG) backgrounds[i].Modulate = Colors.White;
			else backgrounds[i].Modulate = TRANSPARENT;
		}
	}

	public override void _Process(double delta) {
		base._Process(delta);

		for (int i = 0; i < backgrounds.Length; i++) {
			if (i == selectedBG) backgrounds[i].Modulate = backgrounds[i].Modulate.Lerp(Colors.White, (float) delta);
			else backgrounds[i].Modulate = backgrounds[i].Modulate.Lerp(TRANSPARENT, (float) delta);
		}
	}

	private void OnLevelUp() {
		currentIndex++;

		selectedBG = backgroundIndex[currentIndex];
	}

}
