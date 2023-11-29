using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grappling : MonoBehaviour
{
    [Header("References")]
    private PlayerMovement pm; //link to playermovement script
    public Transform cam; //camera
    public Transform gunTip; //tip of grapple gun
    public Transform player; //player object
    public LayerMask whatIsGrappleable; //what layer is grappleable
    public LineRenderer lr; //line renderer for drawing the grapple line - this is the line renderer on the grappling hook gun
    public LineRenderer playerLr; //line renderer for player object

    [Header("Grappling")]
    public float maxGrappleDistance; //max distance you can grapple
    public float grappleDelayTime; //delay between grapples
    public float overshootYAxis; //used for calculating grapple arc

    private Vector3 grapplePoint; //point of grapple

    [Header("Swinging")]
    private float maxSwingDistance = 25f; //max distance the user can swing from
    private Vector3 swingPoint; //private vector3 variable for point to swing from
    private SpringJoint joint; //private springjoint variable for 'springy' feel to swing

    private Vector3 currentSwingPosition; //vector3 variable for storing the current swing position

    [Header("Air Movement")]
    public Transform orientation; //variable for orientation
    public Rigidbody rb; //variable for rigidbody of player
    public float horizontalForce; //horizontal movement force
    public float forwardForce; //forward movement force
    public float extendSwingSpeed; //speed of extending the swing beam/hook

    [Header("Cooldown")]
    public float grapplingCd; //cooldown of grapple
    private float grapplingCdTimer; //timer of cooldown

    [Header("Input")]
    public KeyCode grappleKey = KeyCode.Mouse1; //key to grapple
    public KeyCode swingKey = KeyCode.Mouse2; //key to swing

    public bool grappling; //bool if user is grappling
    public bool cdActive; //bool if there is currently a grapple cooldown active

    private void Start()
    {
        pm = GetComponent<PlayerMovement>(); //get movement script
        //lr = GetComponent<LineRenderer>(); //get line renderer - for some reason this makes the line renderer bug out
    }

    private void Update()
    {
        if (Input.GetKeyDown(grappleKey)) //if grapple key pressed
        {
            if(grappling == false)
            {
                StartGrapple(); //start grapple
            }
        }

        if(grapplingCdTimer > 0) //is cooldown greater than 0
        {
            cdActive = true; //grapple cooldown is currently active
            grapplingCdTimer -= Time.deltaTime; //start cooldown timer
        }
        
        if(grapplingCdTimer <= 0) //if cooldown is less than or equal to 0
        {
            cdActive = false; //no grapple cooldown is currently active
        }

        if(Input.GetKeyDown(swingKey)) //if swing key pressed down
        {
            bool swingCheck = CanGrapple(); //check if player has an object to swing from

            if(swingCheck) //if they can swing
            {
                StartSwing(); //start swing function
            }
        }

        if(Input.GetKeyUp(swingKey)) //if swing key released
        {
            StopSwing(); //stop swing function
        }

        if(joint != null) //if joint has been created
        {
            AirMovement(); //allow for air movement
        }
    }

    private void LateUpdate()
    {
        if (grappling) //if user is grappling
        {
            lr.SetPosition(0, gunTip.position); //change position of line renderer to gun tip
        }

        if (joint) //if player is swinging and a 'joint' has been created
        {
            currentSwingPosition = Vector3.Lerp(currentSwingPosition, swingPoint, Time.deltaTime * 8f); //make grapple slowly go towards the grapple point

            playerLr.SetPosition(0, gunTip.position); //line renderer position 0 set to gun tip
            playerLr.SetPosition(1, currentSwingPosition); //line renderer position 1 set to swing point   ---   change currentSwingPosition to swingPoint to make it instant
        }
    }

    public bool CanGrapple()
    {
        RaycastHit hit; //create raycast hit variable
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable)) //if an object which is grappleable and within distance is hit
        {
            return true; //player can grapple
        }
        else //else if a grapple-able object is not in distance or aimed at
        {
            return false; //player cannot grapple
        }
    }

    private void StartGrapple()
    {
        StopSwing(); //stop a current swing

        if(grapplingCdTimer > 0) //if cooldown more than 0
        {
            return; //return as user cannot grapple
        }

        pm.grappling = true; //set grappling to true in PlayerMovement class
        grappling = true; //grappling is true
        //pm.freeze = true; //freeze player

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
        //pm.freeze = false; //player not frozen

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
        //pm.freeze = false; //player not frozen

        pm.grappling = false; //set bool to false in PlayerMovement class
        grappling = false; //user not grappling

        grapplingCdTimer = grapplingCd; //cooldown timer set to grapplingCd

        lr.enabled = false; //disable line renderer
    }

    private void StartSwing()
    {
        StopGrapple(); //stop any current grapple
        pm.ResetRestrictions(); //reset any current movement restrictions

        pm.swinging = true; //set swinging to true in PlayerMovement class

        RaycastHit hit; //create new raycast hit variable
        if(Physics.Raycast(cam.position, cam.forward, out hit, maxSwingDistance, whatIsGrappleable)) //if valid swing point has been found
        {
            swingPoint = hit.point; //swing point set to raycast hit point
            joint = player.gameObject.AddComponent<SpringJoint>(); //joint is a spring joint on the player game object
            joint.autoConfigureConnectedAnchor = false; //set to false
            joint.connectedAnchor = swingPoint; //point to swing from set to swing point variable

            float distanceFromPoint = Vector3.Distance(player.position, swingPoint); //distance from the swing point

            joint.maxDistance = distanceFromPoint * 0.8f; //max distance the player will be distanced from the swing point
            joint.minDistance = distanceFromPoint * 0.25f; //min distance the player will be distanced from the swing point

            //CAN CHANGE THE VALUES BELOW IF NEEDED

            joint.spring = 4.5f; //spring value
            joint.damper = 7f; //damper value
            joint.massScale = 4.5f; //mass scale value

            playerLr.positionCount = 2;
            currentSwingPosition = gunTip.position;
        }
    }

    private void StopSwing()
    {
        pm.swinging = false; //set swinging to false in PlayerMovement class

        playerLr.positionCount = 0; //set position of line renderer
        Destroy(joint); //destroy the swing
    }

    private void AirMovement()
    {
        if (Input.GetKey(KeyCode.A)) //if A is pressed
        {
            rb.AddForce(-orientation.right * horizontalForce * Time.deltaTime); //add force to the left of the orientation
        }

        if(Input.GetKey(KeyCode.D)) //if D is pressed
        {
            rb.AddForce(orientation.right * horizontalForce * Time.deltaTime); //add force to the right of the orientation
        }

        if (Input.GetKey(KeyCode.W)) //if W is pressed
        {
            rb.AddForce(orientation.forward * forwardForce * Time.deltaTime); //add force in the forward direction of the orientation
        }

        if (Input.GetKey(KeyCode.S)) //if S is pressed
        {
            rb.AddForce(-orientation.forward * forwardForce * Time.deltaTime); //add force in the backward direction of the orientation
        }

        if(Input.GetKey(KeyCode.Space)) //if space pressed
        {
            Vector3 directionToPoint = swingPoint - transform.position; //get direction to the swing point
            rb.AddForce(directionToPoint.normalized * forwardForce * Time.deltaTime); //add force to rigidbody towards the swing point
            float distanceFromPoint = Vector3.Distance(transform.position, swingPoint); //distance to the swing point

            joint.maxDistance = distanceFromPoint * 0.8f; //max distance to joint is set
            joint.minDistance = distanceFromPoint * 0.25f; //minimum distance to joint is set
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendSwingSpeed; //new distance from the swing point so player can go further from swing point

            joint.maxDistance = extendedDistanceFromPoint * 0.8f; //max distance to joint is set
            joint.minDistance = extendedDistanceFromPoint * 0.25f; //minimum distance to joint is set
        }
    }
}
