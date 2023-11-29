using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DemoTimer : MonoBehaviour
{
    public TMP_Text timeText; //reference to text for time to be displayed on
    public float time; //time variable
    public bool exitReached; //has player reached exit or not

    private void Update()
    {
        if (!exitReached) //if exit not reached
        {
            time += Time.deltaTime; //increase time
            UpdateTimer(time); //update the timer text
        }
        else //else if exit has been reached
        {
            Debug.Log("exit reached"); //for testing to see if exit has been reached
        }
    }

    public void UpdateTimer(float currentTime)
    {
        currentTime += 1; //increment currentTime

        float minutes = Mathf.FloorToInt(currentTime / 60); //calculate number of minutes
        float seconds = Mathf.FloorToInt(currentTime % 60); //calculate number of seconds using modulus

        timeText.text = string.Format("{0:00} : {1:00}", minutes, seconds); //change text of the text object with a certain format
    }

    public void ExitTouched()
    {
        exitReached = true; //set exitReached to true
    }
}
