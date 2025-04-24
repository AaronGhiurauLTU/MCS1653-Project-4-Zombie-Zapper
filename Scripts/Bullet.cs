using Godot;
using System;

public partial class Bullet : Node3D
{
	public int pierce = 1, damage = 1;
	public float speed = 10, accuracy = .1f;
	private Vector3 offset;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		offset = new Vector3((float)GD.RandRange(-1.0,1.0), (float)GD.RandRange(-1.0,1.0), (float)GD.RandRange(-1.0,1.0));
		offset *= 1 - accuracy;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Position += (float)delta * speed * -1 * (GlobalTransform.Basis.Y + (0.25f * offset));
	}

	private void OnTimeout()
	{
		QueueFree();
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body is Enemy enemy)
		{
			pierce--;

			enemy.health.TakeDamage(damage);

			if (pierce <= 0)
			{
				QueueFree();
			}
		}
		else
		{
			// destroy bullet when it collides with level
			QueueFree();
		}
	}
}
