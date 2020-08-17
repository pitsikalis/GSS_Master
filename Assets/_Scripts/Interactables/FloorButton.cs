using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using FMODUnity;

public class FloorButton : MonoBehaviour
{
    [SerializeField] private string animatorParameter;
    [SerializeField] private Animator animator;
    [SerializeField] private float WaitTime;
    
    
 

    [SerializeField] private StudioEventEmitter ActivateSound;
    [SerializeField] private StudioEventEmitter DeactivateSound;

   
    private float _depressedTime;

   
    private bool IsDepressed;

    [SerializeField] private UnityEvent OnActivated;
    [SerializeField] private UnityEvent OnDeactivated;

    //private void OnEnable()
    //{
    //}

    //private void OnDisable()
    //{
    
    //}

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger || other.attachedRigidbody == null)
        {
            return;
        }
        if (other.tag == "Player")
        {
            animator.SetBool(animatorParameter, true);
            OnActivated.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            animator.SetBool(animatorParameter, false);
            OnDeactivated.Invoke();

                if (DeactivateSound != null)
                {
                    DeactivateSound.Play();
                } 

                IsDepressed = true;
        }
    }
    
    private void Update()
    {
        if (!IsDepressed)
        {
            _depressedTime = 0f;

            return;
        }

        if (_depressedTime >= WaitTime)
        {
            
           

            if (ActivateSound != null)
            {
                ActivateSound.Play();
            }
            IsDepressed = true;
        }

        _depressedTime += Time.deltaTime;
    }
}
