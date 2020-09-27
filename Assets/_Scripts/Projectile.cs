
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class Projectile : MonoBehaviour, IPooledObject
{

    public float upForce = 1f;
    public float sideForce = 0.1f;

    public void OnObjectSpawn()
    {
        float xForce = Random.Range(-sideForce, sideForce);
        float yForce = Random.Range(upForce / 2f, upForce);
        float zForce = Random.Range(-sideForce, sideForce);

        Vector3 force = new Vector3(xForce, yForce, zForce);

         force = GetComponent<Rigidbody>().velocity ;

    }
}
