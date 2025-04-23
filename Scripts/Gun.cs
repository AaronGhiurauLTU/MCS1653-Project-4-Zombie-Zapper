using Godot;
using System;

public partial class Gun : MeshInstance3D
{
	[Export] private Node3D bulletSpawn;
	[Export] private string bulletScenePath;
	private PackedScene bulletScene;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		bulletScene = GD.Load<PackedScene>(bulletScenePath);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{	
		//GD.Print(bulletSpawn.GlobalRotationDegrees + " " + bulletSpawn.GlobalTransform.Basis.Z);
		if (Input.IsActionJustPressed("fire"))
		{
			Node3D bullet = (Node3D)bulletScene.Instantiate();

			bullet.Position = bulletSpawn.GlobalPosition;
			bullet.Basis = bulletSpawn.GlobalTransform.Basis;
			bullet.Scale = new Vector3(1,1,1);

			//bullet.RotateZ(Mathf.DegToRad(90));
			//GD.Print(GetParent().GetParent().GetParent().Name);
			//bulletSpawn.RemoveChild(bullet);
			this.GetParent().GetParent().GetParent().AddChild(bullet);
		
			GD.Print(bullet.GetParent().Name);
		}
	}
}
