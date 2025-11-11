using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(AudioSource))]
public class FootstepAudio : MonoBehaviour
{
    public ActionBasedContinuousMoveProvider moveProvider;
    public AudioClip footstepClip;
    public float minMoveSpeed = 0.1f;

    private AudioSource audioSource;
    private CharacterController characterController;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = footstepClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 1.0f;

        characterController = GetComponent<CharacterController>();

        if (moveProvider == null)
            Debug.LogWarning("No se asignó el Move Provider al FootstepAudio");
    }

    void Update()
    {
        if (characterController == null || moveProvider == null)
            return;

        // Use the correct property for input actions
        InputActionProperty inputMoveAction = moveProvider.leftHandMoveAction; // or rightHandMoveAction based on your setup
        Vector2 input = inputMoveAction.action.ReadValue<Vector2>();
        bool isMoving = input.magnitude > minMoveSpeed;

        if (isMoving)
        {
            if (!audioSource.isPlaying)
                audioSource.Play();
        }
        else
        {
            if (audioSource.isPlaying)
                audioSource.Stop();
        }
    }
}
