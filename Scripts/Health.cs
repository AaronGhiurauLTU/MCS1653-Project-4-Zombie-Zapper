using Godot;
using System;

public partial class Health : Node
{
	[Export] private int maxHealth = 5;
	[Export] public TextureProgressBar healthBar;
	private int currentHealth;
	private AudioStreamPlayer2D hitSound;
	// get the current health without allowing the setting of it publicly
	public int CurrentHealth { get { return currentHealth; } }
	public int MaxHealth { get { return maxHealth; } }
	// custom signal to fire when health reaches 0
	[Signal] public delegate void HealthDepletedEventHandler();

	[Signal] public delegate void HealthChangedEventHandler(int newHealth);
	public void TakeDamage(int damage)
	{
		currentHealth -= damage;

		// keep health from going below 0
		currentHealth = Math.Max(0, currentHealth);

		EmitSignal(SignalName.HealthChanged, currentHealth);

		healthBar.Visible = true;
		healthBar.Value = (float)currentHealth / maxHealth;	
		//hitSound.Play();
		// detect death
		if (currentHealth == 0)
		{
			EmitSignal(SignalName.HealthDepleted);
		}
	}

	public void AddHealth(int healthChange)
	{
		currentHealth += healthChange;

		// keep health from going below 0
		currentHealth = Math.Min(maxHealth, currentHealth);

		EmitSignal(SignalName.HealthChanged, currentHealth);

		healthBar.Visible = true;
		healthBar.Value = (float)currentHealth / maxHealth;	
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		currentHealth = maxHealth;

		if (GetParent() is Enemy)
		{
			healthBar.Visible = false;
			healthBar.Size = new Vector2(100 * (maxHealth / 3), healthBar.Size.Y);
			healthBar.Position = new Vector2(150 - (healthBar.Size.X / 2), healthBar.Position.Y);
		}

		if (healthBar != null)
			healthBar.Value = 1;

		//hitSound = GetNode<AudioStreamPlayer2D>("HitSound");
	}
}
