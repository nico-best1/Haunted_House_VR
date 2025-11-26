using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Movement;
using static UnityEngine.Rendering.DebugUI;

public class DynamicMoveSpeed : MonoBehaviour
{
    public ContinuousMoveProvider moveProvider;
    public InputActionProperty joystick;
    public float velocidadMaxima = 2.4f;
    private FMOD.Studio.EventInstance footStepsSound;

    private void Start()
    {
        footStepsSound = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.Instance.playerFootsteps);
        footStepsSound.start();
        footStepsSound.setPaused(true);
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input = joystick.action.ReadValue<Vector2>();
        float intensidad = input.magnitude;

        float nuevaVelocidad = velocidadMaxima * intensidad;
        if (nuevaVelocidad == 0)
            footStepsSound.setPaused(true);
        else
        {
            footStepsSound.setPaused(false);
            footStepsSound.setParameterByName("Velocidad", intensidad);
        }
        moveProvider.moveSpeed = nuevaVelocidad;
    }
}
