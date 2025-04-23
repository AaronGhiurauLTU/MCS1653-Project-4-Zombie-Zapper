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
	private Vector2 mouseLook = Vector2.Zero;
	public override void _Ready()
	{
		Input.MouseMode = Input.MouseModeEnum.Captured;
		health.HealthDepleted += OnHealthDepleted;
		health.HealthChanged += OnHealthChanged;
	}

	private void OnHealthDepleted()
	{
		GetTree().ChangeSceneToFile("res://Scenes/level.tscn");
	}

	private void OnHealthChanged(int newHealth)
	{

	}
	public override void _PhysicsProcess(double delta)
	{
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
