using FMOD.Studio;
using UnityEngine;
using System.Collections;

public class CauldronSoundController : MonoBehaviour
{
    [Header("FMOD PARAMETER NAME")]
    public string parameterName = "CauldronIntensity";

    [Header("Fade Durations")]
    public float fadeInDuration = 3f;
    public float fadeOutDuration = 1f;

    [Header("POSICIÓN DONDE SE CREARÁ EL HERVIDOR")]
    public GameObject cauldronObject;  // <= arrastra aquí la posición única

    private EventInstance cauldron;
    private bool isFading = false;

    void Start()
    {
        // Crear la instancia única
        cauldron = AudioManager.Instance.CreateInstance(FMODEvents.Instance.cauldronBoiling);
        cauldron.setParameterByName(parameterName, 0f);

        // Fijar a la posición 3D del mundo
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(cauldron, cauldronObject, cauldronObject.GetComponent<Rigidbody>());
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            FadeIn();
    }

    // ==================== 🔊 FADE IN (0 → 1) ====================
    public void FadeIn()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        if (isFading) yield break;
        isFading = true;

        cauldron.start();

        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            float v = Mathf.Clamp01(t / fadeInDuration);

            cauldron.setParameterByName(parameterName, v);

            yield return null;
        }

        cauldron.setParameterByName(parameterName, 1f);
        isFading = false;
    }

    // ==================== 🔇 FADE OUT (1 → 0) ====================
    public void FadeOut()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        isFading = true;

        cauldron.getParameterByName(parameterName, out float startValue);

        float t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(startValue, 0f, t / fadeOutDuration);

            cauldron.setParameterByName(parameterName, v);
            yield return null;
        }

        cauldron.setParameterByName(parameterName, 0f);
        cauldron.stop(STOP_MODE.ALLOWFADEOUT);
        cauldron.release();

        isFading = false;
    }
}
