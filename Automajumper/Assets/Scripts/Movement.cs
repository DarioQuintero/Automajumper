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


    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
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
        // only can jump when landed and didn't used the single jump
        if(context.started && IsGrounded() && singleJumpUnused)
        {
            acceleratedFalling = false;
            singleJumpUnused = false;
            jumpPressedDown = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
        else if(context.canceled)
        {
            // experience accelerated falling if not pressing jumping 
            acceleratedFalling = true;
            jumpPressedDown = false;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        // when grounded
        if (IsGrounded())
        {
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
        }

        // smoother jump George wrote; can't help to explain
        if (acceleratedFalling || rb.velocity.y < 1.3) {
            rb.velocity += (fallMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector3.up;
        } else {
            rb.velocity += (defaultMultiplier - 1) * Physics.gravity.y * Time.deltaTime * Vector3.up;
        }
        /*
        */
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

    //// landed when collide with a ground 
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if (LayerMask.LayerToName(collision.gameObject.layer) == "Ground")
    //        landed = true;
    //}

    void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Checkpoint") && IsGrounded())
        {
            LevelCreator.instance.respawnPosition = other.transform.position;
            Destroy(other.gameObject);
        }
    }

    // set respawn position when touch a check point
    //private void OnTriggerEnter(Collider other)
    //{
    //    Debug.Log(other);
    //    Debug.Log(other.gameObject.CompareTag("Checkpoint"));
    //    if (other.gameObject.CompareTag("Checkpoint") && IsGrounded())
    //    {
    //        LevelCreator.instance.respawnPosition = other.transform.position;
    //        Destroy(other.gameObject);
    //    }
    //}
}

