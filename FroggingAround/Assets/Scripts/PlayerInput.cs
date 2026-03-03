using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public Transform camHolder;
    public float interactDistance;

    public GameObject heldObject;

    void Start()
    {
        heldObject = null;
    }
    void Update()
    {
        if(heldObject != null)
        {

            heldObject.transform.position = Vector3.Lerp(heldObject.transform.position, (camHolder.position - camHolder.up) + camHolder.forward * 3f, Time.deltaTime * 20f);

            if (Input.GetKeyDown(KeyCode.E)) 
            {
                InteractableObject tScript = heldObject.GetComponent<InteractableObject>();
                tScript.rb.useGravity = true;
                tScript.rb.AddForce(camHolder.forward * 25f + camHolder.up * 5f, ForceMode.Impulse);
                heldObject = null;
            }
        }
        else
        {
            Ray ray = new Ray(camHolder.position, camHolder.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance) && hit.collider.gameObject.CompareTag("Interactable"))
            {
                GameObject tarObject = hit.collider.gameObject;
                InteractableObject interactScript = tarObject.GetComponent<InteractableObject>();

                interactScript.HoverOver();

                if (Input.GetKeyDown(KeyCode.E))
                {
                    interactScript.Interact(this);
                    if (interactScript.pickupable && heldObject == null)
                    {
                        interactScript.rb.useGravity = false;
                        heldObject = tarObject;
                    }
                }
            }
        }
    }
}
