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

    public MovementState playerState;
    public enum MovementState
    {
        walking,
        sprinting,
        crouching,
        sliding,
        inAir
    }

    public bool sliding;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        startYScale = transform.localScale.y;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.1f, whatIsGround);

        GetInputs();
        SpeedControl();
        StateHandler();

        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
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
        if (sliding)
        {
            playerState = MovementState.sliding;

            if (OnSlope() && rb.velocity.y < 0.1f)
                desiredMoveSpeed = slideSpeed;

            else
                desiredMoveSpeed = sprintSpeed;
        }

        else if (Input.GetKey(crouchKey))
        {
            playerState = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }

        else if (isGrounded && Input.GetKey(sprintKey))
        {
            playerState = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }

        else if (isGrounded)
        {
            playerState = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
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
        moveDirection = orientation.forward * keyboardInput.y + orientation.right * keyboardInput.x;

        if (OnSlope() && !exitSlope)
        {
            rb.AddForce(GetSlopeMoveDir(moveDirection) * movementSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
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
}
