using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class DamageHandler : MonoBehaviour
{
    // Stats to do with this objects health
    public float maxHealth;
    public float health;
    public bool playerTakeDmg;

    public GameObject takeDmgEffect;
    public float intensity = 0;
    PostProcessVolume volume;
    Vignette vignette;

    private void Start()
    {
        health = maxHealth;

        volume = takeDmgEffect.GetComponent<PostProcessVolume>();
        volume.profile.TryGetSettings<Vignette>(out vignette);

        if (!vignette)
        {
            Debug.Log("error, vignette empty");
        }

        else
        {
            vignette.enabled.Override(false);
        }
    }

    // Deal damage to this objects health
    public void DealDamage(float damage)
    {
        Debug.Log(damage);
        health -= damage;
        playerTakeDmg = true;
    }

    private void Update()
    {
        // Check when this object is dead
        if (health <= 0) 
        {
            Death();
        }

        if(playerTakeDmg == true)
        {
            StartTakeDmg();
        }
    }
       
    // Object death logic goes here
    private void Death()
    {

    }

    public void StartTakeDmg()
    {
        StartCoroutine(TakeDmgEffect());
        playerTakeDmg = false;
    }
    public IEnumerator TakeDmgEffect()
    {
        intensity = 0.3f;

        vignette.enabled.Override(true);
        vignette.intensity.Override(0.3f);

        yield return new WaitForSeconds(0.2f);

        while (intensity > 0)
        {
            intensity -= 0.01f;

            if (intensity < 0)
            {
                intensity = 0;
            }

            vignette.intensity.Override(intensity);

            yield return new WaitForSeconds(0.1f);
        }

        vignette.enabled.Override(false);
        yield break;
    }
}
