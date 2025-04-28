using Godot;
using System;

public partial class Gun : MeshInstance3D
{
	[Signal] public delegate void UpdateAmmoEventHandler(int currentAmmo, int maxAmmo);

	[Export] private Node3D bulletSpawn;
	[Export] private string bulletScenePath;
	[Export] private int bulletPierce = 1,
		bulletDamage = 1,
		maxAmmo = 100;

	public int MaxAmmo { get { return maxAmmo; } }
	[Export] private float bulletSpeed = 10,
		bulletAccuracy = .9f;
	[Export] private Timer attackCooldown;
	
	private bool onAttackCooldown = false,
		disabled = false;
	private int currentAmmo;
	public int CurrentAmmo { get { return currentAmmo; } }
	private PackedScene bulletScene;

	public void AddAmmo(int ammoChange)
	{
		currentAmmo += ammoChange;
		currentAmmo = Math.Min(currentAmmo, maxAmmo);

		EmitSignal(SignalName.UpdateAmmo, currentAmmo, maxAmmo);	
	}

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bulletScene = GD.Load<PackedScene>(bulletScenePath);
		LookToCenter(GetViewport().GetCamera3D());
		currentAmmo = maxAmmo;

		if (Visible)
			EmitSignal(SignalName.UpdateAmmo, currentAmmo, maxAmmo);	
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		if (!Visible)
		{
			disabled = true;
			return;
		}
		else if (disabled)
		{
			disabled = false;
			EmitSignal(SignalName.UpdateAmmo, currentAmmo, maxAmmo);
		}


		if (Input.IsActionPressed("fire") && !onAttackCooldown && currentAmmo > 0)
		{
			currentAmmo--;

			EmitSignal(SignalName.UpdateAmmo, currentAmmo, maxAmmo);

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
