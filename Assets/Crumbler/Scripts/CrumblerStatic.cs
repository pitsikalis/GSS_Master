//This code can be used for private or commercial projects but cannot be sold or redestibuted without written permission.
//Copyright Nik W. Kraus / Dark Cube Entertainment LLC.

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrumblerStatic : MonoBehaviour{

    public bool Activate = false;       
    public enum BuildTypeEnum { BottomUp, TopDown };

    [Tooltip("Build type, Virtical in world space")]
    public BuildTypeEnum BuildType;
        
    [Tooltip("Speed of each object moves back to start position")]
    public float Speed = 3f;
    [Tooltip("Speed of each object moves towards center of original shape")]
    public float GatherSpeed = 2f;
    [Tooltip("Speed of each object to start moving towards start position")]
    public float ClimbSpeed = .3f;
    [Tooltip("Each object rise speed in the world Y direction")]
    public float VertBurst = .5f;
    [Tooltip("Each object will use a RayCast to find a path back to start position")]
    public bool UseSeek = false;
    [Tooltip("Each object will be set to Kinematic as it reaches it's start position")]
    public bool SetOnBuild = false;
    [Tooltip("All objects will move to start position at the same time")]
    public bool UseSnap = false;
    [Tooltip("All objects will move in a swirl pattern back to thier start positions")]
    public bool UseSwirl = false;
    //public bool UseTeleport = false;
    public float SwirlPower = .3f;
    [Tooltip("All objects will move in a swirl pattern back to thier start positions")]
    public bool UseTimer = false;
    public float Timer = 2f;    
    //public Vector3 SwirlAxiz = new Vector3(0,1,0);
    [Tooltip("Editor preview color and size of object start positions and current climb position")]
    public Color PreviewColor = new Color(.8f, .1f, .5f, .5f);
    public float PreviewSize = 1f;

    [HideInInspector]
    public float DistThreshold = .1f;
    [HideInInspector]
    public List<GameObject> CHren;
    [HideInInspector]
    public List<Vector3> StartPos;
    [HideInInspector]
    public List<Quaternion> StartPosRot;
    [HideInInspector]
    public List<bool> CHrenSet;

    [HideInInspector]
    public float TimerTime;

    private int TotalSet = 0;
    private int ChPosCount;

    private Vector3 Force;
    private float CurrBuildHeight;
    private float HighestPos;
    private float LowestPos;
    
    private Transform[] allCHren;
    private float Dist;

    private bool CHCheck = false;
    private float t = 0f;
    private float FixedTime = 0f;

    private Vector3 bNormal;
    private float TAng;
    private RaycastHit hit;
    private RaycastHit hit2;
    

    void Awake()
    {
        allCHren = GetComponentsInChildren<Transform>();
        LowestPos = allCHren[0].position.y;
        t = 0f;

        foreach (Transform CH in allCHren)
        {
            if (CH.gameObject.GetComponent<Rigidbody>())
            {
                CHren.Add(CH.gameObject);
                StartPos.Add(CH.gameObject.transform.position);
                StartPosRot.Add(CH.gameObject.transform.rotation);
                CHrenSet.Add(false);

                //Define highest and Lowest World pos
                if (CH.gameObject.transform.position.y < LowestPos)
                {
                    LowestPos = CH.gameObject.transform.position.y - .5f;
                }

                if (CH.gameObject.transform.position.y > HighestPos)
                {
                    HighestPos = CH.gameObject.transform.position.y + .5f;
                }
            }
        }

        CurrBuildHeight = LowestPos;
        CHCheck = false;
        TimerTime = Time.time + Timer;
    }



    void Update()
    {
        if (UseTimer && !Activate)
        {
            if (TimerTime < Time.time)
            {
                TimerTime = Time.time + Timer;
                Activate = true;
            }
        }
    }


  
    void FixedUpdate()
    {
        if (Speed < 0f)
        {
            Speed = 0f;
        }

        if (Activate)
        {
            //Check for each item set state
            TotalSet = 0;

            for (int ii = 0; ii < CHren.Count; ii++)
            {
                if (CHrenSet[ii])
                {
                    TotalSet = TotalSet + 1;
                }
            }

            FixedTime = Time.deltaTime;
            t += ClimbSpeed * .005f;

            if (TotalSet != CHren.Count && !UseSnap)
            {
                DistThreshold = Mathf.Lerp(DistThreshold, 10f, (Time.deltaTime * Speed * .008f));
            }

            //Main Loop
            for (int i = 0; i < CHren.Count; i++)
            {
                if (!UseSnap)
                {
                    Dist = Vector3.Distance(StartPos[i], CHren[i].transform.position);

                    if (TotalSet == CHren.Count)
                    {
                        CHren[i].gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    }

                    if (Dist <= DistThreshold * 1.2f) {
                        CHren[i].transform.rotation = Quaternion.Lerp(CHren[i].transform.rotation, StartPosRot[i], FixedTime * Speed * .1f);

                        if (Dist < DistThreshold)
                        {
                            CHrenSet[i] = true;
                            CHren[i].transform.position = Vector3.Lerp(CHren[i].transform.position, StartPos[i], FixedTime * Speed);
                            CHren[i].transform.rotation = Quaternion.Lerp(CHren[i].transform.rotation, StartPosRot[i], FixedTime * Speed * .5f);

                            if (SetOnBuild && Dist < DistThreshold / 2)
                            {
                                CHren[i].gameObject.GetComponent<Rigidbody>().isKinematic = true;
                            }
                        }
                    }                    

                    //Main Threshold check
                    if (Dist > DistThreshold && StartPos[i] != CHren[i].transform.position)
                    {
                        CHren[i].gameObject.GetComponent<Rigidbody>().useGravity = false;
                        Force = StartPos[i] - CHren[i].transform.position;
                        Force = Force * CHren[i].GetComponent<Rigidbody>().mass;
                        bNormal = transform.TransformDirection(Force);

                        //Check Velocity Stalled items
                        Vector3 ChVelocity = CHren[i].gameObject.GetComponent<Rigidbody>().velocity;
                        float ChMag = CHren[i].gameObject.GetComponent<Rigidbody>().velocity.magnitude;

                        if (UseSeek && Dist > DistThreshold)
                        {                            
                            if (Physics.Raycast(CHren[i].transform.position, Force, out hit, ChMag))
                            {
                                TAng = TAng + .1f;
                                if(TAng > 360)
                                {
                                    TAng = 0;
                                }

                                Debug.DrawRay(CHren[i].transform.position, Force, Color.yellow);
                                Force = Vector3.Cross(Force, Vector3.forward) * .5f;                                
                                Vector3 TVec = Quaternion.AngleAxis(TAng, CHren[i].transform.position) * Vector3.Cross(bNormal, CHren[i].transform.position);
                                Debug.DrawRay(CHren[i].transform.position, TVec, Color.blue);

                                if (Physics.Raycast(CHren[i].transform.position, TVec, out hit2, ChMag))
                                {
                                    Force = Force - (TVec * .05f);
                                }
                                else
                                {
                                    Force = Force + (TVec * .05f);
                                }
                            }
                            else
                            {
                                Debug.DrawRay(CHren[i].transform.position, Force, Color.green);
                                if(ChMag < .1f)
                                {
                                    Debug.DrawRay(CHren[i].transform.position, Force, Color.red);
                                    Force = -Force * 5f;
                                }
                            }// End RayCast
                                                       
                            CHren[i].GetComponent<Rigidbody>().drag = 1 - 1 + Dist;
                            CHren[i].GetComponent<Rigidbody>().angularDrag = 1 - 1 + Dist;
                            
                        }
                        else
                        {
                            CHren[i].GetComponent<Rigidbody>().drag = 1 - 1 + Dist;
                            CHren[i].GetComponent<Rigidbody>().angularDrag = 1 - 1 + Dist;
                        }//End Velocity  

                     

                        //Build from top or bottom
                        if (BuildType == BuildTypeEnum.BottomUp)
                        {
                            CurrBuildHeight = Mathf.Lerp(LowestPos, HighestPos, t);
                            if (StartPos[i].y <= CurrBuildHeight)
                            {
                                if (!UseSwirl)
                                {
                                    CHren[i].GetComponent<Rigidbody>().AddForce(Force * (Speed + DistThreshold));
                                }
                                else
                                {
                                    Force = Force + Vector3.Cross(Force, Vector3.up) * Mathf.Lerp(SwirlPower, 0f, t);
                                    CHren[i].GetComponent<Rigidbody>().AddForce(Force + new Vector3(0, Force.y * .01f * Dist, 0));
                                    CHren[i].GetComponent<Rigidbody>().AddForce(Force * (Speed + DistThreshold));
                                }
                                CHren[i].GetComponent<Rigidbody>().AddForce(Force.x * GatherSpeed * Dist, 0, 0);
                                CHren[i].GetComponent<Rigidbody>().AddForce(0, 0, Force.z * GatherSpeed * Dist);
                                CHren[i].GetComponent<Rigidbody>().AddForce(0,Force.y * VertBurst * Dist, 0);
                            }
                        }
                        else
                        {
                            CurrBuildHeight = Mathf.Lerp(HighestPos, LowestPos, t);
                            if (StartPos[i].y >= CurrBuildHeight)
                            {
                                if (!UseSwirl)
                                {
                                    CHren[i].GetComponent<Rigidbody>().AddForce(Force * (Speed + DistThreshold));
                                }
                                else
                                {
                                    Force = Force + Vector3.Cross(Force, Vector3.up) * Mathf.Lerp(SwirlPower, 0f, t);
                                    CHren[i].GetComponent<Rigidbody>().AddForce(Force + new Vector3(0, Force.y * .01f * Dist, 0));
                                    CHren[i].GetComponent<Rigidbody>().AddForce(Force * (Speed + DistThreshold));
                                }
                                CHren[i].GetComponent<Rigidbody>().AddForce(Force.x * GatherSpeed * Dist, 0, 0);
                                CHren[i].GetComponent<Rigidbody>().AddForce(0, 0, Force.z * GatherSpeed * Dist);
                                CHren[i].GetComponent<Rigidbody>().AddForce(0, Force.y * VertBurst * Dist, 0);
                            }
                        }                                              
                    }

                }
                else
                {
                    CHren[i].gameObject.GetComponent<Rigidbody>().isKinematic = true;
                    CHren[i].transform.position = Vector3.Lerp(CHren[i].transform.position, StartPos[i], FixedTime * Speed);
                    CHren[i].transform.rotation = Quaternion.Lerp(CHren[i].transform.rotation, StartPosRot[i], FixedTime * Speed);
                }//End User Lerp
            }//End loop
        }
        else
        {
            if (TotalSet > 0 || CurrBuildHeight != 0f || UseSnap)
            {
                DistThreshold = 0f;
                CurrBuildHeight = LowestPos;
                ChPosCount = 0;
                TotalSet = 0;
                t = 0f;
                for (int i = 0; i < CHren.Count; i++)
                {
                    CHren[i].gameObject.GetComponent<Rigidbody>().isKinematic = false;
                    CHren[i].gameObject.GetComponent<Rigidbody>().useGravity = true;
                    CHren[i].GetComponent<Rigidbody>().drag = .0f;
                    CHren[i].GetComponent<Rigidbody>().angularDrag = .01f;
                    CHrenSet[i] = false;
                }
            }
        }
    }//End Fixed Update


    Rigidbody[] allCHrenRB;
    Vector3 BuildGizmo;

    void OnDrawGizmos()
    {
        //Vector3 BuildGizmo = new Vector3(0,0,0);
        if (transform.childCount > 0 && CHren != null)
        {
            for (int i = 0; i < CHren.Count; i++)
            {
                if (!CHrenSet[i])
                {
                    Gizmos.color = PreviewColor;
                }
                else
                {
                    Gizmos.color = Color.green;
                }

                Gizmos.DrawCube(StartPos[i], Vector3.one * PreviewSize / 2);
                BuildGizmo = (BuildGizmo + StartPos[i]);
            }

            BuildGizmo = BuildGizmo / CHren.Count;
            BuildGizmo.y = CurrBuildHeight;
            Gizmos.color = new Color(.1f, .1f, 1f, .5f);
            Gizmos.DrawCube(BuildGizmo, new Vector3(3f, .1f, 3f) * PreviewSize * 2);
        }
        
    }



    void OnDrawGizmosSelected()
    {
        if (!CHCheck)
        {
            CHCheck = true;
            allCHrenRB = GetComponentsInChildren<Rigidbody>();

            if (transform.childCount > 0)
            {
                if (allCHrenRB.Length < 1)
                {
                    Debug.Log("No Child objects have a RigidBody within " + transform.name + ", please add the component to items needed for effect.");
                }

                if (allCHrenRB.Length > 28)
                {
                    Debug.Log("More then 28 child objects have Rigidbodies within " + transform.name + ". Please keep the number below 30 to help improve performance.");
                }
            }
            else
            {
                Debug.Log("No Child objects found within " + transform.name + ", Please add some with Rigidbodies");
            }
        }       
    }//End Gizmo

}