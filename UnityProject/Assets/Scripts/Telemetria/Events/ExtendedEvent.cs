using Unity.Mathematics;
using UnityEngine;

public class ExtendedEvent : ProgresionEvent
{
    public int angular_velocity;
    public PositionEvent position_target;

    public ExtendedEvent(string eventType, int timeStamp, PositionEvent p, int angular_velocity, PositionEvent position_target) : base(eventType, timeStamp, p) { this.position = p; this.angular_velocity = angular_velocity; this.position_target = position_target; }

    public void setPositionTarget(PositionEvent p) {  position_target = p; }
    public PositionEvent getPositionTarget() { return position; }
    public void setAngularVelocity(int v) { angular_velocity = v; }
    public int getAngularVelocity() { return angular_velocity; }

    public override string ToJSON() { return JsonUtility.ToJson(this); }
}
