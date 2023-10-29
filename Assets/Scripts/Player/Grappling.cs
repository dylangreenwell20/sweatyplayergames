using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement pm; //link to playermovement script
    public Transform cam; //camera
    public Transform gunTip; //tip of grapple gun
    public LayerMask whatIsGrappleable; //what layer is grappleable
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance; //max distance you can grapple
    public float grappleDelayTime; //delay between grapples
    public float overshootYAxis; //used for calculating grapple arc

    private Vector3 grapplePoint; //point of grapple

    [Header("Cooldown")]
    public float grapplingCd; //cooldown of grapple
    private float grapplingCdTimer; //timer of cooldown

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1; //key to grpaple

    private bool grappling; //bool if user is grappling

    private void Start()
    {
        pm = GetComponent<PlayerMovement>(); //get movement script
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey)) //if grapple key pressed
        {
            StartGrapple(); //start grapple
        }

        if(grapplingCdTimer > 0) //is cooldown greater than 0
        {
            grapplingCdTimer -= Time.deltaTime; //start cooldown timer
        }
    }

    private void LateUpdate()
    {
        if (grappling) //if user is grappling
        {
            lr.SetPosition(0, gunTip.position); //change position of line renderer to gun tip
        }
    }

    private void StartGrapple()
    {
        if(grapplingCdTimer > 0) //if cooldown more than 0
        {
            return; //return as user cannot grapple
        }

        grappling = true; //grappling is true
        pm.freeze = true; //freeze player

        RaycastHit hit; //new raycast
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable)) //if an object which is grappleable and within distance is hit
        {
            grapplePoint = hit.point; //set grapple point
            Invoke(nameof(ExecuteGrapple), grappleDelayTime); //grapple delay before it activates
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance; //grapple distance shown
            Invoke(nameof(StopGrapple), grappleDelayTime); //stop grapple will begin after a delay
        }

        lr.enabled = true; //enable line renderer
        lr.SetPosition(1, grapplePoint); //set line renderer positions
    }

    private void ExecuteGrapple()
    {
        pm.freeze = false; //player not frozen

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z); //lowest point of the player

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y; //difference of y axis between player and grapple point
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis; //add overshoot y axis to y difference

        if(grapplePointRelativeYPos < 0) //if point is below player
        {
            highestPointOnArc = overshootYAxis; //use overshoot y axis to calculate arc
        }

        pm.JumpToPosition(grapplePoint, highestPointOnArc); //run JumpToPosition function

        Invoke(nameof(StopGrapple), 1f); //stop grapple after 1 second
    }

    public void StopGrapple()
    {
        pm.freeze = false; //player not frozen

        grappling = false; //user not grappling

        grapplingCdTimer = grapplingCd; //cooldown timer set to grapplingCd

        lr.enabled = false; //disable line renderer
    }
}
