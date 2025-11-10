using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Event2 : MonoBehaviour
{
    public AudioClip StrongString;
    public List<Light> flickerLights;
    public float flickerDuration = 3f;
    public List<AudioClip> flickerSounds;
    public float flickerVolume = 1f;
    public float minFlickerInterval = 0.05f;
    public float maxFlickerInterval = 0.2f;

    public AudioSource curtainAudio;
    public AudioSource backgroundAudioSource;
    public float fadeInDuration = 3f;

    public Event1 previousEvent;
    public GhostMovement ghost;

    private bool hasTriggered = false;
    private List<Vector3> originalLightPositions = new List<Vector3>();


    void Start()
    {
        if (backgroundAudioSource != null)
        {
            backgroundAudioSource.volume = 0f;
            backgroundAudioSource.Play();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;

        if (other.CompareTag("Player") && previousEvent != null && previousEvent.HasEventTriggered())
        {
            hasTriggered = true;
            StartCoroutine(TriggerJumpscare());
        }
    }

    private IEnumerator TriggerJumpscare()
    {
        if (curtainAudio != null && curtainAudio.isPlaying)
            curtainAudio.Stop();

        if (StrongString != null)
            SoundManager.Instance.PlaySFX(StrongString);

        if (backgroundAudioSource != null)
        {
            StartCoroutine(FadeInBackgroundMusic());
        }

        StartCoroutine(FlickerLights());

        if (ghost != null)
            StartCoroutine(ghost.MoveAcross());

        yield return null;
    }

    private IEnumerator FadeInBackgroundMusic()
    {
        float timer = 0f;
        while (timer < fadeInDuration)
        {
            timer += Time.deltaTime;
            backgroundAudioSource.volume = Mathf.Lerp(0f, 1f, timer / fadeInDuration);
            yield return null;
        }
        backgroundAudioSource.volume = 1f;
    }

    private IEnumerator FlickerLights()
    {
        // Guardamos posiciones originales si aún no están almacenadas
        if (originalLightPositions.Count == 0)
        {
            foreach (Light light in flickerLights)
                originalLightPositions.Add(light.transform.position);
        }

        float timer = 0f;
        float soundCooldown = 0f;

        while (timer < flickerDuration)
        {
            // Selecciona una luz aleatoria
            int index = Random.Range(0, flickerLights.Count);
            Light light = flickerLights[index];

            if (light != null)
            {
                // Mueve la luz fuera del pasillo
                light.transform.position += Vector3.right * 100f;

                // Sonido
                if (flickerSounds != null && flickerSounds.Count > 0 && soundCooldown <= 0f)
                {
                    AudioClip randomClip = flickerSounds[Random.Range(0, flickerSounds.Count)];
                    AudioPool.Instance.PlaySound(randomClip, light.transform.position, flickerVolume);
                    soundCooldown = 0.2f;
                }

                // Espera un tiempo aleatorio
                float waitTime = Random.Range(minFlickerInterval, maxFlickerInterval);
                yield return new WaitForSeconds(waitTime);
                timer += waitTime;
                soundCooldown -= waitTime;

                // Regresa la luz a su posición original
                light.transform.position = originalLightPositions[index];
            }
        }
    }
}
