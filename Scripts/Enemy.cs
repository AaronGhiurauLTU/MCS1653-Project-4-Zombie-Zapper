using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	[Export] private float speed = 2, accel = 18;
	[Export] private Node3D target;
	[Export] private NavigationAgent3D agent;
	[Export] public Health health;
	private bool setup = false;

	public override void _Ready()
	{
		health.HealthDepleted += OnHealthDepleted;
		health.HealthChanged += OnHealthChanged;
	}

	private void OnHealthDepleted()
	{
		QueueFree();
	}

	private void OnHealthChanged(int newHealth)
	{

	}

	public override void _PhysicsProcess(double delta)
	{
		// skip first frame to allow nav mesh to syncXhronize
		if (!setup)
		{
			setup = true;
			return;
		}
		
		Vector3 direction = Vector3.Zero;

		agent.TargetPosition = target.Position;
		direction = agent.GetNextPathPosition() - GlobalPosition;
		direction = direction.Normalized();

		Velocity = Velocity.Lerp(direction * speed, accel * (float)delta);

		MoveAndSlide();
	}
}
