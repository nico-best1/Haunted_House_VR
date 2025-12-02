using UnityEngine;
using FMODUnity;
using FMOD.Studio;
using System.Collections;

public class TensionAudioController : MonoBehaviour
{
    private EventInstance tension;
    public AmbienceController ambienceController;
    public CreepySoundsController creepySoundsController;
    public CauldronSoundController cauldronSoundController;
    public EventInstance breathEvent;
    [Header("FMOD PARAMETER NAME")]
    public string parameterName = "TensionIntensity";

    [Header("Fade Durations")]
    public float fadeInDuration = 3f;
    public float fadeOutDuration = 1f;

    private bool isFading = false;

    void Start()
    {
        tension = AudioManager.Instance.CreateInstance(FMODEvents.Instance.tension);
        tension.setParameterByName(parameterName, 0f); 
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FadeInTension();
            breathEvent = AudioManager.Instance.CreateInstance(FMODEvents.Instance.breath);
            breathEvent.start();
            ambienceController.FadeOut();
            creepySoundsController.StopCreepySounds();
            cauldronSoundController.FadeOut();  
        }
    }

    public void FadeInTension()
    {
        StopAllCoroutines();
        StartCoroutine(FadeInRoutine());
    }

    IEnumerator FadeInRoutine()
    {
        if (isFading) yield break;
        isFading = true;

        tension.start(); // solo se dispara una vez

        float t = 0f;
        while (t < fadeInDuration)
        {
            t += Time.deltaTime;
            float value = Mathf.Clamp01(t / fadeInDuration);
            tension.setParameterByName(parameterName, value);
            yield return null;
        }

        tension.setParameterByName(parameterName, 1f);
        isFading = false;
    }

    public void FadeOutTension()
    {
        StopAllCoroutines();
        StartCoroutine(FadeOutRoutine());
    }

    IEnumerator FadeOutRoutine()
    {
        isFading = true;

        float currentValue;
        tension.getParameterByName(parameterName, out currentValue);

        float t = 0f;
        while (t < fadeOutDuration)
        {
            t += Time.deltaTime;
            float value = Mathf.Lerp(currentValue, 0f, t / fadeOutDuration);
            tension.setParameterByName(parameterName, value);
            yield return null;
        }

        tension.setParameterByName(parameterName, 0f);
        tension.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        tension.release();

        breathEvent.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        breathEvent.release();

        isFading = false;
    }
}
