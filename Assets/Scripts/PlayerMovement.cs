using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerMovement : MonoBehaviour
{
    private float moveSpeed;
    public float walkSpeed;
    public float sprintSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    public bool readyToJump;

    public float playerHeight;
    bool grounded;

    public Transform orientation;

    public InputAction moveAction;
    public InputAction JumpAction;
    public InputAction SprintAction;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;

    public MovementState state;

    public TMP_Text speed2d;
    public TMP_Text speedabs;

    public enum MovementState 
    { 
        walking,
        sprinting,
        air
    }

    private void OnEnable()
    {
        moveAction.Enable();
        JumpAction.Enable();
        SprintAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
        JumpAction.Disable();
        SprintAction.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;

        moveSpeed = walkSpeed;
    }
    private void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f);
        InputDetection();
        HandleDrag();
        SpeedDebug();
    }
    private void FixedUpdate()
    {
        StateHandler();
        MovePlayer();
        ControlSpeed();
    }
    private void InputDetection()
    {
        Vector2 inputs = moveAction.ReadValue<Vector2>();
        horizontalInput = inputs.x;
        verticalInput = inputs.y;

        if (JumpAction.WasPressedThisFrame() && readyToJump && grounded) 
        { 
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void StateHandler()
    {
        if (SprintAction.IsPressed())
        {
            state = MovementState.sprinting;
            moveSpeed = sprintSpeed;
        }
        else if (!grounded)
        {
            state = MovementState.air;
        }
        else
        {
            state = MovementState.walking;
            moveSpeed = walkSpeed;
        }
    }

    private void HandleDrag()
    {
        if (grounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0f;
        }
    }
    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (moveDirection.sqrMagnitude > 1f)
        {
            moveDirection.Normalize();
        }

        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        if (flatVel.magnitude >= moveSpeed)
            return;

        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
        }
        else
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }
     }
    private void ControlSpeed()
    {
        Vector3 flatVel = new Vector3(rb.linearVelocity.x, 0f , rb.linearVelocity.z);
        if (flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.linearVelocity = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
        }
    }
    private void Jump()
    {
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void SpeedDebug()
    {
        speed2d.text = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z).magnitude.ToString();
        speedabs.text = new Vector3(rb.linearVelocity.x, rb.linearVelocity.y, rb.linearVelocity.z).magnitude.ToString();
    }
}
