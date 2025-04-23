using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	[Export] private float speed = 2, accel = 18;
	[Export] private Node3D target;
	[Export] private NavigationAgent3D agent;
	[Export] private int damage = 1;
	[Export] public Health health;
	private bool setup = false;
	private Player playerBeingAttacked;
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

	private void OnHurtBoxEntered(Node3D body)
	{
		if (body is Player player)
		{
			playerBeingAttacked = player;
			Attack();
		}
	}

	private void OnHurtBoxExited(Node3D body)
	{
		if (body is Player player && playerBeingAttacked == player)
		{
			playerBeingAttacked = null;
		}
	}

	private void Attack()
	{
		if (playerBeingAttacked == null)
			return;

		playerBeingAttacked.health.TakeDamage(damage);
	}
}
