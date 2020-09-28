using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BombTrigger : MonoBehaviour
{
 

    //public bool PlayOnAwake;
  
    [HideInInspector]
    public bool BombIsActive;
    
    public bool CounterIsActive;

    public bool OnFire; // For Debug

    public float CounterTime;

    private Animator MyAnim;


    [Space()]
    [SerializeField]
    private UnityEvent OnActivation;
    [Space()]
    [SerializeField]
    private UnityEvent OnExplosion;


    void Start()
    {

        MyAnim = this.GetComponent<Animator>();

        //if(PlayOnAwake)
        //{
        //    BombIsActive = true; 
        //}
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

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Fire")
        {
            OnFire = true; // For Debug
            MyAnim.SetBool("OnFire", true);
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

    private void Activate()
    {
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
