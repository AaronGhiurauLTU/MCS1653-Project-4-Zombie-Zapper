using Godot;
using System;

public partial class Player : CharacterBody3D
{
	[Export] private float Speed = 5.0f;
	[Export] private Camera3D camera;
	[Export] private float minLookDownAngle = -45, 
		maxLookDownAngle = 45;
	[Export] private float cameraSensitivity = 0.05f;

	[Export] public Health health;
	[Export] public TextureProgressBar storeHealthBar;
	[Export] private Label doorUseHint, buyAmmoHint, ammoIndicator, moneyIndicator, buyHealthHint;
	[Export] private RayCast3D interactCast;
	[Export] private Gun gun1, gun2;
	[Export] private int startingMoney = 0;
	[Export] public Control gameOverMenu;
	[Export] private ColorRect pixelationEffect;
	private Gun currentGun;
	private int currentMoney;
	private Vector2 mouseLook = Vector2.Zero;
	private GasStation store;
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		health.HealthDepleted += OnHealthDepleted;
		health.HealthChanged += OnHealthChanged;

		gun1.UpdateAmmo += OnBulletFired;
		gun2.UpdateAmmo += OnBulletFired;

		SetGun(1);
		currentMoney = 0;
		ChangeMoney(startingMoney);
		ChangeUseDoorHint(false);
		ChangeBuyAmmoHint(false);
		ChangeBuyHealthHint(false);

		store = GetParent().GetNode<GasStation>("GasStation");
	}

	private void OnHealthDepleted()
	{
		Engine.TimeScale = 0;

		gameOverMenu.Visible = true;
		Input.MouseMode = Input.MouseModeEnum.Visible;
		//GetTree().ChangeSceneToFile("res://Scenes/level.tscn");
	}

	private void OnHealthChanged(int newHealth)
	{

	}

	private void OnBulletFired(int currentAmmo, int maxAmmo)
	{
		ammoIndicator.Text = $"Ammo: {currentAmmo}/{maxAmmo}";
	}

	public bool ChangeMoney(int change)
	{
		if (currentMoney + change < 0)
			return false;
		
		currentMoney += change;
		moneyIndicator.Text = $"Money: ${currentMoney}";
		
		return true;
	}

	public void ChangeUseDoorHint(bool show)
	{
		if (show)
		{
			doorUseHint.Visible = true;
		}
		else
		{
			doorUseHint.Visible = false;
		}
	}

	public void ChangeBuyAmmoHint(bool show)
	{
		if (show)
		{
			buyAmmoHint.Visible = true;
		}
		else
		{
			buyAmmoHint.Visible = false;
		}
	}

	public void ChangeBuyHealthHint(bool show)
	{
		if (show)
		{
			buyHealthHint.Visible = true;
		}
		else
		{
			buyHealthHint.Visible = false;
		}
	}

	private void SetGun(int number)
	{
		switch (number)
		{
			case 1:
				currentGun = gun1;
				gun2.Visible = false;
				break;
			case 2:
				currentGun = gun2;
				gun1.Visible = false;
				break;
		}

		currentGun.Visible = true;
	}
	public override void _PhysicsProcess(double delta)
	{
		if (Engine.TimeScale == 0)
			return;

		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		if (Input.IsActionJustPressed("ui_cancel"))
		{
			if (Input.MouseMode == Input.MouseModeEnum.Captured)
			{
				Input.MouseMode = Input.MouseModeEnum.Visible;
			}
			else
			{
				Input.MouseMode = Input.MouseModeEnum.Captured;
			}
		}
		else if (Input.IsActionJustPressed("gun1"))
		{
			SetGun(1);
		}
		else if (Input.IsActionJustPressed("gun2"))
		{
			SetGun(2);
		}

		if (interactCast.IsColliding())
		{
			if ((Node)interactCast.GetCollider() != null && ((Node)interactCast.GetCollider()).GetParent() is Node)
			{
				if (((Node)interactCast.GetCollider()).GetParent().GetParent() is DoorInteract doorInteract)
				{
					ChangeUseDoorHint(true);
					if (Input.IsActionJustPressed("interact") && doorUseHint.Visible)
					{
						Position = doorInteract.tpNode.GlobalPosition;
					}
				}
				else if (((Node)interactCast.GetCollider()).GetParent().GetParent() is AmmoInteract ammoInteract)
				{
					ChangeBuyAmmoHint(true);
					if (Input.IsActionJustPressed("interact") && buyAmmoHint.Visible
					 && ((ammoInteract.gunType == 1 && currentGun == gun1) || (ammoInteract.gunType == 2 && currentGun == gun2))
					 && (currentGun.CurrentAmmo < currentGun.MaxAmmo)
					 && ChangeMoney(ammoInteract.cost))
					{
						currentGun.AddAmmo(ammoInteract.ammoAmount);
					}
				}
				else if (((Node)interactCast.GetCollider()).GetParent().GetParent() is HealthInteract healthInteract)
				{
					ChangeBuyHealthHint(true);
					if (Input.IsActionJustPressed("interact") && buyHealthHint.Visible
					 && ((healthInteract.type == HealthInteract.HPType.store && store.health.CurrentHealth < store.health.MaxHealth) 
					 	|| (healthInteract.type == HealthInteract.HPType.player && health.CurrentHealth < health.MaxHealth))
					 && ChangeMoney(healthInteract.cost))
					{
						if (healthInteract.type == HealthInteract.HPType.store)
						{
							store.health.AddHealth(healthInteract.ammount);
						}
						else if (healthInteract.type == HealthInteract.HPType.player)
						{
							health.AddHealth(healthInteract.ammount);
						}
					}
				}
				else
				{
					ChangeUseDoorHint(false);
					ChangeBuyAmmoHint(false);
					ChangeBuyHealthHint(false);
				}
			}
			else
			{
				ChangeUseDoorHint(false);
				ChangeBuyAmmoHint(false);
				ChangeBuyHealthHint(false);
			}
		}
		else
		{
			ChangeUseDoorHint(false);
			ChangeBuyAmmoHint(false);
			ChangeBuyHealthHint(false);
		}
		
		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "up", "down");
		Vector3 direction = (Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		mouseLook *= (float)delta;
		camera.RotateX(mouseLook.Y);
		float x = Math.Clamp(camera.Rotation.X,
			Mathf.DegToRad(minLookDownAngle),
			Mathf.DegToRad(maxLookDownAngle)
		);
		camera.Rotation = new Vector3(x, 0, 0);
		RotateY(mouseLook.X);

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventMouseMotion inputEventMouseMotion)
		{
			if (Input.MouseMode == Input.MouseModeEnum.Captured)
			{
				mouseLook = inputEventMouseMotion.Relative * -1 * cameraSensitivity;
			}
		}
	}
}
