using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class EcoObject : MonoBehaviour
{
    [Header("Fire Settings")]
    [SerializeField]
    private float TimeTillIgnition;
    [SerializeField]
    private float TimeTillBurnout;
    //[SerializeField]
    private bool OnFire;
    //[SerializeField]
    private bool Watered;
    private bool WetToDry;
    private bool IsBurnout;


    [Header("Water Settings")]
    [SerializeField]
    private float TimeTillWaterDown;
    [SerializeField]
    private float TimeTillDry;

    [SerializeField]
    private float DryCounter;
    [SerializeField]
    private float BurnoutCounter;


    public UnityEvent OnIgnition;
    public UnityEvent OnStartBurnout;
    public UnityEvent OnBurnedout;
    public UnityEvent OnWater;

    // Start is called before the fir   st frame update

    private void Update()
    {
        UpdateTimer();

        ApplyFireDamage();

        if (DryCounter >= TimeTillDry)
        {
           
            WetToDry = false;
            Watered = false;
            DryCounter = 0f;
        }

        if (BurnoutCounter >= TimeTillBurnout)
        {
            IsBurnout = true;
            Watered = false;
            OnFire = false;
           
            StartCoroutine(StartBurnoutCount());
           
            OnStartBurnout.Invoke();
        }
        
    }

    private void UpdateTimer()
    {
      
            if (WetToDry)
            {
                DryCounter += Time.deltaTime;

            }

            if (OnFire)
            {
                BurnoutCounter += Time.deltaTime;
            }
           
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsBurnout)
        {
            if (other.gameObject.tag == "Fire")
            {
                if (Watered)
                {
                    WetToDry = true;
                    //StartCoroutine(StartDryCount());
                }
                else if (!OnFire)
                {
                    StartCoroutine(StartFireCount());
                }

            }


            if (other.gameObject.tag == "Water")
            {
                if (OnFire)
                {

                    DryCounter = 0f;
                    StartCoroutine(StartWaterCount());
                }
                else
                {
                    BurnoutCounter = 0f;
                    DryCounter = 0f;
                    Watered = true;
                }
            }
        }
      
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsBurnout)
        {
            if (other.gameObject.tag == "Water")
            {
                WetToDry = true;
                DryCounter = 0f;
            }

          
        }
       
       
    }
    IEnumerator StartBurnoutCount()
    {

        yield return new WaitForSeconds(0.5f);
       
        OnBurnedout.Invoke();
       



    }

    IEnumerator StartFireCount()
    {

        yield return new WaitForSeconds(TimeTillIgnition);
        DryCounter = 0f;
        OnIgnition.Invoke();
        OnFire = true;
        Watered = false;



        
    }

    IEnumerator StartWaterCount()
    {

        yield return new WaitForSeconds(TimeTillWaterDown);

        OnWater.Invoke();
        OnFire = false;
        Watered = true;
    }

    public void ApplyFireDamage()
    {
       
        if (!IsBurnout)
        {
            if (OnFire)
            {
                //Apply Fire Damage to player / NPC
                // Spawn FireDamage Collider
            }
        }

            
    }

    
    //IEnumerator StartDryCount()
    //{

    //    yield return new WaitForSeconds(TimeTillDry);

    //    Watered = false;
    //}
}
