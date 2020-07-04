
using UnityEngine;
using System.Collections;

public class DampCamera2D : MonoBehaviour
{
    public Transform target;
    public float smoothTime = 0.3F;
    private Vector3 velocity = Vector3.zero;
    public CharacterMotor MyCharMotor;

    void Update()
    {
        // Define a target position above and behind the target transform
        Vector3 targetPositionZoom = target.TransformPoint(new Vector3(0, 0, -15));
        Vector3 targetPosition = target.TransformPoint(new Vector3(0, 0, -10));

        // Smoothly move the camera towards that target position
        //if (!MyCharMotor.IsRunning)
        //{
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
        //}
        //else if(MyCharMotor.Movement.x != 0 || MyCharMotor.Movement.y != 0)
        //{
        //    transform.position = Vector3.SmoothDamp(transform.position, targetPositionZoom, ref velocity, smoothTime);
        //}

        
    }
}

