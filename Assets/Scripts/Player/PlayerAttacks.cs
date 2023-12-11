using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerAttacks : MonoBehaviour
{
    [Header("Weapon Settings")]
    // The attack speed of the weapon in fires per minute
    // Is also used to limit the semi auto fire rate when not isAutomatic
    public float weaponAttackRate;
    public float weaponDamage;
    public float weaponRange;
    public int magSize;

    public bool isAutomatic;
    public KeyCode attackKey = KeyCode.Mouse0;
    public KeyCode reloadKey = KeyCode.R;
    public LayerMask toAttack;

    public TMP_Text ammoCounter;

    private float fireTimer;
    private int currentMag;

    public EnemyAI enemyAI; //reference to enemy ai script
    public Transform cam; //reference to player camera

    private void Start()
    {
        currentMag = magSize;
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

        // Reloading the weapon when the player presses the reaload button
        if (Input.GetKeyDown(reloadKey))
        {
            ReloadWeapon();
        }

        ammoCounter.text = currentMag.ToString("D2") + " / " + magSize.ToString("D2");
    }

    // Needs updating so not instant
    private void ReloadWeapon()
    {
        currentMag = magSize;
    }

    // Code that actually fires the weapon
    private void FireWeapon()
    {
        // Only firing when ammo > 0
        if (currentMag > 0)
        {
            currentMag--;

            // Hit enemies here
            Debug.Log("Fire");

            FindObjectOfType<AudioManager>().PlayOverlap("PlayerShoot"); //play player shoot audio

            RaycastHit hit;

            if (Physics.Raycast(cam.position, cam.forward, out hit, weaponRange, toAttack)) //if raycast hits an attackable enemy within weapon range
            {
                hit.transform.gameObject.GetComponent<EnemyAI>().TakeDamage(weaponDamage); //make enemy take damage equal to weapon damage
                Debug.Log("hit enemy"); //for testing if enemy was hit or not
            }
            else
            {
                Debug.Log("not hit enemy"); //for testing if enemy was hit or not
            }
        }
    }

    // Get the current ammo left in mag
    public int getMag()
    {
        return currentMag;
    }
}