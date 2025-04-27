using Godot;
using System;

public partial class Gun : MeshInstance3D
{
	[Export] private Node3D bulletSpawn;
	[Export] private string bulletScenePath;
	[Export] private int bulletPierce = 1,
		bulletDamage = 1;

	[Export] private float bulletSpeed = 10,
		bulletAccuracy = .9f;
	[Export] private Timer attackCooldown;
	
	private bool onAttackCooldown = false;

	private PackedScene bulletScene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bulletScene = GD.Load<PackedScene>(bulletScenePath);
		LookToCenter(GetViewport().GetCamera3D());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		if (!Visible)
			return;
			
		if (Input.IsActionPressed("fire") && !onAttackCooldown)
		{
			onAttackCooldown = true;
			attackCooldown.Start();

			Bullet bullet = (Bullet)bulletScene.Instantiate();
			bullet.pierce = bulletPierce;
			bullet.damage = bulletDamage;
			bullet.speed = bulletSpeed;
			bullet.accuracy = bulletAccuracy;

			bullet.Position = bulletSpawn.GlobalPosition;
			bullet.Basis = bulletSpawn.GlobalTransform.Basis;
			bullet.Scale = new Vector3(1,1,1);
			GetParent().GetParent().GetParent().GetParent().AddChild(bullet);
		}
	}

	private void OnAttackCooldownTimeout()
	{
		onAttackCooldown = false;
	}

	public void LookToCenter(Node3D camera)
	{
		LookAt(camera.GlobalPosition + (camera.GlobalTransform.Basis.Z * 10000.0f));
	}
}
