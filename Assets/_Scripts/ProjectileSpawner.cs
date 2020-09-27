using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileSpawner : MonoBehaviour
{
    public string ObjectType;
    ObjectPooler objectPooler;

    private void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }
   
    void FixedUpdate()
    {
        ObjectPooler.Instance.SpawnFromPool(ObjectType, transform.position, Quaternion.identity);
    }
}
