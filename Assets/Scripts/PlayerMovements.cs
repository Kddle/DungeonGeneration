using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovements : MonoBehaviour
{
    public float BaseSpeed = 5f;
    public float CrouchSpeed = 5f;

    public float Gravity = -30f;
    public float JumpForce = 15f;
    public float SlideBoost = 10f;

    public float v = 0f;
    public float v0 = 0f;
    public float a = 5f;
    public float vy = 0f;

    public float deltaX = 0f;
    public float deltaY = 0f;

    public float maxSpeed = 30f;

    public float SlideThreshold = 0.20f;

    //public float DashDistance = 20f;
    public Vector3 Drag;

    CharacterController characterController;
    Transform torso;

    Vector3 velocity = Vector3.zero;

    float upSpeed = 0f;
    float speed;
    float slideSpeed = 0f;

    bool isCrouching = false;
    bool isJumping = false;
    bool isSliding = false;

    bool FOVChanged = false;

    Vector3 slideDirection = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        characterController = GetComponent<CharacterController>();
        torso = transform.Find("Torso") ?? throw new System.Exception("Torso");
        speed = BaseSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        var hMovement = Input.GetAxis("Horizontal");
        var vMovement = Input.GetAxis("Vertical");



        if (hMovement > 0.1f || vMovement > 0.1f || hMovement < -0.1f || vMovement < -0.1f)
        {
            if (v0 == 0f)
                v0 = BaseSpeed;

            v = v0 + (a * Time.deltaTime);
            v = Mathf.Clamp(v, 0f, maxSpeed);

            deltaX = (v0 * Time.deltaTime) + (0.5f * a * Mathf.Pow(Time.deltaTime, 2f));
            //deltaX = ((v + v0) / 2.00f) * Time.deltaTime;
        }
        else
        {
            v = 0f;
            v0 = 0f;
            deltaX = 0f;
        }



        if (Input.GetButtonDown("Jump") && !isJumping)
            Jump();

        //deltaY = (vy * Time.deltaTime) + (.5f * Gravity * Mathf.Pow(Time.deltaTime, 2f));

        if (Input.GetButtonDown("Crouch"))
            StartCrouch();

        if (Input.GetButtonUp("Crouch"))
            EndCrouch();

        //if (Input.GetButtonDown("Hook"))
        //    StartHooking();

        //if (Input.GetButtonUp("Hook"))
        //    EndHooking();

        var moveDirection = (transform.forward * vMovement) + (transform.right * hMovement);
        velocity = (moveDirection.normalized * deltaX);
        //if (!isSliding)
        //    velocity = ((transform.forward * vMovement) + (transform.right * hMovement)) * speed * Time.deltaTime;
        //else if (isSliding && !isJumping)
        //    velocity = slideDirection * slideSpeed * Time.deltaTime;
        //else if (isSliding && isJumping)
        //    velocity = ((transform.forward * vMovement) + (transform.right * hMovement)) * (speed + slideSpeed) * Time.deltaTime;

        velocity.y += upSpeed * Time.deltaTime;

        // Apply Drag to movement
        //velocity.x /= 1 + Drag.x * Time.deltaTime;
        //velocity.y /= 1 + Drag.y * Time.deltaTime;
        //velocity.z /= 1 + Drag.z * Time.deltaTime;

        characterController.Move(velocity);
        //characterController.Move((transform.up.normalized * deltaY));

        if (isJumping && characterController.isGrounded)
            EndJump();

        if (!characterController.isGrounded)
            upSpeed += Gravity * Time.deltaTime;

        if (characterController.isGrounded && upSpeed < 0f)
            upSpeed = 0f;

        if (isSliding && slideSpeed > 0f)
            slideSpeed += Gravity * Time.deltaTime;

        if (isSliding && slideSpeed <= 0f)
            EndSlide();

        v0 = v;
    }

    private void EndHooking()
    {
        throw new NotImplementedException();
    }

    private void StartHooking()
    {
        throw new NotImplementedException();
    }

    void StartCrouch()
    {
        //Debug.Log("Start Crouch");

        isCrouching = true;

        transform.localScale = new Vector3(1f, 0.5f, 1f);
        torso.localScale = new Vector3(1f, 2f, 1f);
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);

        if (!isJumping && v0 >= SlideThreshold)
        {
            // Start Sliding
            StartSlide();
        }
        else if (!isJumping && !FOVChanged)
        {
            speed = CrouchSpeed;
        }
    }

    void EndCrouch()
    {
        //Debug.Log("End Crouch");

        if (isSliding)
            EndSlide();

        transform.localScale = new Vector3(1f, 1f, 1f);
        torso.localScale = new Vector3(1f, 1f, 1f);
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);

        isCrouching = false;
        speed = BaseSpeed;
    }

    void Jump()
    {
        //Debug.Log("Start Jump");
        isJumping = true;

        //vy = JumpForce;
        //Invoke("ResetJumpForce", JumpTime);

        upSpeed = JumpForce;
    }

    void ResetJumpForce()
    {
        vy = -JumpForce * 2f;
    }

    void EndJump()
    {
        //Debug.Log("End Jump");
        isJumping = false;
        //vy = 0f;
        upSpeed = 0f;
    }

    void StartSlide()
    {
        //Debug.Log("Start Sliding");
        isSliding = true;
        // slideDirection = velocity.normalized;
        // slideSpeed = SlideBoost + speed;

        deltaX += SlideBoost;
        //speedEffect.Play();
    }

    void EndSlide()
    {
        //Debug.Log("End Slide");

        if (isJumping)
            speed = BaseSpeed;
        else
        {
            if (isCrouching)
                speed = CrouchSpeed;
            else
                speed = BaseSpeed;

            isSliding = false;
            slideSpeed = 0f;
        }
    }
}