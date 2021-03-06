﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;

public class CharacterMotor : MonoBehaviour
{

    public LayerMask groundLayers;

    public float JumpForce = 5f;

    public float MoveSpeed = 5f;

    public float RunSpeed = 8f;

    private bool isgrounded = true;

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
    public UnityEvent OnRunEnter;
    public UnityEvent OnRunExit;


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
        if (isgrounded)
        {

            //Attack
            if (Input.GetKeyDown(KeyCode.E))
            {
                StartAttack();
            }


            //Dodge
            if (Input.GetKeyDown(KeyCode.Q))
            {
                StartDodge();
            }


            //Jump
            if (Input.GetKeyDown(KeyCode.Space))
            {
                MyAnimator.SetBool("Jump", true);
                StartJump();
            }


            //Movement

            if (Input.GetKey(KeyCode.LeftShift))
            {
                StartRun();
            }

            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                EndRun();
                EndRun();
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
                    //Debug.Log("idle up");
                }

                if (CurrentDirection == 2)
                {
                    //Debug.Log("idle right");
                }

                if (CurrentDirection == 3)
                {
                    //Debug.Log("idle down");
                }

                if (CurrentDirection == 4)
                {
                    //Debug.Log("idle left");
                }
            }
        }
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.layer == 9)
        {
            isgrounded = true;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.gameObject.layer == 9)
        {
            isgrounded = false;
        }
    }

    void FixedUpdate()
    {
        //Movement
        if (!InAction)
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
    void EndJump()
    {
        InAction = false;
        IsJumping = false;
        MyAnimator.SetBool("Jump", false);
    }

    void StartRun()
    {
        IsRunning = true;
        MyAnimator.SetBool("Running", true);
        OnRunEnter.Invoke();
    }

    void EndRun()
    {
        IsRunning = false;
        MyAnimator.SetBool("Running", false);
        OnRunExit.Invoke();
    }

    void StartJump()
    {
        InAction = true;
        IsJumping = true;

        MyRigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
    }
    void EndAttack()
    {
        InAction = false;
        IsAttacking = false;
        MyAnimator.SetBool("Attack", false);
        OnAttackExit.Invoke();
    }

    void StartAttack()
    {
        InAction = true;
        IsAttacking = true;
        MyAnimator.SetBool("Attack", true);

        OnAttackEnter.Invoke();
    }

    void StartDodge()
    {

        InAction = true;
        IsDodging = true;
        MyAnimator.SetBool("Dodge", true);
    }

    void EndDodge()
    {
        InAction = false;
        IsDodging = false;
        MyAnimator.SetBool("Dodge", false);
    }


  
}
