using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterVolume : MonoBehaviour
{
    public float boyency;
    private void OnTriggerStay(Collider other)
    {
        float objDepth = Mathf.Abs(transform.position.y - other.gameObject.transform.position.y);
        if (other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.AddForce(Vector3.up * (objDepth * 3f) * boyency * Time.deltaTime);
        }
        else if (other.gameObject.GetComponentInParent<Rigidbody>() != null)
        {
            other.gameObject.GetComponentInParent<Rigidbody>().AddForce(Vector3.up * (objDepth * 3f) * boyency * Time.deltaTime);
        }
    }
}
