using Godot;
using System;

public partial class LevelName : Label {

	[Export] private bool allowCheats = false;

	private string[] greekLetters = {
		"alpha",
		"beta",
		"gamma",
		"delta",
		"epsilon",
		"zeta",
		"eta",
		"theta",
		"iota",
		"kappa",
		"lambda",
		"mu",
		"nu",
		"xi",
		"omicron",
		"pi",
		"rho",
		"sigma",
		"tau",
		"upsilon",
		"phi",
		"chi",
		"psi",
		"omega"
	};

	int greekAlpha = 0x03B1;

	private string[] pieFlavours = {
		"Apple", "Pumpkin", "Pecan"
	};

	private string[] universePrefix = {
		"The Universe ", "The World ", "The Dimention "
	};

	private string[] universeSuffix = {
		"Where You Got The Girl", "Where Time Travel Exists ", "Behind Mirrors", "Lost To Time", "You Made", "Where god Is A Toaster", "Where Hot Air Balloons Use Cold Air", "Where Up is Left", "You Asked For", "Where Pigs Fly"
	};

	public override void _Ready() {
		base._Ready();

		this.Text = GetLevelName(GameManager.Level);
	}

	public override void _Process(double delta) {
		base._Process(delta);

		if (allowCheats && GameManager.AllowCheats) {
			bool isCheatKeyNowPressed = Input.IsKeyPressed(Key.Backslash);
			if (isCheatKeyNowPressed && !isCheatkeyPressed) {
				GameManager.Level++;
				this.Text = GetLevelName(GameManager.Level);
			}
			isCheatkeyPressed = isCheatKeyNowPressed;
		}
	}
	private bool isCheatkeyPressed = false;

	private string GetLevelName(int level) {

		if (level <= greekLetters.Length * 2) {
			string greek = $"{greekLetters[(level - 1) % greekLetters.Length]}";
			bool isPrime = level > greekLetters.Length;

			if (GameManager.Seed <= 0.1f && greek == "pi") {
				greek = pieFlavours[Mathf.RoundToInt(GameManager.Seed * 1000) % pieFlavours.Length];
				greek += " Pie";
			}

			return $"Universe {greek}{(isPrime ? " PRIME" : "")}";
		}

		return universePrefix[level % universePrefix.Length] + universeSuffix[level % universeSuffix.Length];
	}

}
