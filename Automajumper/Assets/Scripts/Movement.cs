using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    private bool acceleratedFalling;
    private float fallMultiplier = 7f;
    private float defaultMultiplier = 4f;
    private float horizontal;
    private bool jumpPressedDown;
    private float speed = 8f;
    private float jumpingPower = 19f;
    private bool faceRight = true;
    private bool singleJumpUnused;
    // Start is called before the first frame update
    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private bool IsGrounded() 
    {
        return Physics.OverlapSphere(groundCheck.position, 0.15f, groundLayer).Length != 0;
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

        // only can jump when grounded and didn't used the single jump
        if(context.started && IsGrounded() && singleJumpUnused)
        {
            jumpPressedDown = true;
            singleJumpUnused = false;
        }

        if(context.canceled)
        {
            jumpPressedDown = false;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // when grounded
        if (IsGrounded())
        {
            // jump if the key is pressed
            if (jumpPressedDown)
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
            }
            // if the jump key is released
            else
            {
                // you can jump again now
                singleJumpUnused = true;
            }

            // prevent hold jump key to jump
            if (!singleJumpUnused)
                jumpPressedDown = false;
        }

        Debug.Log(jumpPressedDown);

        // if not pressing jumping, then you experience accelerated falling
        if (!jumpPressedDown)
            acceleratedFalling = true;

        // smoother jump George wrote; can't help to explain
        if (acceleratedFalling || rb.velocity.y < 1.3) {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        } else {
            rb.velocity += Vector3.up * Physics.gravity.y * (defaultMultiplier - 1) * Time.deltaTime;
        }

        //flip();
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
    
    void OnTriggerStay(Collider other) {
        if (other.tag == "Checkpoint" && IsGrounded()) {
            LevelCreator.instance.respawnPosition = other.transform.position;
            Destroy(other.gameObject);
        }
    }
}

