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

    public float wingDir = 1;

    Vector3 startPoint;
    Vector3 randOffset;
    Vector3 randFlyOffset;
    float randFlySpeedMod;

    bool isHit = false;

    public float flapProgress;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;

        startPoint = transform.position;
        randOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        randFlyOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f));
        randFlySpeedMod = Random.Range(1f, 2f);

        GameObject.Find("Player").GetComponent<PlayerInput>().fliesCount++;
    }
    public void Hit()
    {
        if (isHit) { return; }
        rb.useGravity = true;
        isHit = true;
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
        if (isHit)
        {
            leftWing.localEulerAngles += Vector3.forward * wingDir * wingSpeed * Time.deltaTime;
            rightWing.localEulerAngles -= Vector3.forward * wingDir * wingSpeed * Time.deltaTime;
            wingDir = Random.Range(-5f, 5f);
        }
        else
        {
            leftWing.localEulerAngles += Vector3.forward * wingSpeed * wingDir * Time.deltaTime;
            rightWing.localEulerAngles -= Vector3.forward * wingSpeed * wingDir * Time.deltaTime;
            flapProgress += wingSpeed * wingDir * Time.deltaTime;
            if(wingDir > 0 && flapProgress >= 50f) { wingDir = -1; }
            else if (wingDir < 0 && flapProgress <= -20f) { wingDir = 1; }

            if (rb.velocity.magnitude != 0) { transform.rotation = Quaternion.LookRotation(rb.velocity); }
            
            if (Vector3.Distance(transform.position, startPoint + randOffset) < 0.5f) { randFlyOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), Random.Range(-1f, 1f)); }
            rb.AddForce((((startPoint + randOffset) - transform.position) / 2f + randFlyOffset / 1.2f) * flySpeed * randFlySpeedMod * Time.deltaTime);
        }
    }
}
