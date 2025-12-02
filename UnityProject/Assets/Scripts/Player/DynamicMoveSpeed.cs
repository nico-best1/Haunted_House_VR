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

    // --- Suavizado de velocidad ---
    private float velocidadSuavizada = 0f;
    public float suavizado = 6f;

    // --- Nuevo: parámetro StepsType ---
    private int currentStepsType = 0;   // 0 = wood (default), 1 = carpet

    // LayerMask opcional para evitar raycast a otras cosas
    public LayerMask sueloLayerMask = ~0;

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

        // Suaviza cambios de velocidad
        velocidadSuavizada = Mathf.Lerp(velocidadSuavizada, intensidad, suavizado * Time.deltaTime);

        moveProvider.moveSpeed = velocidadMaxima * velocidadSuavizada;

        DetectarSuperficie();   // <-- NUEVO

        if (velocidadSuavizada > 0.05f)
        {
            if (!footStepsStarted)
            {
                footStepsSound.start();
                footStepsStarted = true;
            }

            // Parámetro de velocidad
            footStepsSound.setParameterByName("Velocidad", Mathf.Clamp01(velocidadSuavizada));

            // Parámetro StepsType
            footStepsSound.setParameterByName("StepsType", currentStepsType);
        }
        else
        {
            footStepsSound.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            footStepsStarted = false;
        }
    }

    private void DetectarSuperficie()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, 2f, sueloLayerMask))
        {
            SurfaceType surface = hit.collider.GetComponent<SurfaceType>();
            if (surface != null)
            {
                currentStepsType = (int)surface.tipo;
            }
        }
    }

    private void OnDestroy()
    {
        footStepsSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        footStepsSound.release();
    }
}
