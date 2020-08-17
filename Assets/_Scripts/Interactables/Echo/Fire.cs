using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class Fire : MonoBehaviour
{
    public LayerMask layerMask;


    [SerializeField] private UnityEvent OnActivated;
    [SerializeField] private UnityEvent OnDeactivated;

 

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 12) // Shut down
        {
            OnDeactivated.Invoke();
        }

        if (other.gameObject.layer == 13) // Fire Grow
        {
            OnActivated.Invoke();
        }

    }
}
