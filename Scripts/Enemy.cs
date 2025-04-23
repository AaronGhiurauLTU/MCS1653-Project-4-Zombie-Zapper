using Godot;
using System;

public partial class Enemy : CharacterBody3D
{
	[Export] private float speed = 2, accel = 18;
	[Export] private Node3D target;
	[Export] private NavigationAgent3D agent;
	public override void _PhysicsProcess(double delta)
	{
		Vector3 direction = Vector3.Zero;

		agent.TargetPosition = target.Position;
		direction = agent.GetNextPathPosition() - GlobalPosition;
		direction = direction.Normalized();

		Velocity = Velocity.Lerp(direction * speed, accel * (float)delta);

		MoveAndSlide();
	}
}
