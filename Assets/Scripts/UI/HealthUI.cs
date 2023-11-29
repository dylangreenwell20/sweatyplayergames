using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{
    public Animator healthUI;
    public GameObject brokenHeartUI;

    void Start()
    {
        healthUI = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(healthUI != null)
        {
            if (Input.GetKeyDown(KeyCode.O))
            {
                healthUI.SetTrigger("HealthDropped");
            }

            if (Input.GetKeyDown(KeyCode.P))
            {
                healthUI.SetTrigger("HealthHeal");
            }
        }
    }
}
