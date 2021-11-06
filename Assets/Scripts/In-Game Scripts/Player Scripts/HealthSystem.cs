using System;
using UnityEngine;

public class HealthSystem
{
    public event EventHandler OnHealthChanged;
    public event EventHandler OnDead;

    int health;
    int healthMax;

    public HealthSystem(int healthMax)
    {
        this.healthMax = healthMax;
        health = healthMax;
    }

    public int GetHealth()
    {
        return health;
    }

    public float GetHealthPercent()
    {
        return (float)health / healthMax;
    }

    public void SetHealthAmount(int health)
    {
        this.health = health;
        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
    }

    public void Damage(int amount)
    {
        health -= amount;
        if (health < 0)
            health = 0;

        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);

        if (health <= 0)
            Die();
    }

    public void Die()
    {
        if (OnDead != null) OnDead(this, EventArgs.Empty);
    }

    public bool IsDead()
    {
        return health <= 0;
    }

    public void Heal(int amount)
    {
        health += amount;
        if (health > healthMax)
            health = healthMax;

        if (OnHealthChanged != null) OnHealthChanged(this, EventArgs.Empty);
    }
}
