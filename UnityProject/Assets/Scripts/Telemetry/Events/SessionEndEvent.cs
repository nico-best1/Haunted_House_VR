using System;

[Serializable]
public class SessionEndEvent : TelemetryEvent
{
    public float sessionDuration;

    public SessionEndEvent(
        string sessionId,
        float duration
    )
    {
        eventType = "Session_End";

        session_id = sessionId;

        sessionDuration = duration;

        timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}