using UnityEngine;
using FMOD.Studio;

public class AmbienceController : MonoBehaviour
{
    [Header("Fade Durations")]
    public float fadeInDuration = 3f;
    public float fadeOutDuration = 2f;

    private EventInstance ambienceInstance;
    private bool isFading = false;

    void Start()
    {
        ambienceInstance = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.Instance.ambience);
        ambienceInstance.start();

        // Comienza silencioso
        ambienceInstance.setParameterByName("ambienceIntensity", 0f);
    }

    // ------------ FADE IN (sube hasta 1 desde donde esté) ------------
    public void FadeIn()
    {
        if (!isFading) StartCoroutine(FadeRoutine(1f, fadeInDuration));
    }

    // ------------ FADE OUT (baja hasta 0 desde donde esté) ------------
    public void FadeOut()
    {
        if (!isFading) StartCoroutine(FadeRoutine(0f, fadeOutDuration));
    }

    // ------------ CONTROL GENERAL DE FADE ------------
    private System.Collections.IEnumerator FadeRoutine(float target, float duration)
    {
        isFading = true;

        ambienceInstance.getParameterByName("ambienceIntensity", out float current);
        float start = current;
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(start, target, t / duration);

            ambienceInstance.setParameterByName("ambienceIntensity", v);
            yield return null;
        }

        ambienceInstance.setParameterByName("ambienceIntensity", target);
        isFading = false;
    }

    void OnDestroy()
    {
        ambienceInstance.stop(STOP_MODE.ALLOWFADEOUT);
        ambienceInstance.release();
    }
}
