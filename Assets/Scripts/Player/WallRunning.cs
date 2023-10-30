using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    // Settings related to player wall running
    [Header("WallRunning")]
    public LayerMask whatIsWall;
    public LayerMask whatIsGround;
    
    public float wallRunForce;
    public float maxWallRunTime;
    private float wallRunTimer;

    // Force for the jump off a wall
    public float wallJumpVertical;
    public float wallJumpHorizontal;

    // Current inputs
    [Header("Inputs")]
    private float horizontalInput;
    private float verticalInput;

    // Settings for detecting walls
    [Header("Detection")]
    public float wallCheckDistance;
    public float minJumpHeight;

    // Variables to store information about the wall positions
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    [Header("Exit Wall")]
    private bool exitingWall;
    public float exitWallTime;
    private float exitWallTimer;

    // References to various required objects
    [Header("References")]
    public Transform orientation;
    private PlayerMovement pm;
    private Rigidbody rb;

    private void Start()
    {
        // Getting player rigid body and movement script
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pm.wallRunning)
        {
            WallRunMove();
        }
    }

    // Checking both sides for wall
    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    // Check if player high enough to start wallrun
    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    // Function to decide which state the player is currently in 
    private void StateMachine()
    {
        // Get inputs
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Wall running state
        if((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            if (!pm.wallRunning)
            {
                StartWallRun();
            }

            if (wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }

            if (wallRunTimer < 0 && pm.wallRunning)
            {
                exitingWall = true;
                exitWallTimer = exitWallTime;
            }

            if (Input.GetKeyDown(pm.jumpKey))
            {
                wallJump();
            }
        }
        // Currently leaving a wall run State
        else if (exitingWall)
        {
            if (pm.wallRunning)
            {
                EndWallRun();
            }

            if (exitWallTimer > 0)
            {
                exitWallTimer -= Time.deltaTime;
            }

            if (exitWallTimer < 0)
            {
                exitingWall = false;
            }
        }

        // No state
        else
        {
            if (pm.wallRunning)
            {
                EndWallRun();
            }
        }
    }

    // WallRunning functions
    private void StartWallRun()
    {
        wallRunTimer = maxWallRunTime;

        pm.wallRunning = true;
        rb.useGravity = false;
    }
    private void WallRunMove()
    {
        
        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if(!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force);
        }
    }
    private void EndWallRun()
    {
        rb.useGravity = true;
        pm.wallRunning = false;
    }

    private void wallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = wallRight ? rightWallHit.normal: leftWallHit.normal;
        Vector3 forceToApply = transform.up * wallJumpVertical + wallNormal * wallJumpHorizontal;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
