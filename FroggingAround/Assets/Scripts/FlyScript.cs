using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyScript : MonoBehaviour
{
    Rigidbody rb;
    public Transform leftWing;
    public Transform rightWing;
    public float wingSpeed;
    public float flySpeed;

    float wingDir = 1;

    Vector3 startPoint;
    Vector3 randOffset;
    Vector3 randFlyOffset;
    float randFlySpeedMod;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        startPoint = transform.position;
        randOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        randFlyOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        randFlySpeedMod = Random.Range(1f, 2f);
    }
    public void Hit()
    {
        rb.useGravity = true;
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.TryGetComponent<PlayerInput>(out PlayerInput pi))
        {
            pi.FlyCollected();
            Destroy(gameObject);
        }
    }
    void Update()
    {
        leftWing.localEulerAngles -= Vector3.forward * wingSpeed * Time.deltaTime;
        rightWing.localEulerAngles += Vector3.forward * wingSpeed * Time.deltaTime;
        if (leftWing.localEulerAngles.z < -5 && wingDir == 1) { wingDir = -1; }
        if (leftWing.localEulerAngles.z > 5 && wingDir == -1) { wingDir = 1; }

        transform.rotation = Quaternion.LookRotation(rb.velocity);
        if (Vector3.Distance(transform.position, startPoint+randOffset) < 0.5f) { randFlyOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)); }
        rb.AddForce((((startPoint + randOffset) - transform.position)/2f + randFlyOffset/1.2f) * flySpeed * randFlySpeedMod * Time.deltaTime);
    }
}
