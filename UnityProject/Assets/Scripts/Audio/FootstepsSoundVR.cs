using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class FootstepsSoundVR : MonoBehaviour
{
    [Header("FMOD Event")]
    [SerializeField] private EventReference footstepEvent;

    [Header("Movement Detection")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float minMoveSpeed = 0.1f;

    private EventInstance footstepInstance;
    private bool isPlaying = false;

    private void Start()
    {
        if (characterController == null)
            characterController = GetComponent<CharacterController>();

        footstepInstance = AudioManager.Instance.CreateInstance(footstepEvent);
    }

    private void Update()
    {
        bool isMoving = characterController.velocity.magnitude > minMoveSpeed;

        if (isMoving && !isPlaying)
        {
            footstepInstance.start();
            isPlaying = true;
        }
        else if (!isMoving && isPlaying)
        {
            footstepInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            isPlaying = false;
        }
    }

    private void OnDestroy()
    {
        footstepInstance.release();
    }
}

