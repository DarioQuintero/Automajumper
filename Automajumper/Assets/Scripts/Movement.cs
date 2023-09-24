using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    private bool falling;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float defaultMultiplier = 2f;
    private float horizontal;
    private bool jumpPressedDown;
    private float speed = 8f;
    [SerializeField] float jumpingPower = 6.5f;
    private bool faceRight = true;
    // Start is called before the first frame update
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private bool IsGrounded() 
    {
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
            //horizontal = Input.GetAxisRaw("Horizontal");

            /*
            if ((Input.GetKey(KeyCode.RightArrow) && horizontal > 0) || (Input.GetKey(KeyCode.LeftArrow) && horizontal < 0)) {
                speed *= 1.3f;
            }

            */
        if (IsGrounded()) {
            falling = false;
            if (jumpPressedDown) rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
            
        if (falling || rb.velocity.y < 1.3) {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else {
            rb.velocity += Vector3.up * Physics.gravity.y * (defaultMultiplier - 1) * Time.deltaTime;
        }
        if (!jumpPressedDown) {
            falling = true;
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

