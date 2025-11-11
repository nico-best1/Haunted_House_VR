using UnityEngine;

public class PendulumIdleController : MonoBehaviour
{
    public Transform pendulum;
    public AudioSource clockAudio;
    public float swingSpeed = 2f;
    public float swingAngle = 5f;

    private Quaternion startRotation;
    private bool isActive = true;

    void Start()
    {
        if (pendulum != null)
            startRotation = pendulum.localRotation;

        if (clockAudio != null && !clockAudio.isPlaying)
        {
            clockAudio.volume = 0.2f;
            clockAudio.pitch = 1f;
            clockAudio.loop = true;
            clockAudio.Play();
        }
    }

    void Update()
    {
        if (!isActive || pendulum == null) return;

        float angle = Mathf.Sin(Time.time * swingSpeed) * swingAngle;
        pendulum.localRotation = startRotation * Quaternion.Euler(0, 0, angle);
    }

    // Llamar desde otro script para detener la animación y el sonido
    public void StopPendulum()
    {
        isActive = false;

        if (clockAudio != null)
            clockAudio.Stop();
    }

    // Llamar para reanudar el movimiento y sonido del péndulo
    public void StartPendulum()
    {
        isActive = true;

        if (clockAudio != null && !clockAudio.isPlaying)
        {
            clockAudio.volume = 0.2f;
            clockAudio.pitch = 1f;
            clockAudio.loop = true;
            clockAudio.Play();
        }
    }
}
