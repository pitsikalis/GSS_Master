//This code can be used for private or commercial projects but cannot be sold or redestibuted without written permission.
//Copyright Nik W. Kraus / Dark Cube Entertainment LLC.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblerCollider : MonoBehaviour {

    [Tooltip("minimum magnitude for colliding object to activate crumbler script")]
    public float MinMagnitude = 1f;
    public bool UseTrigger = false;

    void OnTriggerEnter(Collider other)
    {
        if (UseTrigger)
        {
            if (other.gameObject.GetComponentInParent<CrumblerStatic>())
            {
                if (other.gameObject.GetComponentInParent<CrumblerStatic>().Activate == true)
                {
                    other.gameObject.GetComponentInParent<CrumblerStatic>().Activate = false;
                }
            }
        }
    }


    void OnCollisionEnter(Collision collision)
    {
        if (!UseTrigger)
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (contact.otherCollider.gameObject.GetComponentInParent<CrumblerStatic>())
                {
                    if (gameObject.GetComponent<Rigidbody>().velocity.magnitude >= MinMagnitude)
                    {
                        if (contact.otherCollider.gameObject.GetComponentInParent<CrumblerStatic>().Activate == true)
                        {
                            contact.otherCollider.gameObject.GetComponentInParent<CrumblerStatic>().Activate = false;
                        }
                    }
                }

                if (contact.otherCollider.gameObject.GetComponent<Rigidbody>())
                {
                    //contact.otherCollider.gameObject.GetComponent<Rigidbody>().AddForce(-contact.normal * 2f);
                }

                Debug.DrawRay(contact.point, contact.normal * 5, Color.red);
            }
        }
    }
}
