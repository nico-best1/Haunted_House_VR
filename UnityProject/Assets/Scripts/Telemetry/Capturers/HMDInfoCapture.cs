using System;
using UnityEngine;

/// <summary>
/// Captura información periódica del HMD y la envía al sistema de telemetría.
/// </summary>
public class HMDInfoCapture : MonoBehaviour
{
    [Header("References")]
    [SerializeField]
    private Transform hmd;

    [Header("Capture")]
    [SerializeField]
    private float captureInterval = 1f; // En segundos

    [Header("Raycast")]
    [SerializeField]
    private float rayDistance = 100f;

    [SerializeField]
    private LayerMask raycastLayers = ~0;

    private float timer;

    private Quaternion previousRotation;

    private void Start()
    {
        if (hmd == null)
        {
            hmd = transform;
        }

        previousRotation = hmd.rotation;
    }

    private void Update()
    {
        if (TelemetryManager.Instance == null)
            return;

        timer += Time.deltaTime;

        if (timer >= captureInterval)
        {
            timer = 0f;

            CaptureHMDInfo();
        }
    }

    /// <summary>
    /// Captura snapshot del estado actual del HMD.
    /// </summary>
    private void CaptureHMDInfo()
    {
        long time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        Vector3 angularVelocity = CalculateAngularVelocity();

        Vector3 targetPosition = GetTargetPosition();

        HMDInfoEvent telemetryEvent =
            new HMDInfoEvent
            {
                eventType = "HMD_Info",

                session_id = TelemetryManager.Instance.SessionId,

                timestamp = time,

                angular_velocity = angularVelocity,

                position_target = targetPosition,

                hmd_position = hmd.position,

                hmd_rotation = hmd.rotation
            };

        TelemetryManager
            .Instance
            .Track(telemetryEvent);
    }

    /// <summary>
    /// Calcula velocidad angular aproximada
    /// entre capturas.
    /// </summary>
    private Vector3 CalculateAngularVelocity()
    {
        Quaternion delta = hmd.rotation * Quaternion.Inverse(previousRotation);

        delta.ToAngleAxis(
            out float angle,
            out Vector3 axis
        );

        previousRotation = hmd.rotation;

        // Evitar NaN
        if (float.IsInfinity(axis.x))
            return Vector3.zero;

        if (captureInterval <= 0f)
            return Vector3.zero;

        // radianes/segundo
        float radians = angle * Mathf.Deg2Rad;

        return axis * (radians / captureInterval);
    }

    /// <summary>
    /// Obtiene el primer punto
    /// al que mira el HMD.
    /// </summary>
    private Vector3 GetTargetPosition()
    {
        Ray ray = new Ray(
            hmd.position,
            hmd.forward
        );

        if (
            Physics.Raycast(
                ray,
                out RaycastHit hit,
                rayDistance,
                raycastLayers
            )
        )
        {
            return hit.point;
        }

        return Vector3.zero;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (hmd == null)
            return;

        Gizmos.DrawRay(
            hmd.position,
            hmd.forward * rayDistance
        );
    }
#endif
}