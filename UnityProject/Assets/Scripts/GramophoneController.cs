using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GramophoneController : MonoBehaviour
{
    public AudioSource audioSource;        // Solo un AudioSource
    public AudioClip startSoundClip;        // El sonido de arranque
    public AudioClip musicClip;             // La canción en bucle

    void Start()
    {
        // Configurar el primer sonido (sin loop)
        audioSource.clip = startSoundClip;
        audioSource.loop = false;
        audioSource.Play();

        // Cuando termine el sonido de arranque, empezar la música
        Invoke(nameof(StartMusic), startSoundClip.length);
    }

    void StartMusic()
    {
        // Configurar la música (en loop)
        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.Play();
    }
}
