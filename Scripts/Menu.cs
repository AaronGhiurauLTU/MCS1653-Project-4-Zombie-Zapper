using Godot;
using System;

public partial class Menu : Control
{
	[Export] private VBoxContainer instructionsContainer, mainMenuContainer, creditsContainer;
	[Export] Label label;
	[Export] private ColorRect pixelationEffect;

	public override void _Ready()
	{
		if (pixelationEffect != null)
			pixelationEffect.Visible = true;
	}
	public void ChangeLabelText(string newText)
	{
		label.Text = newText;
	}
	private void OnContinuePressed()
	{
		Engine.TimeScale = 1;
		Visible = false;
		Input.MouseMode = Input.MouseModeEnum.Captured;
	}
	private void OnResetPressed()
	{
		Engine.TimeScale = 1;
		//GameManager.ReloadLevel();
	}
	private void OnPlayPressed()
	{
		Engine.TimeScale = 1;
		GetTree().ChangeSceneToFile("res://Scenes/level.tscn");
		GD.Print("Play pressed");
		//GameManager.LoadLevel(1);
	}

	private void OnNextLevelPressed()
	{
		//GameManager.LoadLevel(++GameManager.currentLevel);
	}
	private void OnMainMenuPressed()
	{
		Engine.TimeScale = 1;
		GetTree().ChangeSceneToFile("res://Scenes/MainMenu.tscn");
	}

	private void OnQuitPressed()
	{
		GetTree().Quit();
	}

	private void OnCreditsPressed()
	{
		creditsContainer.Visible = true;
		mainMenuContainer.Visible = false;
	}

	private void OnCreditsBackPressed()
	{
		creditsContainer.Visible = false;
		mainMenuContainer.Visible = true;
	}

	private void OnInstructionsPressed()
	{
		instructionsContainer.Visible = true;
		mainMenuContainer.Visible = false;	
	}
	private void OnInstructionsBackPressed()
	{
		instructionsContainer.Visible = false;
		mainMenuContainer.Visible = true;
	}
}
