using UnityEngine;
using FMODUnity;

public class FMODInteractableEvents : MonoBehaviour
{
    public void PlaySocketSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.puzzleCubeSound);
        }
        else
        {
            Debug.LogWarning("AudioManager instance not found!");
        }
    }

    public void PlayFlashlightSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.flashlight);
        }
        else
        {
            Debug.LogWarning("AudioManager instance not found!");
        }
    }
}
