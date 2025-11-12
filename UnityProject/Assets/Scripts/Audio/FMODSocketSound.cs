using UnityEngine;
using FMODUnity;

public class FMODSocketSound : MonoBehaviour
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
}
