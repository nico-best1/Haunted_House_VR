
using UnityEngine;

public class TrackerEvent
{
    public string eventType;
    public string sessionId;
    public string eventId;
    public int timeStamp;

    public TrackerEvent(string eventType, int timeStamp) { this.eventType = eventType; this.timeStamp = timeStamp; }

    public void setTimeStamp(int t) { timeStamp = t; }
    public void setEventType(string type) { eventType = type; }
    public void setSessionId(string id) { sessionId = id; }
    public void setEventId(string id) { eventId = id; }

    public int getTimeStamp() { return timeStamp; }
    public string getEventType() { return eventType; }
    public string getSessionId() { return sessionId; }
    public string getEventId() { return eventId; }

    public virtual string ToJSON() { 
        return JsonUtility.ToJson(this);
    }

    public virtual string ToCSV()
    {
        // Variables separadas por comas.
        return $"{eventType},{sessionId},{eventId},{timeStamp}";
    }
}
