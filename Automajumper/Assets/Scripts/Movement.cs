using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Movement : MonoBehaviour
{
    private float fallMultiplier = 7f;
    private float defaultMultiplier = 4f;
    private float horizontal;
    private float speed = 8f;
    private float jumpingPower = 19f;
    public bool faceRight = true;

    private bool jumpPressedDown;
    private bool singleJumpUnused;
    private bool acceleratedFalling;

    private float coyoteTime = 0f;
    private bool isCoyote = true;


    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform hitBox;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }
    public void jump() {
        if (singleJumpUnused) {
            acceleratedFalling = false;
            singleJumpUnused = false;
            jumpPressedDown = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
        // if (!jumpPressedDown) {
        //     acceleratedFalling = true;
        // }
    }

    private bool IsGrounded()
    {
        bool result;
        result = Physics.OverlapSphere(groundCheck.position, 0.15f, groundLayer).Length != 0;
        anim.SetBool("Grounded", result);
        return result;
        
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        horizontal = context.ReadValue<float>();
        anim.SetFloat("VelocityX", horizontal);
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (coyoteTime <= -0.09) coyoteTime = 0.5f;
        // only can jump when landed and didn't used the single jump
        //singleJumpUnused && 
        if(context.started)
        {
            if (IsGrounded()) {
                coyoteTime = -0.1f;
            }
        }
        else if(context.canceled)
        {
            // experience accelerated falling if not pressing jumping 
            coyoteTime = -0.1f;
            acceleratedFalling = true;
            jumpPressedDown = false;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        coyoteTime = Math.Max(-0.1f, coyoteTime - Time.deltaTime);
        if (coyoteTime >= 0f) {
            isCoyote = true;
        } else {
            isCoyote = false;
        }
        // when grounded
        if (IsGrounded())
        {
            if (isCoyote) {
                jump();
                coyoteTime = -0.1f;
            }
            // if the jump key is released
            if (!jumpPressedDown)
            {
                // you can jump again now
                singleJumpUnused = true;
            }

            // prevent hold jump key to jump
            if (!singleJumpUnused)
            {
                jumpPressedDown = false;
            }
            
            if (Physics.OverlapSphere(hitBox.position, 0.02f, groundLayer).Length != 0) {
                transform.position = LevelCreator.instance.respawnPosition;
            }
            
        }

        // smoother jump George wrote; can't help to explain
        if (acceleratedFalling || rb.velocity.y < 1.3) {
            rb.velocity += (fallMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector3.up;
        } else {
            rb.velocity += (defaultMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector3.up;
        }
    }

    private void FixedUpdate()
    {
        //Animation Direction Changing
        if(horizontal>0.01 && transform.eulerAngles.y != -90f)
        {
            transform.eulerAngles = new Vector3(0f, -90f, 0f);
            
            //anim.SetTrigger("Spin");
        }
        if(horizontal < -0.01 && transform.eulerAngles.y != 90f)
        {
            transform.eulerAngles = new Vector3(0f, 90f, 0f);

            //anim.SetTrigger("Spin");
        }
        rb.velocity = new Vector2(horizontal * speed, rb.velocity.y);
        anim.SetFloat("VelocityY", rb.velocity.y);
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

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Checkpoint") && IsGrounded())
        {
            LevelCreator.instance.respawnPosition = other.transform.position;
            Destroy(other.gameObject);
        }

        if (other.CompareTag("FinishLine") && IsGrounded())
        {
            LevelManager.instance.levelTransition();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Killer"))
        {
            transform.position = LevelCreator.instance.respawnPosition;
        }
    }
}

