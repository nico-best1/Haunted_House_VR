using System;

[Serializable]
public abstract class TelemetryEvent
{
    public string eventType;

    public string session_id;

    // timestamp en milisegundos
    public long timestamp;
}