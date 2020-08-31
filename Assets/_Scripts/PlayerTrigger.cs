using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerTrigger : MonoBehaviour
{
    public UnityEvent PlayerEnter;
    public UnityEvent PlayerExit;

    [SerializeField]
    protected bool enterDelay;
    [SerializeField] 
    protected float waitForSecondsOnEnter;
    [SerializeField] 
    protected bool exitDelay;
    [SerializeField] 
 
    protected float waitForSecondsOnExit;
    
    private float? triggerPassedAtTime;
    private Func<float, bool> enoughTimeHasElapsed;

     private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (exitDelay && (triggerPassedAtTime == null || enoughTimeHasElapsed(waitForSecondsOnExit)))
            {
                OnPlayerEnter();
            }
        }
       
    
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            if (exitDelay)
            {
                triggerPassedAtTime = Time.time;
            }

        }
        else
        {
            OnPlayerExit();
        }
           
    }

  

    public void OnPlayerEnter()
    {
        triggerPassedAtTime = Time.time;

        if (enterDelay)
        {
            
            StartCoroutine(StartEnterCount());

        }
        else
        {
            PlayerEnter.Invoke();
        }

    }

    IEnumerator StartEnterCount()
    {
        
        yield return new WaitForSeconds(waitForSecondsOnEnter);

        PlayerEnter.Invoke();

    }

    private void OnPlayerExit()
    {
        if (exitDelay)
        {
            triggerPassedAtTime = null;

            StartCoroutine(StartExitCount());

        }
        else
        {
            PlayerExit.Invoke();
        }
        
       
    }

    IEnumerator StartExitCount()
    {

        yield return new WaitForSeconds(waitForSecondsOnExit);

        PlayerExit.Invoke();

    }


    private void Update()
    {
        if (exitDelay && enoughTimeHasElapsed(waitForSecondsOnExit))
        {
            OnPlayerExit();
        }
           
    }
}