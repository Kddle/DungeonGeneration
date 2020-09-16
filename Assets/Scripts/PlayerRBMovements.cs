using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRBMovements : MonoBehaviour
{
    public LayerMask WhatIsGround;
    public float WalkSpeed = 10f;
    public float RunBoost = 15f;
    public float CurrentMaxSpeed;
    public float JumpForce = 20f;
    public float FloorFriction = -5f;
    public bool isGrounded = false;
    CapsuleCollider capsuleCollider;
    Rigidbody rb;
    float moveSpeed = 0f;
    public float SlideForce = 40f;
    public float SlideFriction = -10f;
    float slideSpeed;
    Vector3 slideDirection;

    float currentSpeed;

    public bool isSliding = false;
    int currentMovement = 0; // 0 = idle, 1 = walking, 2 = running

    // Start is called before the first frame update
    void Start()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();

        moveSpeed = WalkSpeed;

        CurrentMaxSpeed = WalkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.CheckCapsule(capsuleCollider.bounds.center, capsuleCollider.bounds.min, capsuleCollider.radius, WhatIsGround);


        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                rb.AddForce(Vector3.up * JumpForce, ForceMode.Acceleration);
            }
        }

        if (Input.GetButtonDown("Run"))
        {
            CurrentMaxSpeed = RunBoost;
            currentSpeed += RunBoost;
        }

        if (Input.GetButtonUp("Run"))
        {
            CurrentMaxSpeed = WalkSpeed;
        }

        // if (Input.GetButtonDown("Crouch"))
        // {
        //     if (rb.velocity.magnitude > WalkSpeed)
        //     {
        //         slideDirection = rb.velocity.normalized;
        //         rb.AddForce(slideDirection * SlideForce, ForceMode.VelocityChange);
        //         isSliding = true;
        //     }
        // }

        // if (Input.GetButtonUp("Crouch"))
        // {
        //     isSliding = false;
        // }
    }

    void FixedUpdate()
    {
        if (currentSpeed > CurrentMaxSpeed)
        {
            if (isSliding)
            {
                currentSpeed += SlideFriction * Time.fixedDeltaTime;
            }
            else if (isGrounded)
            {
                Debug.Log("Apply floor friction");
                currentSpeed += FloorFriction * Time.fixedDeltaTime;
            }
        }

        var hInput = Input.GetAxisRaw("Horizontal");
        var vInput = Input.GetAxisRaw("Vertical");

        var moveDirection = isSliding ? slideDirection.normalized : (transform.forward * vInput + transform.right * hInput).normalized;
        var isMoving = (hInput != 0f) || (vInput != 0f);

        if (isMoving && currentSpeed == 0f)
            currentSpeed = WalkSpeed;

        var hMovement = moveDirection * currentSpeed;
        var vMovement = new Vector3(0, rb.velocity.y, 0);

        if (isMoving)
            rb.velocity = hMovement + vMovement;
        else
        {
            if (isGrounded)
                rb.velocity = new Vector3(0f, rb.velocity.y, 0f);
        }


        Debug.Log($"Velocity : {rb.velocity.magnitude}");
    }

    void StopSlide()
    {
        isSliding = false;
    }

    // void OnDrawGizmos()
    // {
    //     Gizmos.color = Color.red;
    //     var decal = new Vector3(0f, -.5f, 0f);
    //     Gizmos.DrawSphere(transform.position + decal , .6f);
    // }
}
