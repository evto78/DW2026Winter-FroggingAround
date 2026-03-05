using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyCagePuzzle : MonoBehaviour
{
    public Animator anim;
    public int requiredObjects;
    public int currentObjects;
    public List<MeshRenderer> lights;
    public Material offMat;
    public Material onMat;
    void Start()
    {
        currentObjects = 0;
        anim.SetBool("Open", false);
    }
    private void Update()
    {
        for(int i = 0; i < lights.Count; i++)
        {
            if(currentObjects >= i) { lights[i].material = onMat; }
            else { lights[i].material = offMat; }
        }

        anim.SetBool("Open", currentObjects >= requiredObjects);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Branch") { currentObjects++; }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Branch") { currentObjects--; }
    }
}
