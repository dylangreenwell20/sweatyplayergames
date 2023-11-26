using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    // Stats to do with this objects health
    public float maxHealth;
    private float health;

    private void Start()
    {
        health = maxHealth;
    }

    // Deal damage to this objects health
    public void DealDamage(float damage)
    {
        Debug.Log(damage);
        health -= damage;
    }

    private void Update()
    {
        // Check when this object is dead
        if (health <= 0) 
        {
            Death();
        }
    }
       
    // Object death logic goes here
    private void Death()
    {

    }
}
