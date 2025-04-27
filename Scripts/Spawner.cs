using Godot;
using System;
using System.Drawing;

public partial class Spawner : Node3D
{
	[Export] private string enemyScenePath;
	[Export] private Timer spawnCooldown;
	private PackedScene enemyScene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		enemyScene = GD.Load<PackedScene>(enemyScenePath);
		spawnCooldown.Start();
	}

	private void OnSpawnCooldownTimeout()
	{
		Enemy enemy = (Enemy)enemyScene.Instantiate();

		float x = (float)GD.RandRange(-1 * Scale.X / 2, Scale.X / 2);
		enemy.Scale = new Vector3(1 / Scale.X,1 / Scale.Y,1/ Scale.Z);
		AddChild(enemy);
		enemy.Position = new Vector3(x / Scale.X, 0, 0);

		spawnCooldown.Start();
	}
}
