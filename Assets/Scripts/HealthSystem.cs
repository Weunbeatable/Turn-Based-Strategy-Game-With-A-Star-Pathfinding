using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public event EventHandler onDead;
    public event EventHandler onDamaged;

   [SerializeField] private int health = 100;
    private int healthMax;

    private void Awake()
    {
        healthMax = health;
    }

    public void Damage(int damageAmount)
    {
        health -= damageAmount;

        if(health < 0)
        {
            health = 0;
        }

        onDamaged?.Invoke(this, EventArgs.Empty);

        Debug.Log("health");
        if(health == 0)
        {
            Die();
        }
    }

    private void Die()
    {
        onDead?.Invoke(this, EventArgs.Empty);
    }

    public float GetHealthNormalized() => (float)health / healthMax; // cast to a float to have a float / int to get a float
}
