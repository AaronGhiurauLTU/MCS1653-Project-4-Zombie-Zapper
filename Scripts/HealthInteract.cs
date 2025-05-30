using Godot;
using System;

public partial class HealthInteract : Node3D
{
	public enum HPType
	{
		store,
		player
	}
	[Export] public int cost = -10, ammount = 5;
	[Export] public HPType type = HPType.player;
	[Export] public Label information;
	public override void _Ready()
	{
		information.Text = $"Cost: ${-1 * cost}\nAmount Healed: {ammount}";
	}

}
