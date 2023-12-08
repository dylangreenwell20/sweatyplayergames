using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class TakeDmg : MonoBehaviour
{
    public float intensity = 0;

    PostProcessVolume volume;
    Vignette vignette;

    DamageHandler damageHandler;

    // Start is called before the first frame update
    void Start()
    {
        volume = GetComponent<PostProcessVolume>();

        volume.profile.TryGetSettings<Vignette>( out vignette );

        if (!vignette)
        {
            Debug.Log("error, vignette empty");
        }

        else
        {
            vignette.enabled.Override(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*if (damageHandler.playerTakeDmg = true)
        {
            StartCoroutine(TakeDmgEffect());
            damageHandler.playerTakeDmg = false;
        }*/

        if (Input.GetKeyDown(KeyCode.M))
        {
            StartCoroutine(TakeDmgEffect());
        }
    }

    public void playTakeDmg()
    {
        StartCoroutine (TakeDmgEffect());
    }

    public IEnumerator TakeDmgEffect()
    {
        intensity = 0.3f;

        vignette.enabled.Override(true);
        vignette.intensity.Override(0.3f);

        yield return new WaitForSeconds(0.3f);

        while(intensity > 0)
        {
            intensity -= 0.01f;

            if(intensity < 0)
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
