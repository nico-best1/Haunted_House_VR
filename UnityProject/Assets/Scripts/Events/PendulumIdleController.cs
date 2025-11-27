using UnityEngine;

public class PendulumIdleController : MonoBehaviour
{
    public Transform pendulum;
    public float swingSpeed = 2f;
    public float swingAngle = 5f;

    private Quaternion startRotation;
    private bool isActive = true; // Siempre activo

    private FMOD.Studio.EventInstance clockInstance;

    public FMOD.Studio.EventInstance ClockInstance => clockInstance;

    // ============================================================
    // RECIBE LA INSTANCIA DEL RELOJ YA CREADA EN OTRO SCRIPT
    // ============================================================
    public void AssignClockInstance(FMOD.Studio.EventInstance inst)
    {
        clockInstance = inst;
    }

    void Start()
    {
        if (pendulum != null)
            startRotation = pendulum.localRotation;
    }

    void Update()
    {
        if (!isActive || pendulum == null || clockInstance.handle == System.IntPtr.Zero) return;

        float angle = Mathf.Sin(Time.time * swingSpeed) * swingAngle; // Movimiento normal
        pendulum.localRotation = startRotation * Quaternion.Euler(0, 0, angle);
    }

    public void StopClockSound()
    {
        clockInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    void OnDestroy()
    {
        if (clockInstance.handle != System.IntPtr.Zero)
            clockInstance.release();
    }
}
