using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Script to Play the particle systems when the muzzle flash should show
public class MuzzleFlashHandle : MonoBehaviour
{
    public ParticleSystem sparks;
    public ParticleSystem glow;
    public ParticleSystem muzzle;

    // Play all 3 particle systems
    private void OnEnable()
    {
        muzzle.Play();
        sparks.Play();
        glow.Play();
    }

    private void OnDisable()
    {
        muzzle.Stop();
        sparks.Stop();
        glow.Stop();
    }
}
