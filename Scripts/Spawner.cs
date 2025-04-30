using Godot;
using System;
using System.Collections;
using System.Drawing;
using System.Collections.Generic;

public partial class Spawner : Node3D
{
	[Export] private string enemyScenePath;
	[Export] private Timer spawnCooldown;

	public class EnemyInformation
	{
		public int type;
		public float delay;

		public EnemyInformation(int type, float delay)
		{
			this.type = type;
			this.delay = delay;
		}
	}
	private Queue<EnemyInformation> enemyWaves = new Queue<EnemyInformation>(new EnemyInformation[] {
		new(1, 5),
		new (1, 5),
		new (1, 5),
		new (1, 5),
		new (1, 2),
		new (1, 2),
		new (1, 2),
		new (1, 2),
		new (1, 1),
		new (1, 1),
		new (1, 1),
		new (1, 1),
		new (1, .5f),
		new (1, .5f),
		new (1, .5f),
		new (1, .5f),
	});
	private PackedScene enemyScene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		enemyScene = GD.Load<PackedScene>(enemyScenePath);
		spawnCooldown.Start();
	}

	private void OnSpawnCooldownTimeout()
	{
		if (enemyWaves.Count <= 0)
			return;

		EnemyInformation enemyInformation = enemyWaves.Dequeue();
		Enemy enemy = (Enemy)enemyScene.Instantiate();

		float x = (float)GD.RandRange(-1 * Scale.X / 2, Scale.X / 2);
		enemy.Scale = new Vector3(1 / Scale.X,1 / Scale.Y,1/ Scale.Z);
		AddChild(enemy);
		enemy.Position = new Vector3(x / Scale.X, 0, 0);

		spawnCooldown.Start(enemyInformation.delay);
	}
}
