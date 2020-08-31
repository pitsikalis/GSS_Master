using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AnimatorSimpleEvent : MonoBehaviour
{
    public bool WaitTimeActive01;
    public float WaitTime01;
    public bool WaitTimeActive02;
    public float WaitTime02;

    [Space(10)] // 10 pixels of spacing here.
    private bool ElapsedTime01;
    private bool ElapsedTime02;

    public UnityEvent OnStart01;
    public UnityEvent OnEnd01;
    public UnityEvent OnStart02;
    public UnityEvent OnEnd02;
    


    IEnumerator StartCount01()
    {

        yield return new WaitForSeconds(WaitTime01);

        ElapsedTime01 = true;

    }

    IEnumerator StartCount02()
    {

        yield return new WaitForSeconds(WaitTime02);

        ElapsedTime02 = true;

    }


    public void StartEvent01()
    {
        if (WaitTimeActive01)
        {
            StartCoroutine(StartCount01());

            if (ElapsedTime01)
            {
                OnStart01.Invoke();
                ElapsedTime01 = false;
            }
        }
        else
        {
            OnStart01.Invoke();
        }
       
    }

    public void EndEvent01()
    {
        if (WaitTimeActive01)
        {
            StartCoroutine(StartCount01());

            if (ElapsedTime01)
            {
                OnEnd01.Invoke();
                ElapsedTime01 = false;
            }   
        }
        else
        {
            OnEnd01.Invoke();
        }
       
    }

    public void StartEvent02()
    {
        if (WaitTimeActive02)
        {
            StartCoroutine(StartCount02());

            if (ElapsedTime02)
            {
                OnStart02.Invoke();
                ElapsedTime02 = false;
            }
        }
        else
        {
            OnStart02.Invoke();
        }
       
    }

    public void EndEvent02()
    {
        if (WaitTimeActive02)
        {
            StartCoroutine(StartCount02());

            if (ElapsedTime02)
            {
                OnEnd02.Invoke();
                ElapsedTime02 = false;
            }
        }
        else
        {
            OnEnd02.Invoke();
        }
        
    }

}
