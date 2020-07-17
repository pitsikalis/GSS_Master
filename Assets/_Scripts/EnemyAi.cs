using UnityEngine;

public class EnemyAi : MonoBehaviour
{
    private GameObject player;
    private Transform target;
    public float MoveSpeed;
    private Vector2 direction;


    public float CurrentDirection;
    public float LastDirection;
    public float PastDirection;
    private Animator MyAnimator;

    public float MinimumDistance = 2f;

    // Start is called before the first frame update
    void Start()
    {
        MyAnimator = gameObject.GetComponent<Animator>();

        player = GameObject.FindGameObjectWithTag("Player");

        target = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();

        CurrentDirection = LastDirection = PastDirection = 1;
    }

    // Update is called once per frame
    void Update()
    {
        //Simple Follow the Player//

        float PlayerDistance = Vector3.Distance(transform.position, target.transform.position);
        Debug.Log(PlayerDistance);

        if (PlayerDistance > MinimumDistance)
        {
            FollowTarget();
            MyAnimator.SetBool("Attack", false);
        }
        else 
        {
            Attack();

           
        }
       

        MyAnimator.SetFloat("Horizontal", direction.x);
        MyAnimator.SetFloat("Vertical", direction.y);
        MyAnimator.SetFloat("Speed", direction.sqrMagnitude);


        ////
        ///


        if (direction.x >= 0.01f)
        {
            CurrentDirection = 2;
        }
        else if (direction.x <= -0.01f)
        {
            CurrentDirection = 4;
        }

        if (direction.y >= 0.01f)
        {
            CurrentDirection = 1;
        }
        else if (direction.y <= -0.01f)
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

    private void FollowTarget()
    {
        if (target != null)
        {
            direction = (target.transform.position - transform.position).normalized;
            transform.position = Vector2.MoveTowards(transform.position, target.position, MoveSpeed * Time.deltaTime);
        }
    }


    private void Attack()
    {

        if (target != null)
        {
            MyAnimator.SetBool("Attack", true);
            Debug.Log("I ATTACK!");
        }
           
    }
}
