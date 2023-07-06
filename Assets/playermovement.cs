using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Analytics;

public class playermovement : MonoBehaviour
{
    public Transform cam;
    [Header("movement")]
    public float moveSpeed;

    public float groundDrag;

    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody rb;
    private object limitedVel;

    public Transform groundCheck;
    public float groundDistance = 0.4f;

    [Header("Ledge Detection")]
    public float ledgeDetectionLenght;
    public float ledgeSphereCastRadius;
    public LayerMask whatIsLedge;
    public float maxLedgeGrabDistance;
    bool ledgeDetected;

    private Transform lastLedge;
    private Transform currLedge;

    private RaycastHit ledgeHit;
    public float ledgeDrag;
    bool readyToWallJump;
    public float wallJumpForce;
    public float wallJumpCooldown;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        readyToJump = true;
        
    }

    private void Update()
    {
        // ground check
        if (!ledgeDetected)  
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        grounded = Physics.CheckSphere(groundCheck.position, groundDistance, whatIsGround);
        Myinput();
        SpeedControl();


        // handle drag
        if (grounded)
            rb.drag = groundDrag;
        else
            rb.drag = 0;
        

        LedgeDetection();
        
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    private void Myinput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //when to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void MovePlayer()
    {
        //calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on ground
        if (grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if (!grounded)
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        
    }

    private void SpeedControl()
    {
        Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        // limit velocity if needed
        if(flatVel.magnitude > moveSpeed)
        {
            Vector3 limitedVel = flatVel.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
        }
    }

    private void Jump()
    {
        //reset y velocity
        rb.velocity = new Vector3(rb.velocity.x,0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void wallJump()
    {
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * wallJumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
    }
    private void ResetWallJump()
    {
        readyToWallJump = true;
    }

    private void LedgeDetection()
    {
        bool ledgeDetected = Physics.SphereCast(transform.position, ledgeSphereCastRadius, cam.forward, out ledgeHit, ledgeDetectionLenght, whatIsLedge);
        if (Input.GetKey(jumpKey) && !grounded)
        {
            ResetWallJump();
            
        }

        if (ledgeDetected && Input.GetKey(jumpKey) && readyToWallJump)
  
        {
            wallJump();
            readyToWallJump = false;
            Invoke(nameof(ResetWallJump), wallJumpCooldown);
        }
    }
}

