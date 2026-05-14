using System;
using UnityEngine;

[Serializable]
public class GameplayEvent : TelemetryEvent
{
    public Vector3 position;

    public string payload;

    public GameplayEvent(
        string sessionId,
        string eventName,
        Vector3 worldPosition,
        string data = ""
    )
    {
        eventType = eventName;

        session_id = sessionId;

        timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        position = worldPosition;

        payload = data;
    }
}