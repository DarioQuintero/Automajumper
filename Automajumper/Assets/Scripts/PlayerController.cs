using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : MonoBehaviour
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

    private float inputBufferTime = 0f;
    private float offGroundTime = 0f;
    private float coyoteTime = 0.1f;


    [SerializeField] private Rigidbody rb;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Transform hitBox;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator anim;

    [SerializeField] private GameObject bounds;

    private TextMeshProUGUI checkpointReached;

    private void Start()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        GameObject canvas = GameObject.Find("Canvas");
        checkpointReached = canvas.transform.Find("CPReached").gameObject.GetComponent<TextMeshProUGUI>();
        checkpointReached.gameObject.SetActive(false);

        bounds = GameObject.Find("Bounds");
    }

    public void jump() {
        // upward velocity
        rb.velocity = new Vector2(rb.velocity.x, jumpingPower);

        // fall slower 
        acceleratedFalling = false;

        // used the jump
        singleJumpUnused = false;
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
        // when jump key is first pressed
        if(context.started)
        {
            jumpPressedDown = true;

            // only can jump when landed in coyote time and didn't used the single jump
            if ((IsGrounded() || offGroundTime < coyoteTime) && singleJumpUnused)
            {
                offGroundTime = coyoteTime;
                jump();
            }
            
            // otherwise give player an input buffer time
            else
            {
                inputBufferTime = 0.1f;
            }
        }

        // when jump key is released
        else if (context.canceled)
        {
            jumpPressedDown = false;

            // experience accelerated falling if not pressing jumping 
            acceleratedFalling = true;

            // used to prevent holding the key to jump
            singleJumpUnused = true;
        }
    }

    public void OnChangeUpdateSpeed(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            MapManager.instance.SpeedUp();
        }
        if (context.canceled)
        {
            MapManager.instance.SlowDown();
        }
    }

    // Update is called once per frame
    void Update()
    {
        // decrease input buffer time and coyote time
        inputBufferTime -= Time.deltaTime;
        // if the jump key is being pressed
        if (jumpPressedDown)
        {
            // jump if on ground during input buffer time
            if (IsGrounded() && inputBufferTime >= 0)
            {
                jump();
            }
        }

        // increase off ground time
        offGroundTime += Time.deltaTime;
        if (IsGrounded() && singleJumpUnused)
        {
            // start off ground time when off ground and don't reset after jumped
            offGroundTime = 0f;
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
            checkpointReached.gameObject.SetActive(true);
            Invoke(nameof(hideCheckpointReached), 3f);
        }

        if (other.CompareTag("FinishLine") && IsGrounded())
        {
            // last minute change to destroy the bound
            Destroy(bounds);
            Destroy(other.gameObject);
            LevelManager.instance.LevelTransition();
        }
    }

    private void hideCheckpointReached()
    {
        checkpointReached.gameObject.SetActive(false);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Killer"))
        {
            transform.position = LevelCreator.instance.respawnPosition;
        }
    }
}

