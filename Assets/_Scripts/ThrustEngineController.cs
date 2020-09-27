using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrustEngineController : MonoBehaviour
{
    
   // public float yAmount;
    private float rotationY;
    private float Speed = 7f;
    private float Thrust;

    private ThrustEngine myThrustEngine;

    // Update is called once per frame


    void Awake()
    {
        myThrustEngine = gameObject.GetComponent<ThrustEngine>();
    }


    void Update()
    {
        if (Input.GetAxis("Horizontal") != 0 )
        {
            lockedRotation();
        }

        if (Input.GetAxis("Vertical") != 0)
        {
            ForwardThrust();
        }

    }

    void lockedRotation()
    {
        rotationY += Input.GetAxis("Horizontal") * Speed ;
        rotationY = Mathf.Clamp(rotationY, -70, 70);

        transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, -rotationY , transform.localEulerAngles.z);
    }

    void ForwardThrust()
    {
        Thrust += Input.GetAxis("Vertical") * 100f;
        myThrustEngine.currentPowerPercentage = Thrust;
    }

}
