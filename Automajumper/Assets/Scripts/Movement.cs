using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private float horizontal;
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
    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        if ((Input.GetKey(KeyCode.RightArrow) && horizontal > 0) || (Input.GetKey(KeyCode.LeftArrow) && horizontal < 0)) {
            speed *= 1.3f;
        }
        if (Input.GetKey(KeyCode.Space) && IsGrounded()) {
            rb.velocity = new Vector2(rb.velocity.x, jumpingPower);
        }
        if (Input.GetKey(KeyCode.Space) && rb.velocity.y > 0f && boost > 0) {
            if (boost <= 24 && boost % 4 == 0) rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * 1.1f);
            boost --;
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
