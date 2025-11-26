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
        float nuevaVelocidad = velocidadMaxima * intensidad;

        moveProvider.moveSpeed = nuevaVelocidad;

        if (footStepsStarted)
        {
            footStepsSound.setParameterByName("Velocidad", intensidad);
        }
    }

    private void OnDestroy()
    {
        footStepsSound.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        footStepsSound.release();
    }
}
