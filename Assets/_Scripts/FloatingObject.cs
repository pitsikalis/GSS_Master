using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class FloatingObject : MonoBehaviour
{
    public float waterLevel = 0.0f;
    public float floatThreshold = 2.0f;
    public float WaterDensity = 0.125f;
    public float downForce = 4.9f;

    float forceFactor;
    Vector3 floatForce;

    public bool ActivateFloat;

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "WaterSurface")
        {
            ActivateFloat = true;
        }
    }


    void OnTriggerExit(Collider other)
    {
        if (other.tag == "WaterSurface")
        {
            ActivateFloat = false;
        }
    }


    void FixedUpdate()
    {
        if (ActivateFloat)
        {
            forceFactor = 1.0f - ((transform.position.y - waterLevel) / floatThreshold);

            if (forceFactor > 0.0f)
            {
                floatForce = -Physics.gravity * GetComponent<Rigidbody>().mass * (forceFactor - GetComponent<Rigidbody>().velocity.y * WaterDensity);
                floatForce += new Vector3(0f, -downForce * GetComponent<Rigidbody>().mass, 0f);
                GetComponent<Rigidbody>().AddForceAtPosition(floatForce, transform.position);
            }
        }
        
    }
}
