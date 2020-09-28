using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollaptionManager : MonoBehaviour
{
    public bool ActivatePhysics;
    public float CollapsePower = 2f;
    private Transform[] allCHren;
    private bool PlayedOnce;


    void Awake()
    {
        allCHren = GetComponentsInChildren<Transform>();

        KinematicRB();
    }

    private  void OnCollisionEnter(Collision other)
    {
        float TestVelocity = other.relativeVelocity.magnitude;
        Debug.Log(TestVelocity);
      
        if (other.gameObject.CompareTag("Attractable") && other.relativeVelocity.magnitude >= CollapsePower)
        {
            if (!PlayedOnce)
            {
                GravityRB();
                this.gameObject.GetComponent<BoxCollider>().enabled = false;
                PlayedOnce = true;
            }
          
        }
    }

    void KinematicRB()
    {
        foreach (Transform CH in allCHren)
        {
            if (CH.gameObject.GetComponent<Rigidbody>())
            {
                CH.gameObject.GetComponent<Rigidbody>().isKinematic = true;
                CH.gameObject.GetComponent<Rigidbody>().useGravity = false;
            }
        }

    }

    void GravityRB()
    {
        foreach (Transform CH in allCHren)
        {
            if (CH.gameObject.GetComponent<Rigidbody>())
            {
                CH.gameObject.GetComponent<Rigidbody>().isKinematic = false;
                CH.gameObject.GetComponent<Rigidbody>().useGravity = true;
            }
        }

    }
}
