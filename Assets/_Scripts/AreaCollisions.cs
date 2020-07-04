using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaCollisions : MonoBehaviour
{
   

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log("Collision Enter");
    //}

    private void OnTriggerEnter2D(Collider2D collider)
    {
        Debug.Log("Trigger Enter");
    }
}
