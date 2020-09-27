using UnityEngine;
using UnityEngine.Events;

public class VacumnScript : MonoBehaviour
{
   
    public Transform EndPoint;
   
    private Vector3 _startPoint;
    private float _animationTimePosition;
    private bool OnRange;
    public bool Enabled;

    public float outerRadius = 0f;
  //  public float innerRadius = 0f;
    public float speed;
    public float Distance = 3f;

    private bool PlayedOnce;
    public UnityEvent OnSunctionDistance;


    void Update()
    {
        

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Enabled = true;
        }
        else if (Input.GetKeyUp(KeyCode.Space))
        {
            Enabled = false;

        }

        if (Enabled)
        {
            Collider[] objects = Physics.OverlapSphere(transform.position, outerRadius);
            foreach (Collider col in objects)
            {
                if (col.GetComponent<Rigidbody>())
                { //Must be rigidbody
                   

                    Vector3 position = Vector3.MoveTowards(col.transform.position, EndPoint.position, speed * Time.fixedDeltaTime /*/ Vector3.Distance(col.transform.position, EndPoint.position)*/);

                    col.GetComponent<Rigidbody>().MovePosition(position);

                    if (Vector3.Distance(col.transform.position, transform.position) <= Distance && Enabled)
                    {
                        //Start Audio Event Part1 (second part will be placed on Second Collider thats using MoveObjectOnCollision script)

                        col.GetComponent<Rigidbody>().isKinematic = true;
                        col.GetComponent<Rigidbody>().useGravity = false;

                       // PlayEvent();
                       

                    }
                    else 
                    {
                        col.GetComponent<Rigidbody>().isKinematic = false;
                        col.GetComponent<Rigidbody>().useGravity = true;
                    }

                    if (!Enabled)
                    {
                        col.GetComponent<Rigidbody>().isKinematic = false;
                        col.GetComponent<Rigidbody>().useGravity = true;
                    }

                }
               
                
               
            }
        }
       
    }

    //void PlayEvent()
    //{
    //    if (!PlayedOnce)
    //    {
            
    //        Debug.Log("Played twice?");
    //        OnSunctionDistance.Invoke();
    //        //Do 
    //        PlayedOnce = true;
    //    }
    //}

   
    void OnDrawGizmos()
    {
        if (Enabled)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, outerRadius);
           // Gizmos.color = Color.yellow;
           // Gizmos.DrawWireSphere(transform.position, innerRadius);
        }
    }

 }