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
                rb.AddForce(Vector3.up * (objDepth*1.5f) * (boyncy / 4f) * Time.deltaTime );
            }
            else
            {
                rb.AddForce(Vector3.up * (objDepth*2f) * boyncy * Time.deltaTime);
            }
            if(rb.velocity.y > 3f && other.gameObject.tag != "Player") { rb.velocity = new Vector3(rb.velocity.x, 3f, rb.velocity.z); }
        }
        else if (other.gameObject.GetComponentInParent<Rigidbody>() != null)
        {

            if (other.gameObject.tag == "Fly")
            {
                other.gameObject.GetComponentInParent<Rigidbody>().AddForce(Vector3.up * (objDepth*1.5f) * (boyncy / 4f) * Time.deltaTime);
            }
            else
            {
                other.gameObject.GetComponentInParent<Rigidbody>().AddForce(Vector3.up * (objDepth*2f) * boyncy * Time.deltaTime);
            }
            if (other.gameObject.GetComponentInParent<Rigidbody>().velocity.y > 3f && other.gameObject.tag != "Player") 
            { other.gameObject.GetComponentInParent<Rigidbody>().velocity = new Vector3(other.gameObject.GetComponentInParent<Rigidbody>().velocity.x, 3f, other.gameObject.GetComponentInParent<Rigidbody>().velocity.z); }
        }
    }
}
