using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WaterManager : MonoBehaviour
{
    // Start is called before the first frame update

  
    public float WaterHealth = 100f;
    public float ScaleDownTime = 5f;
    public GameObject SplashPrefab;

    private bool Grounded;


    //[HideInInspector]
    public bool OnFire;

    public UnityEvent OnExhaustion;
    public UnityEvent OnGroundCollision;

    private void OnCollisionEnter(Collision other)
    {
        if ((other.gameObject.tag == "Ground") && !Grounded)
        {
            Grounded = true;
            OnGroundCollision.Invoke();
            if (SplashPrefab != null)
            {
                Instantiate(SplashPrefab, transform.position, Quaternion.identity);
               
            }
           
        }
    }

    void Update()
    {
        if (OnFire)
        {
            KillWater();
        }
    }

    public void KillWater()
    {
        
        StartCoroutine(ScaleOverTime(ScaleDownTime));
    }


   

    IEnumerator ScaleOverTime(float time)
    {
        Vector3 originalScale = gameObject.transform.localScale;
        Vector3 destinationScale = new Vector3(0f, 0f, 0f);

        float currentTime = 0.0f;

        do
        {
            gameObject.transform.localScale = Vector3.Lerp(originalScale, destinationScale, currentTime / time);
            currentTime += Time.deltaTime;
            yield return null;
        } while (currentTime <= time);

       

        OnExhaustion.Invoke();
        Destroy(gameObject);
    }
}
