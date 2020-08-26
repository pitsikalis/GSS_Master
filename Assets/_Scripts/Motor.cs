﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using Com.LuisPedroFonseca.ProCamera2D.TopDownShooter;

public class Motor : MonoBehaviour
{
    private InputHandler _Input;

    [SerializeField]
    private float moveSpeed;

    [SerializeField]
    private float rotateSpeed;

    [SerializeField]
    private bool rotateTowardsMouse;

    [SerializeField]
    private Camera _camera;
    private void Awake()
    {
        _Input = GetComponent<InputHandler>();
    }

    void Update()
    {
        var targetVector = new Vector3(_Input.InputVector.x, 0, _Input.InputVector.y);

        var movementVector =  MoveTowardTarget(targetVector);
        if (!rotateTowardsMouse)
        {
            RotateTowardMovementVector(movementVector);
        }
        else 
        {
            RotateTowardMouseVector();
        }
        


    }
    private void RotateTowardMouseVector()
    {
        Ray ray = _camera.ScreenPointToRay(_Input.MousePosition);

        if(Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
        {
            var target = hitInfo.point;
            target.y = transform.position.y;
            transform.LookAt(target);
        }
    }
    private void RotateTowardMovementVector(Vector3 movementVector)
    {
        var rotation = Quaternion.LookRotation(movementVector);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, rotateSpeed);
    }

    private Vector3 MoveTowardTarget (Vector3 targetVector)
    {
        var speed = moveSpeed * Time.deltaTime;

        targetVector = Quaternion.Euler(0, _camera.gameObject.transform.eulerAngles.y, 0) * targetVector;
        var targetPosition = transform.position + targetVector * speed;
        transform.position = targetPosition;
        return targetVector;
    }

}
