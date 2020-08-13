using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class CharacterMotor : MonoBehaviour
{

    public LayerMask groundLayers;

    public float JumpForce = 5f;

    public float MoveSpeed = 5f;

    public float RunSpeed = 8f;

   

    public Vector3  Movement;

    public bool IsRunning = false;
    public bool IsAttacking = false;
    public bool IsJumping = false;
    public bool InAction  = false;

    public bool IsDodging = false;

    public Animator MyAnimator;

    private SphereCollider SphereCol;
    private Rigidbody MyRigidbody;


    public float CurrentDirection;
    public float LastDirection;
    public float PastDirection;


    public UnityEvent OnAttackEnter;
    public UnityEvent OnAttackExit;


    // Start is called before the first frame update
    void Start()
    {
        MyAnimator = gameObject.GetComponent<Animator>();
        SphereCol = gameObject.GetComponent<SphereCollider>();
        MyRigidbody = gameObject.GetComponent<Rigidbody>();
        CurrentDirection = LastDirection = PastDirection = 1;
    }

    // Update is called once per frame
    void Update()
    {
        Movement.x = Input.GetAxisRaw("Horizontal");
        Movement.z = Input.GetAxisRaw("Vertical");

        MyAnimator.SetFloat("Horizontal", Movement.x);
        MyAnimator.SetFloat("Vertical", Movement.z);
        MyAnimator.SetFloat("Speed", Movement.sqrMagnitude);

        //Attack
        if (Input.GetKeyDown(KeyCode.E))
        {
            InAction = true;
            IsAttacking = true;
            MyAnimator.SetBool("Attack", true);

            OnAttackEnter.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.E))
        {
            InAction = false;
            IsAttacking = false;
            MyAnimator.SetBool("Attack", false);
            OnAttackExit.Invoke();
        }

        //Dodge
        if (Input.GetKeyDown(KeyCode.Q))
        {
            InAction = true;
            IsDodging = true;
            MyAnimator.SetBool("Dodge", true);
        }


        if (Input.GetKeyUp(KeyCode.Q))
        {
            InAction = false;
            IsDodging = false;
            MyAnimator.SetBool("Dodge", false);
        }


        //Jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            InAction = true;
            IsJumping = true;

            MyAnimator.SetBool("Jump", true);
            MyRigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            OnAttackEnter.Invoke();
        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            InAction = false;
            IsJumping = false;
            MyAnimator.SetBool("Jump", false);
            OnAttackExit.Invoke();
        }

        //Movement

        if (Input.GetKey(KeyCode.LeftShift))
        {
            IsRunning = true;
            MyAnimator.SetBool("Running", true);
        }

        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            IsRunning = false;
            MyAnimator.SetBool("Running", false);
        }


        if (Movement.x >= 0.01f)
        {
            CurrentDirection = 2;
        }
        else if (Movement.x <= -0.01f)
        {
            CurrentDirection = 4;
        }

        if (Movement.z >= 0.01f)
        {
            CurrentDirection = 1;
        }
        else if (Movement.z <= -0.01f)
        {
            CurrentDirection = 3;
        }

        if (LastDirection != CurrentDirection)
        {
            PastDirection = LastDirection;
            LastDirection = CurrentDirection;

            MyAnimator.SetFloat("LastKnownDirection", CurrentDirection);

            if (CurrentDirection == 1)
            {
                Debug.Log("idle up");
            }

            if (CurrentDirection == 2)
            {
                Debug.Log("idle right");
            }

            if (CurrentDirection == 3)
            {
                Debug.Log("idle down");
            }

            if (CurrentDirection == 4)
            {
                Debug.Log("idle left");
            }

        }

      


    }

    void FixedUpdate()
    {
        //Movement
        if (!InAction )
        {


            if (!IsRunning)
            {
                MyRigidbody.MovePosition(MyRigidbody.position + Movement * MoveSpeed * Time.deltaTime);
            }
            else 
            {
                MyRigidbody.MovePosition(MyRigidbody.position + Movement * RunSpeed * Time.deltaTime);
            }
                
          
        }
    }

    //private bool IsGrounded()
    //{
    //    return Physics.CheckCapsule(SphereCol.bounds.center, new Vector3 (SphereCol.bounds.center.x, SphereCol.bounds.min.y, SphereCol.bounds.min.y, SphereCol.bounds.center.z),
    //        SphereCol.radius * 0.9f, groundLayers);
         
    //}
}
