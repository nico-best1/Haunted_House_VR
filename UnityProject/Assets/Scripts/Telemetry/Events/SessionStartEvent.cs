using System;

[Serializable]
public class SessionStartEvent : TelemetryEvent
{
    public SessionStartEvent(string sessionId)
    {
        eventType = "Session_Start";

        session_id = sessionId;

        timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
    }
}
