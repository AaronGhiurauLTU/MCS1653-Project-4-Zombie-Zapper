using Godot;
using System;
using System.Collections.Generic;

public partial class Bullet : Node3D
{
	public int pierce = 1, damage = 1;
	public float speed = 10, accuracy = .1f;
	private Vector3 offset;
	private HashSet<Enemy> enemiesHit = new();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		offset = new Vector3((float)GD.RandRange(-1.0,1.0), (float)GD.RandRange(-1.0,1.0), (float)GD.RandRange(-1.0,1.0));
		offset *= 1 - accuracy;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		Vector3 velocity = speed * -1 * (GlobalTransform.Basis.Y + (0.25f * offset));

		// to avoid bullets tunneling through objects, create a raycast to where the bullet will move to and check if it hits something	
		Vector3 from = GlobalTransform.Origin;
		Vector3 to = from + velocity * (float)delta;
		var spaceState = GetWorld3D().DirectSpaceState;
		var space_state = GetWorld3D().DirectSpaceState;
		var query = PhysicsRayQueryParameters3D.Create(from, to,
            1);
        var result = spaceState.IntersectRay(query);

		GlobalPosition = to;

		if (result.Count > 0)
		{   
			var collider = result["collider"];
			var colliderObject = (Node3D)collider; // or whatever class you're expecting

			OnBodyEntered(colliderObject);
		}
	}

	private void OnTimeout()
	{
		QueueFree();
	}

	private void OnBodyEntered(Node3D body)
	{
		if (body is Enemy enemy)
		{
			if (enemiesHit.Contains(enemy))
				return;
				
			enemiesHit.Add(enemy);
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
