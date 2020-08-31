using UnityEngine;
using UnityEngine.Events;

public class TimeCounter : MonoBehaviour
{
    private bool TimerActive { get; set; }

    [SerializeField]
    private float timerLength;

    private float timeOfStart;

    public UnityEvent timerStartedEvent;
    public UnityEvent timerExpiredEvent;

    public void AddTime(float seconds = 5f)
    {
        if (!TimerActive)
            return;


        timerLength += seconds;
    }


    public void ResetCounter()
    {
        if (!TimerActive)
            return;


        timeOfStart = Time.time;
    }

    public void SetCounter(float value)
    {
        timerLength = value;
    }

    public void SetCounterActive()
    {
        TimerActive = true;

        StartCounter();

    }
    
    public void StartCounter()
    {
        if (TimerActive)
        {
            TimerActive = true;
            timeOfStart = Time.time;
            timerStartedEvent.Invoke();
        }

    }

    public void StopCounter()
    {
        TimerActive = false;
    }

    private void OnCounterExpired()
    {
        TimerActive = false;
        timerExpiredEvent.Invoke();
    }

    private void Awake()
    {
        TimerActive = false;
    }

    private void Update()
    {
        if (TimerActive)
        {
            if (Time.time - timeOfStart > timerLength)
            {
                OnCounterExpired();
            }
                
        }
      
    }
}
