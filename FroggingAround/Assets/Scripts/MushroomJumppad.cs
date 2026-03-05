using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MushroomJumppad : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player" && collision.gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.AddForce(Vector3.up * 100f, ForceMode.Impulse);
        }
        else if (collision.gameObject.tag == "Player" && collision.transform.parent.gameObject.GetComponent<Rigidbody>() != null)
        {
            collision.transform.parent.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * 100f, ForceMode.Impulse);
        }
    }
}
