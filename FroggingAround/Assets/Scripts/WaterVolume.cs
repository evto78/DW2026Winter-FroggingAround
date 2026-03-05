using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterVolume : MonoBehaviour
{
    public float boyncy;
    private void OnTriggerStay(Collider other)
    {
        float objDepth = Mathf.Abs(transform.position.y - other.gameObject.transform.position.y);
        if (other.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            if(other.gameObject.tag == "Fly")
            {
                rb.AddForce(Vector3.up * (objDepth * 1.5f) * (boyncy / 4f) * Time.deltaTime);
            }
            else
            {
                rb.AddForce(Vector3.up * (objDepth * 3f) * boyncy * Time.deltaTime);
            }
        }
        else if (other.gameObject.GetComponentInParent<Rigidbody>() != null)
        {
            if (other.gameObject.tag == "Fly")
            {
                other.gameObject.GetComponentInParent<Rigidbody>().AddForce(Vector3.up * (objDepth * 1.5f) * (boyncy / 4f) * Time.deltaTime);
            }
            else
            {
                other.gameObject.GetComponentInParent<Rigidbody>().AddForce(Vector3.up * (objDepth * 3f) * boyncy * Time.deltaTime);
            }
        }
    }
}
