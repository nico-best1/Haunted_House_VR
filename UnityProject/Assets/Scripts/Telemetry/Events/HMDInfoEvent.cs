using System;
using UnityEngine;

[Serializable]
public class HMDInfoEvent : TelemetryEvent
{
    public Vector3 angular_velocity;

    public Vector3 position_target;

    public Vector3 hmd_position;

    public Quaternion hmd_rotation;
}
