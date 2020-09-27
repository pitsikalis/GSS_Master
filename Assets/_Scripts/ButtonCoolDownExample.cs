using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonCoolDownExample : MonoBehaviour
{
    public float CoolDownTime;
    private float coolDownUntilNextPress;

    // Update is called once per frame
    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Space) && coolDownUntilNextPress < Time.time)
        {
            coolDownUntilNextPress = Time.time + CoolDownTime;

            Debug.Log(" Button Pushed ! ");
        }
      
    }
}
