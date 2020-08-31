using UnityEngine;

public class PingPongMove : MonoBehaviour
{
    public enum State { Inactive, Moving, Waiting }
    
    
    [SerializeField] private Transform PointA;
    [SerializeField] private Transform PointB;
    [SerializeField] private float MoveTime;
    [SerializeField] private float MaxSpeed = Mathf.Infinity;
    [SerializeField] private float WaitTime = 5f;
    // ReSharper disable once InconsistentNaming
    [SerializeField] private bool _active;

    private State _currentState;
    private bool _movingToA;
    private Vector3 _velocity;
    private float _waitStartTime;

    public bool Active
    {
        get => _active;
        set => _active = value;
    }

    private void OnEnable()
    {
        NewState(Active ? State.Moving : State.Inactive);
    }

    private void NewState(State newState)
    {
        _currentState = newState;
        
        switch (newState)
        {
            case State.Inactive:
                InactiveOnEnter();
                break;
            case State.Moving:
                MovingOnEnter();
                break;
            case State.Waiting:
                WaitingOnEnter();
                break;
        }
    }

    private void FixedUpdate()
    {
        if (PointA == null || PointB == null) return;

        switch (_currentState)
        {
            case State.Inactive:
                InactiveUpdate();
                break;
            case State.Moving:
                MovingUpdate();
                break;
            case State.Waiting:
                WaitingUpdate();
                break;
        }
    }

    private void InactiveOnEnter()
    {
        Active = false;
    }

    private void InactiveUpdate()
    {
        if (Active) NewState(State.Moving);
    }

    private void MovingOnEnter()
    {
        Active = true;
        _velocity = Vector3.zero;
    }

    private void MovingUpdate()
    {
        var moveTarget = _movingToA ? PointA.position : PointB.position;
        transform.position = Vector3.SmoothDamp(transform.position, moveTarget, ref _velocity, MoveTime, MaxSpeed,
            Time.deltaTime);

        if (!Active) NewState(State.Inactive); 
        else if (Vector3.Distance(transform.position, moveTarget) < 0.01f) NewState(State.Waiting);
    }

    private void WaitingOnEnter()
    {
        Active = true;
        _waitStartTime = Time.time;
        _movingToA = !_movingToA;
    }

    private void WaitingUpdate()
    {
        if (!Active) NewState(State.Inactive);
        else if (Time.time - _waitStartTime > WaitTime) NewState(State.Moving);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (PointA) Gizmos.DrawSphere(PointA.position, 0.5f);
        if (PointB) Gizmos.DrawSphere(PointB.position, 0.5f);
        if (PointA && PointB) Gizmos.DrawLine(PointA.position, PointB.position);
    }
}
