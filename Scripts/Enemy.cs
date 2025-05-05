using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	[Export] private float speed = 2, accel = 18;
	private Node3D target;
	[Export] private NavigationAgent3D agent;
	[Export] private int damage = 1,
		moneyDropped = 1;
	[Export] public Health health;
	[Export] private Timer attackCooldown;
	[Export] private AnimationPlayer animationPlayer;
	[Export] private MeshInstance3D zombieMesh;
	private bool setup = false;
	private Player player;
	private Player playerBeingAttacked;
	private GasStation gasStation;
	private Vector3 originalPosition;

	private bool onAttackCooldown = false,
		attackingShop = false;

	public override void _Ready()
	{
		health.HealthDepleted += OnHealthDepleted;
		health.HealthChanged += OnHealthChanged;
		gasStation = GetParent().GetParent().GetNode<GasStation>("GasStation");
		player = GetParent().GetParent().GetNode<Player>("Player");
		originalPosition = GlobalPosition;
	}

	private void OnHealthDepleted()
	{
		player.ChangeMoney(moneyDropped);
		Spawner.enemiesAlive--;
		animationPlayer.Stop();
		animationPlayer.Play("death");
	}

	private void OnHealthChanged(int newHealth)
	{
		animationPlayer.Stop();
		animationPlayer.Play("hit");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Engine.TimeScale == 0 || health.CurrentHealth <= 0)
			return;

		if (Math.Abs(originalPosition.Z - player.GlobalPosition.Z) < Math.Abs(originalPosition.Z - gasStation.GlobalPosition.Z))
		{
			target = player;
		}
		else
		{
			target = gasStation;
		}

		// skip first frame to allow nav mesh to syncXhronize
		if (!setup)
		{
			setup = true;
			return;
		}

		if (Engine.TimeScale == 0)
			return;
		
		Vector3 direction = Vector3.Zero;

		agent.TargetPosition = target.Position;
		direction = agent.GetNextPathPosition() - GlobalPosition;
		direction = direction.Normalized();

		Vector3 lookPosition = GlobalPosition + direction;
		lookPosition.Y = GlobalPosition.Y;

		LookAt(lookPosition);
		Velocity = Velocity.Lerp(direction * speed, accel * (float)delta);

		MoveAndSlide();

		if (playerBeingAttacked != null || attackingShop)
			Attack();
	}

	private void OnHurtBoxEntered(Node3D body)
	{
		if (body is Player player)
		{
			playerBeingAttacked = player;
			Attack();
		}
		else if (body.GetParent().GetParent() is GasStation gasStation)
		{
			attackingShop = true;
			Attack();
		}
	}

	private void OnHurtBoxExited(Node3D body)
	{
		if (body is Player player && playerBeingAttacked == player)
		{
			playerBeingAttacked = null;
		}
		else if (body.GetParent().GetParent() is GasStation gasStation)
		{
			attackingShop = false;
		}
	}

	private void Attack()
	{
		if ((playerBeingAttacked == null && attackingShop == false) || onAttackCooldown)
			return;
		
		onAttackCooldown = true;

		attackCooldown.Start();
		
		if (playerBeingAttacked != null)
		{
			playerBeingAttacked.health.TakeDamage(damage);
		}
		else if (attackingShop)
		{
			gasStation.health.TakeDamage(damage);
		}
	}

	private void OnAttackCooldownTimeout()
	{
		onAttackCooldown = false;
	}
}
