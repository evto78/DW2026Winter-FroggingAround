using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableObject : MonoBehaviour
{
    public MeshRenderer outline;
    public Rigidbody rb;
    bool hoveredLastFrame;
    public bool pickupable;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        outline.enabled = false;
    }
    void Update()
    {
        outline.enabled = hoveredLastFrame;
        hoveredLastFrame = false;
    }
    public void HoverOver()
    {
        hoveredLastFrame = true;
    }
    public void Interact(PlayerInput sender)
    {
        Debug.Log("Interact!");
    }
}
