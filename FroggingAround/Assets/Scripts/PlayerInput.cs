using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerInput : MonoBehaviour
{
    [Header("References")]
    public Transform camHolder;
    public AudioManager audioMan;
    PlayerMovement mvt;
    Rigidbody rb;
    Rigidbody attachedRb;
    public Animator mouthAnim;

    [Header("UI")]
    public Image tongueUI;
    public Image tongueUIDist;
    public TextMeshProUGUI fliesGotText;
    public TextMeshProUGUI fliesCountText;
    public Image flyTrophy;
    public Transform flyCounterUI;
    public float fliesGot;
    public float fliesCount;

    [Header("Tongue Management")]
    public LineRenderer tongueLR;
    public Transform attachPoint;
    public Transform lookPoint;
    Transform lookAtObject;
    public Transform lockPoint;
    public AnimationCurve pointSagCurve;

    public Gradient tenseGradient;
    public Gradient idleGradient;
    public Gradient looseGradient;
    public Gradient actingGradient;

    [Header("Stats")]
    public float maxReach;
    public float extendSpeed;
    public AnimationCurve extendSpeedCurve;
    float extendTimer;
    public float pullStrength;
    public float scrollStrength;

    //Trackers
    float startDist;
    bool avaliablePoint;
    bool tongueOut;
    bool tongueAttached;
    bool retracting;
    bool lookingAtFly;
    void Start()
    {
        mvt = GetComponent<PlayerMovement>();
        audioMan = GetComponent<AudioManager>();
        rb = mvt.rb;

        avaliablePoint = false;
        tongueOut = false;
        tongueAttached = false;
        retracting = false;
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
        else if (Input.GetKeyUp(KeyCode.Mouse0)) { retracting = true;}

        float scrollData = Input.mouseScrollDelta.y;
        if (Input.GetKey(KeyCode.Mouse1)) { scrollData -= 2f; }

        if (tongueAttached)
        {
            startDist += scrollData * scrollStrength * Time.deltaTime;
            startDist = Mathf.Clamp(startDist, 0, maxReach);
        }
        
    }
    void ExtendTongue()
    {
        if (retracting) { return; }
        if (avaliablePoint) 
        {
            audioMan.PlaySoundByKey(0);

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
        attachPoint.GetComponentInChildren<MeshRenderer>().enabled = false;

        if (tongueOut && !retracting)
        {
            attachPoint.GetComponentInChildren<MeshRenderer>().enabled = true;

            if (extendTimer <= 1) { extendTimer += Time.deltaTime * extendSpeed; }
            if (extendTimer > 1) { extendTimer = 1f; if (!tongueAttached) { startDist = Vector3.Distance(transform.position, attachPoint.position); } tongueAttached = true;  }
            attachPoint.position = Vector3.Lerp(transform.position, lockPoint.position, extendSpeedCurve.Evaluate(extendTimer));

            if (Vector3.Distance(transform.position, attachPoint.position) > maxReach + 2f) 
            { retracting = true; lockPoint.parent = transform; tongueAttached = false; audioMan.PlaySoundByKey(1); }
        }
        else if (tongueOut && retracting)
        {
            attachPoint.GetComponentInChildren<MeshRenderer>().enabled = true;

            extendTimer -= Time.deltaTime * extendSpeed; if (extendTimer < 0) { extendTimer = 0; retracting = false; tongueOut = false; startDist = 0; }
            attachPoint.position = Vector3.Lerp(transform.position, lockPoint.position, extendSpeedCurve.Evaluate(extendTimer));
        }

        //if (!tongueOut) { startDist = 0; }

        if (tongueAttached && lockPoint.parent.TryGetComponent<FlyScript>(out FlyScript fScript))
        {
            fScript.Hit();
        }

        if (retracting && extendTimer <= 0) { extendTimer = 0; retracting = false; tongueOut = false; attachPoint.position = transform.position; }

        TonguePhysics();
    }
    void TonguePhysics()
    {
        if (tongueAttached)
        {
            float curDist = Vector3.Distance(transform.position, attachPoint.position);
            if (curDist < startDist - 2) 
            { 
                //startDist -= 0.5f; 
            }
            else if (curDist > startDist) 
            { 
                float difference = curDist - startDist;

                if (attachedRb == null) //Attached obj is static
                {
                    rb.AddForce((attachPoint.position - transform.position).normalized * (difference / 2f) * pullStrength / 1f);
                }
                else if (rb.mass > attachedRb.mass) //Attached obj is lighter
                {
                    rb.AddForce((attachPoint.position - transform.position).normalized * (difference / 2f) * pullStrength / 1f);
                    attachedRb.AddForceAtPosition((transform.position - attachPoint.position).normalized * (difference / 2f) * pullStrength, attachPoint.position);
                }
                else if (rb.mass <= attachedRb.mass) //Attached obj is heavier
                {
                    rb.AddForce((attachPoint.position - transform.position).normalized * (difference / 2f) * pullStrength / 1f);
                    attachedRb.AddForceAtPosition((transform.position - attachPoint.position).normalized * (difference / 2f) * pullStrength, attachPoint.position);
                }
            }
        }
    }
    void Effects()
    {
        tongueLR.positionCount = 10;
        float curDist = Vector3.Distance(transform.position, attachPoint.position);
        for (int i = 0; i < tongueLR.positionCount; i++)
        {
            Vector3 verticalOffset = Vector3.zero;
            Vector3 horizontalOffset = Vector3.zero;
            if (tongueAttached)
            {
                //verticalOffset = -1 * ((Mathf.Clamp(startDist - curDist, 0, 20)/20f) * (Vector3.up * pointSagCurve.Evaluate((float)i / ((float)tongueLR.positionCount - 1))));
                horizontalOffset = transform.right * 2 * Random.Range(-1f, 1f) * (Mathf.Clamp(curDist - startDist, 0, 10)/8f) * (Time.deltaTime);
            }
            tongueLR.SetPosition(i, Vector3.Lerp(transform.position, attachPoint.position, (float)i/((float)tongueLR.positionCount-1f)) + horizontalOffset + verticalOffset);
        }
        tongueLR.enabled = tongueOut;

        tongueUI.fillAmount = startDist / maxReach;
        tongueUIDist.fillAmount = curDist / maxReach;

        fliesGotText.text = fliesGot.ToString();
        fliesCountText.text = fliesCount.ToString();

        flyTrophy.gameObject.SetActive(fliesGot >= fliesCount);

        GradientColorKey[] newColors = new GradientColorKey[2];
        newColors[0] = idleGradient.colorKeys[0];
        newColors[1] = idleGradient.colorKeys[1];
        if (curDist > maxReach)
        {
            newColors[0].color = Color.Lerp(idleGradient.colorKeys[0].color, tenseGradient.colorKeys[0].color, curDist / (maxReach+2f));
            newColors[1].color = Color.Lerp(idleGradient.colorKeys[1].color, tenseGradient.colorKeys[1].color, curDist / (maxReach+2f));
        }
        else if (curDist < startDist)
        {
            newColors[0].color = Color.Lerp(idleGradient.colorKeys[0].color, looseGradient.colorKeys[0].color, Mathf.Abs(curDist - startDist) / maxReach);
            newColors[1].color = Color.Lerp(idleGradient.colorKeys[1].color, looseGradient.colorKeys[1].color, Mathf.Abs(curDist - startDist) / maxReach);
        }
        else if (curDist > startDist)
        {
            newColors[0].color = Color.Lerp(idleGradient.colorKeys[0].color, tenseGradient.colorKeys[0].color, Mathf.Abs(curDist - startDist) / maxReach);
            newColors[1].color = Color.Lerp(idleGradient.colorKeys[1].color, tenseGradient.colorKeys[1].color, Mathf.Abs(curDist - startDist) / maxReach);
        }
        actingGradient.SetKeys(newColors, idleGradient.alphaKeys);
        tongueLR.colorGradient = actingGradient;

        attachPoint.GetComponentInChildren<MeshRenderer>().material.color = newColors[1].color;

        if (!passiveInProgress) { StartCoroutine(PassiveFrogNoises()); }
    }
    bool passiveInProgress;
    IEnumerator PassiveFrogNoises()
    {
        passiveInProgress = true;
        if (audioMan.soundEffects[2].source.isPlaying) { yield return new WaitForSeconds(Random.Range(1f, 2f)); }
        audioMan.PlaySoundByKey(2);
        yield return new WaitForSeconds(Random.Range(5f,10f));
        passiveInProgress = false;
        yield return null;
    }
    IEnumerator FlyPickupFeedback()
    {
        audioMan.PlaySoundByKey(2);
        mouthAnim.SetTrigger("Eat");

        float timer = 0.5f;
        if (fliesGot >= fliesCount) { timer = 1f; }
        while(timer > 0)
        {
            flyCounterUI.localScale = Vector3.one * (1 + timer);
            timer -= Time.deltaTime/2f;
            yield return new WaitForEndOfFrame();
        }
        flyCounterUI.localScale = Vector3.one;

        yield return null;
    }
    void Look()
    {
        Ray ray = new Ray(camHolder.position, camHolder.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, maxReach))
        {
            avaliablePoint = true;
            lookPoint.transform.position = hit.point;
            lookAtObject = hit.transform;

            lookingAtFly = hit.transform.gameObject.tag == "Fly";
        }
        else
        {
            lookingAtFly = false;
            avaliablePoint = false;
            //lookPoint.transform.position = camHolder.position + camHolder.forward * (maxReach - 2f);
            lookPoint.transform.position = camHolder.position - camHolder.forward;
        }
    }
    public void FlyCollected()
    {
        lockPoint.transform.parent = transform;
        //Debug.Log("Fly collected");
        fliesGot++;
        retracting = true;

        StopCoroutine(FlyPickupFeedback());
        StartCoroutine(FlyPickupFeedback());
    }
}
