using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BombTrigger : MonoBehaviour
{
 

    public bool PlayOnAwake;
  
    [HideInInspector]
    public bool BombIsActive;
    
    public bool CounterIsActive;
 
    public float CounterTime;
    [Space()]
    [SerializeField]
    private UnityEvent OnActivation;
    [Space()]
    [SerializeField]
    private UnityEvent OnExplosion;


    void Start()
    {
        if(PlayOnAwake)
        {
            BombIsActive = true; 
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (BombIsActive)
        {
            StartCountDown();
            BombIsActive = false;
        }
    }

    private void StartCountDown()
    {
        if (CounterIsActive)
        {
            StartCoroutine(StartCount());
        }
        else
        {
            Explode();
        }

        OnActivation.Invoke();

    }

    private void Explode()
    {
        OnExplosion.Invoke();       
    }

    IEnumerator StartCount()
    {

        yield return new WaitForSeconds(CounterTime);

        OnExplosion.Invoke();
        
    }
}
