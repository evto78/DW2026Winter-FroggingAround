using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Transform camHolder;

    public LineRenderer tongueLR;
    public Transform attachPoint;
    public Transform lookPoint;
    Transform lookAtObject;
    public Transform lockPoint;

    PlayerMovement mvt;
    Rigidbody rb;
    Rigidbody attachedRb;

    public float maxReach;
    public float extendSpeed;
    public AnimationCurve extendSpeedCurve;
    float extendTimer;
    public float pullStrength;

    float startDist;
    public bool avaliablePoint;
    public bool tongueOut;
    public bool tongueAttached;
    public bool retracting;
    void Start()
    {
        mvt = GetComponent<PlayerMovement>();
        rb = mvt.rb;

        avaliablePoint = false;
        tongueOut = false;
        tongueAttached = false;
        retracting = false;

        tongueLR.positionCount = 2;
        tongueLR.SetPosition(0, transform.position);
        tongueLR.SetPosition(1, transform.position);
    }
    void Update()
    {
        Look();
        GetInputs();

        ManageTongue();
        Effects();
    }
    void GetInputs()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0)) { ExtendTongue(); }
        if (Input.GetKeyUp(KeyCode.Mouse0)) { retracting = true; }
    }
    void ExtendTongue()
    {
        if (retracting) { return; }
        if (avaliablePoint) 
        {
            lockPoint.position = lookPoint.position;
            lockPoint.parent = lookAtObject;
            if (lookAtObject.TryGetComponent<Rigidbody>(out Rigidbody tempRb))
            { attachedRb = tempRb; }
            else if (lookAtObject.GetComponentInParent<Rigidbody>() != null)
            { attachedRb = lookAtObject.GetComponentInParent<Rigidbody>(); }
            else { attachedRb = null; }
            tongueOut = true;
            tongueAttached = false;
            extendTimer = 0f;
        }
    }
    void ManageTongue()
    {
        if (tongueOut && !retracting)
        {
            extendTimer += Time.deltaTime * extendSpeed; if (extendTimer > 1) { extendTimer = 1; tongueAttached = true; }
            attachPoint.position = Vector3.Lerp(transform.position, lockPoint.position, extendSpeedCurve.Evaluate(extendTimer));

            if (Vector3.Distance(transform.position, attachPoint.position) > maxReach + 2f) 
            { retracting = true; lockPoint.parent = transform; tongueAttached = false; startDist = Vector3.Distance(transform.position, attachPoint.position); }
        }
        else if (tongueOut && retracting)
        {
            extendTimer -= Time.deltaTime * extendSpeed; if (extendTimer < 0) { extendTimer = 0; retracting = false; tongueOut = false; }
            attachPoint.position = Vector3.Lerp(transform.position, lockPoint.position, extendSpeedCurve.Evaluate(extendTimer));
        }

        if (retracting && extendTimer <= 0) { extendTimer = 0; retracting = false; tongueOut = false; attachPoint.position = transform.position; }

        TonguePhysics();
    }
    void TonguePhysics()
    {
        if (tongueAttached)
        {
            float curDist = Vector3.Distance(transform.position, attachPoint.position);
            if (curDist < startDist - 2) { startDist -= 0.5f; }
            else if (curDist > startDist) 
            { 
                float difference = curDist - startDist;

                if (attachedRb == null) //Attached obj is static
                {
                    rb.AddForce((attachPoint.position - transform.position).normalized * (difference / 2f) * pullStrength);
                }
                else if (rb.mass > attachedRb.mass) //Attached obj is lighter
                {
                    rb.AddForce((attachPoint.position - transform.position).normalized * (difference / 2f) * pullStrength);
                    attachedRb.AddForceAtPosition((transform.position - attachPoint.position).normalized * (difference / 2f) * pullStrength, attachPoint.position);
                }
                else if (rb.mass <= attachedRb.mass) //Attached obj is heavier
                {
                    rb.AddForce((attachPoint.position - transform.position).normalized * (difference / 2f) * pullStrength);
                    attachedRb.AddForceAtPosition((transform.position - attachPoint.position).normalized * (difference / 2f) * pullStrength, attachPoint.position);
                }
            }
        }
    }
    void Effects()
    {
        tongueLR.SetPosition(0, transform.position);
        tongueLR.SetPosition(1, attachPoint.transform.position);
        tongueLR.enabled = tongueOut;
    }
    void Look()
    {
        Ray ray = new Ray(camHolder.position, camHolder.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxReach))
        {
            avaliablePoint = true;
            lookPoint.transform.position = hit.point;
            lookAtObject = hit.transform;
        }
        else
        {
            avaliablePoint = false;
            lookPoint.transform.position = camHolder.position + camHolder.forward * (maxReach - 2f);
        }
    }
}
