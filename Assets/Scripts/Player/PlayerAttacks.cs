using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{
    [Header("Weapon Settings")]
    // The attack speed of the weapon in fires per minute
    // Is also used to limit the semi auto fire rate when not isAutomatic
    public float weaponAttackRate;
    public float weaponDamage;
    public float weaponRange;

    public bool isAutomatic;
    public KeyCode attackKey = KeyCode.Mouse0;
    public LayerMask toAttack;

    private float fireTimer;

    private void Start()
    {
        fireTimer = 60 / weaponAttackRate;
    }

    // Update is called once per frame
    void Update()
    {
        if (fireTimer > 0)
            fireTimer -= Time.deltaTime;

        // Script for when the weapon should fire automatically
        if (isAutomatic && Input.GetKey(attackKey))
        {
            if (fireTimer <= 0)
            {
                fireTimer = 60 / weaponAttackRate;
                FireWeapon();
                
            }       
        }
        // Script for when the weapon only should fire when the user clicks
        else if (Input.GetKeyDown(attackKey))
        {
            if (fireTimer <= 0)
            {
                fireTimer = 60 / weaponAttackRate;
                FireWeapon();
            }
        }
    }

    // Code that actually fires the weapon
    private void FireWeapon()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, transform.forward, out hit, weaponRange, toAttack)) 
        {
            
        }
    }
}
