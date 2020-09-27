using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObjectOnCollision : MonoBehaviour
{
    public Transform EndPoint;
    public float speed;

    void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Rigidbody>())
        {
            Vector3 position = Vector3.MoveTowards(other.transform.position, EndPoint.position, speed * Time.fixedDeltaTime);

            other.GetComponent<Rigidbody>().MovePosition(position);
        }
    }
}
