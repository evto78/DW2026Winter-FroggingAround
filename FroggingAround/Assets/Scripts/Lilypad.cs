using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lilypad : MonoBehaviour
{
    Rigidbody rb;
    float initialY;
    private void Start()
    {
        initialY = transform.position.y;
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        //transform.localEulerAngles = new Vector3(-90, 0, transform.localEulerAngles.z);

        if (transform.position.y < initialY - 0.5f) { rb.AddForce(Vector3.up*5f*Time.deltaTime,ForceMode.Impulse); }
        if (transform.position.y > initialY + 0.5f) { rb.AddForce(-Vector3.up*5f*Time.deltaTime,ForceMode.Impulse); }

        if (transform.position.y < initialY - 1f) { rb.velocity = new Vector3(rb.velocity.x, 0.1f, rb.velocity.z); }
        if (transform.position.y > initialY + 1f) { rb.velocity = new Vector3(rb.velocity.x, -0.1f, rb.velocity.z); }
    }
}
