using System;
using Unity.Mathematics;
using UnityEngine;

[System.Serializable]
public struct PositionEvent
{
    public float x;
    public float y;
    public float z;

    // Constructor opcional para facilitar la creación
    public PositionEvent(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

public class ProgresionEvent : TrackerEvent
{
    public PositionEvent position;

    public ProgresionEvent(string eventType, int timeStamp, PositionEvent p) : base(eventType, timeStamp) { this.position = p; }

    public void setPosition(PositionEvent p) {  position = p; }
    public PositionEvent getPosition() { return position; }

    public override string ToJSON() { return JsonUtility.ToJson(this); }
    public override string ToCSV() { return FormattableString.Invariant($"{eventType},{sessionId},{eventId},{timeStamp}, {position.x}, {position.y}, {position.z}"); }
}
