using Godot;
using System;

public partial class LightFlicker : OmniLight3D
{
	[Export] Timer flickerTimer;
	[Export] AnimationPlayer animationPlayer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		float time = GD.RandRange(4, 20);
		flickerTimer.Start(time);
	}

	public void OnFlickerTimer()
	{
		animationPlayer.Play("flicker");
		float time = GD.RandRange(4, 20);
		flickerTimer.Start(time);
	}
}
