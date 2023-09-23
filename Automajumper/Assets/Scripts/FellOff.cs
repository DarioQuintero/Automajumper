using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FellOff : MonoBehaviour
{
    public float fallMultiplier = 2.5f;
    public float defaultMultiplier = 2f;

    Rigidbody rb;
    // Start is called before the first frame update
    void Awake() {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.velocity.y < 0) {
            rb.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.deltaTime;
        }
    }
}
