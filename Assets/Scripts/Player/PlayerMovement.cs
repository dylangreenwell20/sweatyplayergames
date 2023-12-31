using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Options")]
    private float movementSpeed;
    public float walkSpeed;
    public float sprintSpeed;
    public float slideSpeed;
    public float wallRunSpeed;
    public float grappleSpeed; //max speed of grappling
    public float swingSpeed; //max speed of swinging

    private float desiredMoveSpeed;
    private float lastDesiredMoveSpeed;


    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;

    bool canJump = true;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;

    [Header("Keybind")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    public bool isGrounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    private RaycastHit onSlope;
    private bool exitSlope;

    [Header("References")]
    public Transform orientation;

    Vector2 keyboardInput;
    Rigidbody rb;
    Vector3 moveDirection;

    public LayerMask whatIsExit; //layer reference for level exit
    public bool isOnExit; //if player is on exit
    public DemoTimer dt; //reference to demo timer script

    public Transform player; //reference to player
    public Transform destination; //reference to level start destination

    public LayerMask whatIsReset; //layer which is touched when a player falls on a course section and needs to be teleported back up
    public bool isOnReset; //if player is on a reset barrier or not

    public Grappling grappleScript; //reference to grapple script


    public MovementState playerState;
    public enum MovementState
    {
        freeze,
        walking,
        sprinting,
        wallrunning,
        crouching,
        sliding,
        grappling,
        swinging,
        inAir
    }

    public bool freeze; //player is frozen bool
    public bool activeGrapple; //active grapple bool
    public bool sliding;
    public bool wallRunning; // Wall running bool
    public bool grappling; //is player grappling or not
    public bool swinging; //is player swinging or not

    public bool isMoving; //is player moving or not

    private void Awake()
    {
        player.position = destination.position; //set player destination to level start position
    }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startYScale = transform.localScale.y;
        FindObjectOfType<AudioManager>().Play("BackgroundMusic"); //play background music
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, whatIsGround); //check if player is on the ground layer
        isOnExit = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, whatIsExit); //check if player is on the exit layer
        isOnReset = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, whatIsReset); //check if player is on the reset layer

        GetInputs();
        SpeedControl();
        StateHandler();

        if(isOnReset) //if player is on reset
        {
            grappleScript.StopSwing(); //stop swing
        }

        float speedCheck = rb.velocity.magnitude; //check the speed of the player

        if(speedCheck <= 0.5f) //if player moving slower than 0.5f velocity
        {
            isMoving = false; //player not moving
        }
        else //else if player moving more than 0.5f velocity
        {
            isMoving = true; //player is moving
        }

        if (isGrounded && !activeGrapple) //if player is on the ground and not currently grappling
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }

        if (isOnExit) //if player is touching the exit
        {
            dt.ExitTouched(); //this function sets the exitReached boolean to true which stops the timer
        }
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void GetInputs()
    {
        if (Input.GetKey(jumpKey) && canJump && isGrounded)
        {
            canJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (Input.GetKeyDown(crouchKey))
        {
            //change to Y Scale of the object, replace with model code
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        if (Input.GetKeyUp(crouchKey))
        {
            //same as KeyDown
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }

        keyboardInput = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void StateHandler()
    {
        if (wallRunning)
        {
            playerState = MovementState.wallrunning;
            desiredMoveSpeed = wallRunSpeed;

            //wall running audio here
        }

        if (freeze) //if user is frozen
        {
            playerState = MovementState.freeze; //freeze movement state
            movementSpeed = 0; //speed set to 0
            rb.velocity = Vector3.zero; //velocity set to 0
        }
        else if (sliding)
        {
            playerState = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;

            else
                desiredMoveSpeed = sprintSpeed;

            //slide audio here
        }

        else if (Input.GetKey(crouchKey))
        {
            playerState = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
            //crouch audio here
        }

        else if (isGrounded && Input.GetKey(sprintKey))
        {
            playerState = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;

            if(isMoving && !grappling && !swinging) //if player is moving but not grappling/swinging
            {
                FindObjectOfType<AudioManager>().Play("PlayerSprint"); //play player sprint audio
            }
        }

        else if (isGrounded)
        {
            playerState = MovementState.walking;
            desiredMoveSpeed = walkSpeed;

            if(isMoving && !grappling && !swinging) //if player is moving but not grappling/swinging
            {
                FindObjectOfType<AudioManager>().Play("PlayerWalk"); //play player walk audio
            }
        }

        else if (grappling) //else if the player is grappling
        {
            playerState = MovementState.grappling; //set player state to grappling
            desiredMoveSpeed = grappleSpeed; //set max speed to the grapple speed
        }

        else if (swinging) //else if the player is swinging
        {
            playerState = MovementState.swinging; //set player state to swinging
            desiredMoveSpeed = swingSpeed; //set max speed to the swing speed
        }

        else
        {
            playerState = MovementState.inAir;
        }

        if (Mathf.Abs(desiredMoveSpeed - lastDesiredMoveSpeed) > 4f && movementSpeed != 0)
        {
            StopAllCoroutines();
            StartCoroutine(SmoothlyLerpMoveSpeed());
        }
        else
        {
            movementSpeed = desiredMoveSpeed;
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
    }

    private IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float diff = Mathf.Abs(desiredMoveSpeed - movementSpeed);
        float startValue = movementSpeed;

        while (time < diff)
        {
            movementSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / diff);
            time += Time.deltaTime;
            yield return null;
        }

        movementSpeed = desiredMoveSpeed;
    }
    void MovePlayer()
    {
        if (activeGrapple) //if player is actively grappling
        {
            return; //return with no movement
        }

        if (swinging) //if player is swinging
        {
            return; //return with no movement
        }

        moveDirection = orientation.forward * keyboardInput.y + orientation.right * keyboardInput.x;

        if (OnSlope() && !exitSlope)
        {
            rb.AddForce(GetSlopeMoveDir(moveDirection) * movementSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
                
        }
        if (isGrounded)
        {
            rb.AddForce(moveDirection * movementSpeed * 10f, ForceMode.Force);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection * movementSpeed * 10f * airMultiplier, ForceMode.Force);
        }
    }

    void Jump()
    {
        exitSlope = true;
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    void ResetJump()
    {
        exitSlope = false;
        canJump = true;
    }

    void SpeedControl()
    {
        if (activeGrapple) //if player is actively grappling
        {
            return; //return with no speed
        }

        if (OnSlope() && !exitSlope)
        {
            if (rb.velocity.magnitude > movementSpeed)
                rb.velocity = rb.velocity.normalized * movementSpeed;
        }

        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            if (flatVel.magnitude > movementSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * movementSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }

    private bool enableMovementOnNextTouch; //let player move after touching a surface after a grapple

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true; //player actively grappling

        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight); //set velocity
        Invoke(nameof(SetVelocity), 0.1f); //apply velocity after 0.1s

        Invoke(nameof(ResetRestrictions), 3f); //reset movement restrictions after 3 seconds incase something goes wrong
    }

    private Vector3 velocityToSet; //velocity to set variable

    private void SetVelocity()
    {
        enableMovementOnNextTouch = true; //let player move on next surface touch
        rb.velocity = velocityToSet; //set velocity of rigid body to variable velocityToSet
    }

    public void ResetRestrictions()
    {
        activeGrapple = false; //set activeGrapple to false
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch) //if enableMovementOnNextTouch is true
        {
            enableMovementOnNextTouch = false; //set it to false
            ResetRestrictions(); //run reset restrictions function

            GetComponent<Grappling>().StopGrapple(); //stop grapple
            return;
        }

        if (isOnReset) //if player is on a reset layer
        {
            collision.gameObject.GetComponent<ResetTeleport>().teleportPlayer(); //teleport player to assigned checkpoint
            isOnReset = false;
            return;
        }
    }

    public bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out onSlope, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, onSlope.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    public Vector3 GetSlopeMoveDir(Vector3 dic)
    {
        return Vector3.ProjectOnPlane(dic, onSlope.normal).normalized;
    }

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y; //gravity float
        float displacementY = endPoint.y - startPoint.y; //float for y displacement
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z); //displacement of X and Z

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight); //Y velocity
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity)); //XZ velocity

        return velocityXZ + velocityY; //return XZ and Y velocity values
    }
}
