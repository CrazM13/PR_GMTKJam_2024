using Godot;
using System;

public partial class PauseMenu : Control {

	[Signal] public delegate void OnMenuStateChangedEventHandler(bool isOpen);

	private bool isMenuOpen = false;
	public bool IsMenuOpen {
		get => isMenuOpen;
		set {
			if (isMenuOpen != value) {

				Visible = value;
				GameManager.IsGamePaused = value;
				Engine.TimeScale = value ? 0 : 1;

				isMenuOpen = value;
			}
		}
	}

	public void SetMenuState(bool open) {
		IsMenuOpen = open;
	}

	public void HideMenu() {
		Visible = false;
		Engine.TimeScale = 1;
	}

	public override void _Ready() {
		base._Ready();

		IsMenuOpen = false;
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (Input.IsActionJustPressed("menu_pause")) {
			IsMenuOpen = !IsMenuOpen;
			EmitSignal(SignalName.OnMenuStateChanged, IsMenuOpen);
		}
	}

}
