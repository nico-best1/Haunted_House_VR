using System.Collections.Generic;
using UnityEngine;

public class AudioPool : MonoBehaviour
{
    public static AudioPool Instance;

    [Header("Configuración")]
    public int poolSize = 10;
    public AudioSource audioSourcePrefab;

    private List<AudioSource> pool;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        pool = new List<AudioSource>();

        for (int i = 0; i < poolSize; i++)
        {
            AudioSource src = Instantiate(audioSourcePrefab, transform);
            src.playOnAwake = false;
            pool.Add(src);
        }
    }

    public void PlaySound(AudioClip clip, Vector3 position, float volume = 1f)
    {
        foreach (AudioSource src in pool)
        {
            if (!src.isPlaying)
            {
                src.transform.position = position;
                src.clip = clip;
                src.volume = volume;
                src.Play();
                return;
            }
        }
    }
}
