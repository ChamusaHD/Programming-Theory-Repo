using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 moveDirection;

    public MovementState currentState;
    public enum MovementState 
    { 
        walking,
        sprinting,
        crouching,
        airborne
    }

    [SerializeField] private Transform orientation;
    [SerializeField] private PlayerCam playerCam;

    [Header("Movement")]
    private float moveSpeed = 7f;
    [SerializeField] private float walkSpeed = 3f;
    [SerializeField] private float sprintSpeed = 3f;

    [SerializeField] private float groundDrag;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundMask;
    [SerializeField]private bool isGrounded;

    [Header("Slope Handling")]
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;
    private bool exitingSlope;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] private bool readyToJump;

    [Header("Crouching")]
    [SerializeField] private float crouchingSpeed = 3f;
    [SerializeField] private float crouchYScale;
    private float startYScale;
     
    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] private KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Interaction")]
    [SerializeField] public Weapon weaponEquiped = null;
    [SerializeField] private int interactRange;
    private IInteractable interactableObject;

    private float horizontalInput;
    private float verticalInput;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    void Start()
    {
        rb.freezeRotation = true;
        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    void Update()
    {   
        MyInput();
        Move();
        SpeedControl();
        StateHandler();

        if (Input.GetButtonDown("Fire1") && weaponEquiped != null)
        {
            weaponEquiped.Fire();
        }

        Debug.Log((int)rb.velocity.magnitude);
    }
    private void FixedUpdate()
    {
        GroundCheck();  
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // When To Jump
        if(Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        // Start croutching
        if (Input.GetKeyDown(crouchKey))
        {
            currentState = MovementState.crouching;
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        }
        // Stop croutching
        if (Input.GetKeyUp(crouchKey))
        {
            currentState = MovementState.walking;
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    private void StateHandler()
    {
        //if(Input.GetKeyDown(crouchKey))
        //{
        //    currentState = MovementState.crouching;
        //    moveSpeed = crouchingSpeed;
        //}

        //if(isGrounded && Input.GetKey(sprintKey))
        //{
        //    currentState = MovementState.sprinting;
        //    moveSpeed = sprintSpeed;
        //}
        //else if (isGrounded)
        //{
        //    currentState = MovementState.walking;
        //    moveSpeed = walkSpeed;
        //}
        //else
        //{
        //    currentState = MovementState.airborne;
        //}

        switch (currentState) {

            case MovementState.walking:
                moveSpeed = walkSpeed;
                break;
            case MovementState.sprinting:
                moveSpeed = sprintSpeed;
                break;
            case MovementState.crouching:
                moveSpeed = crouchingSpeed;
                break;
            case MovementState.airborne:
                moveSpeed = walkSpeed;
                break;
        }
    }

    private void Move()
    {
        //Calculate Movement Direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(onSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * 20f, ForceMode.Force);

            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }

        }

        if (isGrounded) // Walking
        {
            currentState = MovementState.walking; 
            //moveSpeed = walkSpeed;

            if (Input.GetKey(sprintKey)) //Sprinting
            {
                currentState = MovementState.sprinting;
                //moveSpeed = sprintSpeed;
            }

            if (Input.GetKey(crouchKey)) //Crouching
            {
                currentState = MovementState.crouching;
               // moveSpeed = crouchingSpeed;
            }

            rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force); //rb.velocity = new Vector3(horizontalInput, 0, verticalInput) * speed * Time.deltaTime;
        }

        else // airborne
        {
            currentState = MovementState.airborne;
            rb.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);
        }

        //Turn gravity off while on Slope
        rb.useGravity = !onSlope();
    }

    private void SpeedControl()
    {
        // Limiting speed on slope
        if (onSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }

        // Limiting speed on ground and on air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            }
        }
    }
    private void GroundCheck()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, groundMask);
        Debug.DrawRay(transform.position, Vector3.down * playerHeight * (0.5f + 0.2f), color: Color.magenta);

        if (isGrounded)
        {
            rb.drag = groundDrag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        //Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    private bool onSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(slopeHit.normal, Vector3.up);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    # region Getters and Setters

    public void SetWeaponEquiped(Weapon itemToEquip)
    {
        weaponEquiped = itemToEquip as Weapon;
        print("Equiped: " + weaponEquiped.name);
    }
    public Weapon GetCurrentWeaponEquiped()
    {
        return weaponEquiped;
    }
    public void SetWeaponToNull()
    {
        weaponEquiped.gameObject.SetActive(false);
        weaponEquiped = null;
    }
    #endregion
}
