using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    public float minSpeed;
    public float maxSpeed;


    private float horizontal;
    private bool jumpPressedDown;
    private float speed = 8f;
    private float jumpingPower = 6.5f;
    private bool faceRight = true;
    private int boost = 0;
    // Start is called before the first frame update
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    void Start()
    {

    }
    private bool IsGrounded() 
    {
        if (Physics.OverlapSphere(groundCheck.position, 0.5f, groundLayer).Length != 0) boost = 64;
        return Physics.OverlapSphere(groundCheck.position, 0.5f, groundLayer).Length != 0;
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<float>();
        
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        /*
        if(context.started && IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
        */


        if(context.performed)
        {
            jumpPressedDown = true;
        }
        if(context.canceled)
        {
            jumpPressedDown = false;
        }
        
    }
    
    // Update is called once per frame
    void Update()
    {

        if (horizontal != 0f)
        {
            speed *= 1.3f;
        }
            //horizontal = Input.GetAxisRaw("Horizontal");

            /*
            if ((Input.GetKey(KeyCode.RightArrow) && horizontal > 0) || (Input.GetKey(KeyCode.LeftArrow) && horizontal < 0)) {
                speed *= 1.3f;
            }

            */
            
            if (jumpPressedDown && IsGrounded()) {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }


            if (jumpPressedDown && rb.velocity.y > 0f && boost > 0) 
            {
                if (boost <= 24 && boost % 4 == 0) rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 1.1f);
                boost--;

            }


            speed *= 0.9f;
        if (speed > 8f) {
            speed = 8f;
        } else if (speed < 6f) {
            speed = 6f;
        }
        flip();
        
    }
    private void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);

    }
    private void flip()
    {
        if(faceRight && horizontal < 0f || !faceRight && horizontal > 0f) {
            faceRight = !faceRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }
    
}

