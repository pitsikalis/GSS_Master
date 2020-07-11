using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    private GameObject player;
    public float MoveSpeed;



    public float CurrentDirection;
    public float LastDirection;
    public float PastDirection;
    private Animator MyAnimator;

    // Start is called before the first frame update
    void Start()
    {
        MyAnimator = gameObject.GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player");

        CurrentDirection = LastDirection = PastDirection = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //Simple Follow the Player//

        transform.position += (player.transform.position - transform.position).normalized * MoveSpeed * Time.deltaTime;

        if (transform.position.x >= 0.01f)
        {
            CurrentDirection = 2;
        }
        else if (transform.position.x <= -0.01f)
        {
            CurrentDirection = 4;
        }

        if (transform.position.y >= 0.01f)
        {
            CurrentDirection = 1;
        }
        else if (transform.position.y <= -0.01f)
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
}
