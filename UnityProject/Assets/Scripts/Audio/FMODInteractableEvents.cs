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

    public void PlayFlashlightOnSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.flashlightOn);
        }
        else
        {
            Debug.LogWarning("AudioManager instance not found!");
        }
    }

    public void PlayFlashlightOffSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.flashlightOff);
        }
        else
        {
            Debug.LogWarning("AudioManager instance not found!");
        }
    }
}
