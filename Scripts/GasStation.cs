using Godot;
using System;

public partial class GasStation : Node3D
{	
	[Export] public Health health;
	[Export] public Player player;
	[Export] public AudioStreamPlayer3D hitSound;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		health.healthBar = player.storeHealthBar;
		player.storeHealthBar.Value = 1;
		health.HealthDepleted += OnHealthDepleted;
		health.HealthChanged += OnHealthChanged;
	}

	private void OnHealthDepleted()
	{
		Engine.TimeScale = 0;
		player.gameOverMenu.Visible = true;
		Input.MouseMode = Input.MouseModeEnum.Visible;
	}

	private void OnHealthChanged(int newHealth)
	{
		hitSound.Play();
		if (newHealth > 0)
			player.camera.CameraShakeAsync();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
