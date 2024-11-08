using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 movDirection;
    [SerializeField] private Transform orientation;
    [SerializeField] private PlayerCam playerCam;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;

    [SerializeField] private float groundDrag;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask groundMask;
    [SerializeField]private bool isGrounded;

    [Header("Jumping")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField] private bool readyToJump;

    [Header("Keybinds")]
    [SerializeField] private KeyCode jumpKey = KeyCode.Space;
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    [Header("Interaction")]
    [SerializeField] private Weapon weaponEquiped;
    [SerializeField] private int interactRange;

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
    }

    // Update is called once per frame
    void Update()
    {   
        MyInput();
        Move();
        SpeedControl();
        CheckForInteraction();

        if(Input.GetButtonDown("Fire1") && weaponEquiped != null)
        {
            weaponEquiped.Fire();
        }
    }
    private void FixedUpdate()
    {
        GroundCheck();  
    }

    private void MyInput()
    {
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        if(Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void Move()
    {
        movDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if(isGrounded)
            rb.AddForce(movDirection.normalized * moveSpeed, ForceMode.Force); //rb.velocity = new Vector3(horizontalInput, 0, verticalInput) * speed * Time.deltaTime;

        else
            rb.AddForce(movDirection.normalized * moveSpeed * airMultiplier, ForceMode.Force);
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
    private void CheckForInteraction()
    {
        RaycastHit hit;
        Debug.DrawRay(transform.position, transform.forward * interactRange, Color.red);
        
        if (Physics.Raycast(transform.position, transform.forward, out hit, interactRange))
        {
            // Check if the object hit has an IInteractable component
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                // Call Interact on the interactable object
                Debug.DrawRay(transform.position, transform.forward * interactRange, Color.green);

                if (Input.GetKeyDown(interactKey))
                {
                    interactable.Interact();
                }
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
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
}
