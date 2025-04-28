using Godot;
using System;

public partial class AmmoInteract : Node3D
{
	[Export] public int cost = -5, ammoAmount = 50;
	[Export] public int gunType = 1;
	[Export] private Label information;
	public override void _Ready()
	{
		information.Text = $"Gun Type: Gun {gunType}\nCost: ${-1 * cost}\nAmmo: {ammoAmount}";
	}
}
