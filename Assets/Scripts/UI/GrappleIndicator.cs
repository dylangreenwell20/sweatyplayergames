using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GrappleIndicator : MonoBehaviour
{
    private bool canGrapple; //bool to check if player can grapple
    private bool isGrappling; //check if the grapple is currently on cooldown
    private bool isCdActive; //check if grapple cooldown is active
    public Grappling grapple; //reference to Grappling script
    public Image crosshair; //crosshair image reference

    private void Awake()
    {
        crosshair.color = Color.red; //set crosshair to red as soon as the game loads
    }

    private void Update()
    {
        canGrapple = grapple.CanGrapple(); //check if the player is currently looking at an object that is grappleable
        isGrappling = grapple.grappling; //check if player is currently grappling
        isCdActive = grapple.cdActive; //check if cooldown is currently active or not from the Grappling script

        if(canGrapple && !isGrappling && !isCdActive) //if player can grapple and player is not grappling and there is no grapple cooldown active
        {
            crosshair.color = Color.green; //change crosshair colour to green
        }

        else //else if player cannot grapple
        {
            crosshair.color = Color.red; //change crosshair colour to red
        }
    }
}
