using UnityEngine;
using FMOD.Studio;

public class AmbienceController : MonoBehaviour
{
    [Header("Fade Durations")]
    public float fadeInDuration = 3f;
    public float fadeOutDuration = 2f;

    private EventInstance ambienceInstance;
    private bool isFading = false;
    public Transform player; // referencia al jugador VR

    void Start()
    {
        ambienceInstance = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.Instance.ambience);
        ambienceInstance.start();

        // Comienza silencioso
        ambienceInstance.setParameterByName("ambienceIntensity", 0f);
    }

    void Update()
    {
        // Actualiza la posición 3D del evento para que el centro del radio sea el jugador
        if (ambienceInstance.isValid() && player != null)
        {
            ambienceInstance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(player.position));
        }
    }

    public void FadeIn()
    {
        if (ambienceInstance.isValid())
        {
            ambienceInstance.setParameterByName("CreepySoundChance", 1f); // activarlos inmediatamente
            if (!isFading) StartCoroutine(FadeRoutine(1f, fadeInDuration));
        }
    }

    public void FadeOut()
    {
        if (ambienceInstance.isValid())
        {
            ambienceInstance.setParameterByName("CreepySoundChance", 0f); // desactivarlos inmediatamente
            if (!isFading) StartCoroutine(FadeRoutine(0f, fadeOutDuration));
        }
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
