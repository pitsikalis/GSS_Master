using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FireManager : MonoBehaviour
{
    public float FireHealth = 100f ;
    private WaterManager waterManager;
    public float WaterDamage = 10f;
    public UnityEvent OnExtinction;


    // Update is called once per frame
    void Update()
    {
        if (FireHealth <= 0)
        {
            OnExtinction.Invoke();
            FireHealth = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.tag == "Water"))
        {  
            FireHealth -= WaterDamage;
        }
    }
}
