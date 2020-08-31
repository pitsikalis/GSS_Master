using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelineEventMarker : Marker, INotification, INotificationOptionProvider
{
    public string EventName;
    public bool TriggerOnce = true;
    
    public PropertyName id => new PropertyName();
    public NotificationFlags flags => default;
}
