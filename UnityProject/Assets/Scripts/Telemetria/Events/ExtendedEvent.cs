using Unity.Mathematics;
using UnityEngine;

public class ExtendedEvent : ProgresionEvent
{
    public int angular_velocity;
    public positionEvent position_target;

    public ExtendedEvent(string eventType, int timeStamp, positionEvent p, int angular_velocity, positionEvent position_target) : base(eventType, timeStamp, p) { this.position = p; this.angular_velocity = angular_velocity; this.position_target = position_target; }

    public void setPositionTarget(positionEvent p) {  position_target = p; }
    public positionEvent getPositionTarget() { return position; }
    public void setAngularVelocity(int v) { angular_velocity = v; }
    public int getAngularVelocity() { return angular_velocity; }

    public override string ToJSON() { return JsonUtility.ToJson(this); }
}
