using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthBar : MonoBehaviour
{
    public Camera targetCamera;
    public Slider healthBar;

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(targetCamera.transform.position);
    }

    public void updateHealth(float currentValue, float maxValue)
    {
        healthBar.value = currentValue / maxValue;
    }
}
