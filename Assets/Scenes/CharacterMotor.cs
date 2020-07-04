using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMotor : MonoBehaviour
{
    public float MoveSpeed = 5f;

    public float RunSpeed = 8f;

    public Rigidbody2D MyRigidbody;

    public Vector2  Movement;

    public bool IsRunning = false;

    public Animator MyAnimator;

   
  

    public float CurrentDirection;
    public float LastDirection;
    public float PastDirection;

    // Start is called before the first frame update
    void Start()
    {
        MyAnimator = gameObject.GetComponent<Animator>();

        CurrentDirection = LastDirection = PastDirection = 1;
    }

    // Update is called once per frame
    void Update()
    {
        Movement.x = Input.GetAxisRaw("Horizontal");
        Movement.y = Input.GetAxisRaw("Vertical");

        MyAnimator.SetFloat("Horizontal", Movement.x);
        MyAnimator.SetFloat("Vertical", Movement.y);
        MyAnimator.SetFloat("Speed", Movement.sqrMagnitude);


       

        if (Movement.x >= 0.01f)
        {
            CurrentDirection = 2;
        }
        else if (Movement.x <= -0.01f)
        {
            CurrentDirection = 4;
        }

        if (Movement.y >= 0.01f)
        {
            CurrentDirection = 1;
        }
        else if (Movement.y <= -0.01f)
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
        if (Input.GetKey(KeyCode.LeftShift))
        {
            IsRunning = true;
            MyRigidbody.MovePosition(MyRigidbody.position + Movement * RunSpeed * Time.deltaTime);
        }
        else 
        {
            IsRunning = false;
            MyRigidbody.MovePosition(MyRigidbody.position + Movement * MoveSpeed * Time.deltaTime);
        }





        
    }
}
