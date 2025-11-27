using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;

public class DynamicMoveSpeed : MonoBehaviour
{
    public ContinuousMoveProvider moveProvider;
    public InputActionProperty joystick;
    public float velocidadMaxima = 2.4f;

    private FMOD.Studio.EventInstance footStepsSound;
    private bool footStepsStarted = false;

    // --- NUEVO: suavizado del parámetro para que no se corte ---
    private float velocidadSuavizada = 0f;
    public float suavizado = 6f;  // puedes ajustar → 4 suave | 8 más reactivo

    private void Start()
    {
        footStepsSound = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.Instance.playerFootsteps);
        footStepsSound.start();
        footStepsStarted = true;
    }

    void Update()
    {
        Vector2 input = joystick.action.ReadValue<Vector2>();
        float intensidad = input.magnitude;

        // Suaviza cambios de velocidad para evitar silencios
        velocidadSuavizada = Mathf.Lerp(velocidadSuavizada, intensidad, suavizado * Time.deltaTime);

        moveProvider.moveSpeed = velocidadMaxima * velocidadSuavizada;

        if (velocidadSuavizada > 0.05f)
        {
            if (!footStepsStarted)
            {
                footStepsSound.start();
                footStepsStarted = true;
            }

            // Parámetro mucho más estable → el loop ya no se rompe
            footStepsSound.setParameterByName("Velocidad", Mathf.Clamp01(velocidadSuavizada));
        }
        else
        {
            footStepsSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            footStepsStarted = false;
        }
    }

    private void OnDestroy()
    {
        footStepsSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        footStepsSound.release();
    }
}
