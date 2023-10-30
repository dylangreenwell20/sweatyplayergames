using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public LayerMask whatIsWall;
    private PlayerMovement pm;
    private Rigidbody rb;

    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    private float climbTimer;

    private bool isClimbing;

    [Header("Climbing Jump")]
    public float upJumpForce;
    public float backJumpForce;

    public int climbJumps;
    private int climbJumpsLeft;

    private Transform lastWall;
    private Vector3 lastWallNormal;
    public float minWallNormalAngleChange;

    [Header("Wall Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    private float currWallLookAngle;

    private RaycastHit frontWallHit;
    private bool isWallFront;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pm = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        WallCheck();
        StateMachine();

        if (isClimbing)
        {
            ClimbingMovement();
        }
    }

    private void StateMachine()
    {
        // Climbing State
        if(isWallFront && Input.GetKey(KeyCode.W) && currWallLookAngle < maxWallLookAngle)
        {
            if (!isClimbing && climbTimer > 0)
            {
                StartClimbing();
            }

            if (climbTimer > 0)
            {
                climbTimer -= Time.deltaTime;
            }
            if (climbTimer <= 0)
            {
                StopClimbing();
            }
        }

        // No Climbing State
        else
        {
            if (isClimbing)
            {
                StopClimbing();
            }
        }

        if (isWallFront && Input.GetKeyDown(pm.jumpKey) && climbJumpsLeft > 0)
        {
            ClimbJump();
        }
    }

    private void WallCheck()
    {
        isWallFront = Physics.SphereCast(transform.position, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength, whatIsWall);
        currWallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);

        bool newWall = frontWallHit.transform != lastWall || Mathf.Abs(Vector3.Angle(lastWallNormal, frontWallHit.normal)) > minWallNormalAngleChange;

        if ((isWallFront && newWall) || pm.isGrounded)
        {
            climbTimer = maxClimbTime;
            climbJumpsLeft = climbJumps;
        }
    }

    private void StartClimbing()
    {
        isClimbing = true;

        lastWall = frontWallHit.transform;
        lastWallNormal = frontWallHit.normal;
    }
    private void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }
    private void StopClimbing()
    {
        isClimbing = false;
    }

    private void ClimbJump()
    {
        Vector3 forceToApply = transform.up * upJumpForce + frontWallHit.normal * backJumpForce;

        rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        rb.AddForce(forceToApply, ForceMode.Impulse);

        climbJumpsLeft--;
    }
}
