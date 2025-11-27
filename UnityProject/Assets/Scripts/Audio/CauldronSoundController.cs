using FMOD.Studio;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CauldronSoundController : MonoBehaviour
{
    [Header("FMOD PARAMETER NAME")]
    public string parameterName = "CauldronIntensity";

    [Header("Fade Durations")]
    public float fadeInDuration = 3f;
    public float fadeOutDuration = 1f;

    [Header("POSICIONES DONDE SE CREARÁN LOS HERVIDORES")]
    public GameObject[] cauldronsObjects;  // <= arrastra aquí tus 4 posiciones

    private List<EventInstance> cauldrons = new List<EventInstance>();
    private bool isFading = false;

    void Start()
    {
        // Crear una instancia por cada posición que tengas asignada
        foreach (GameObject gameobject in cauldronsObjects)
        {
            EventInstance instance = AudioManager.Instance.CreateInstance(FMODEvents.Instance.cauldronBoiling);
            instance.setParameterByName(parameterName, 0f);

            // Lo fijamos a esa posición 3D del mundo
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(instance, gameobject, gameobject.GetComponent<Rigidbody>());

            cauldrons.Add(instance);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            FadeInAll();
    }

    // ==================== 🔊 FADE IN (0 → 1) PARA TODAS ====================
    public void FadeInAll()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        if (isFading) yield break;
        isFading = true;

        foreach (var c in cauldrons) c.start();

        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            float v = Mathf.Clamp01(t / fadeInDuration);

            foreach (var c in cauldrons)
                c.setParameterByName(parameterName, v);

            yield return null;
        }

        foreach (var c in cauldrons)
            c.setParameterByName(parameterName, 1f);

        isFading = false;
    }

    // ==================== 🔇 FADE OUT (1 → 0) PARA TODAS ====================
    public void FadeOutAll()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        isFading = true;

        // Cogemos el valor actual del primero (todos serán iguales)
        cauldrons[0].getParameterByName(parameterName, out float startValue);

        float t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            float v = Mathf.Lerp(startValue, 0f, t / fadeOutDuration);

            foreach (var c in cauldrons)
                c.setParameterByName(parameterName, v);

            yield return null;
        }

        foreach (var c in cauldrons)
        {
            c.setParameterByName(parameterName, 0f);
            c.stop(STOP_MODE.ALLOWFADEOUT);
            c.release();
        }

        isFading = false;
    }
}
