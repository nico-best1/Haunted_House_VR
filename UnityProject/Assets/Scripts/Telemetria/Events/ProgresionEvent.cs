using Unity.Mathematics;
using UnityEngine;

public struct positionEvent
{
    int x;
    int y;
    int z;
}

public class ProgresionEvent : TrackerEvent
{
    public positionEvent position;

    public ProgresionEvent(string eventType, int timeStamp, positionEvent p) : base(eventType, timeStamp) { this.position = p; }

    public void setPosition(positionEvent p) {  position = p; }
    public positionEvent getPosition() { return position; }

    public override string ToJSON() { return JsonUtility.ToJson(this); }
}
