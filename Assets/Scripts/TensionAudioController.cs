using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TensionAudioController : MonoBehaviour
{
    public AudioSource tensionAudio;
    public float maxVolume = 1f;
    public float fadeSpeed = 0.5f;

    private bool isFadingIn = false;

    void Start()
    {
        if (tensionAudio != null)
        {
            tensionAudio.volume = 0f;
            tensionAudio.loop = true;
        }
    }

    void Update()
    {
        if (isFadingIn && tensionAudio.volume < maxVolume)
        {
            tensionAudio.volume += fadeSpeed * Time.deltaTime;
            tensionAudio.volume = Mathf.Min(tensionAudio.volume, maxVolume);
        }
    }

    public void StartTension()
    {
        if (tensionAudio != null)
        {
            tensionAudio.Play();
            isFadingIn = true;
        }
    }

    public void StopTension()
    {
        if (tensionAudio != null)
        {
            tensionAudio.Stop();
            isFadingIn = false;
        }
    }
}
