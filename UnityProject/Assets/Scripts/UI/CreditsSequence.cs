using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;


public class CreditsSequence : MonoBehaviour
{
    [Header("Créditos (en orden)")]
    public GameObject[] creditObjects; // Asigna aquí los GameObjects que contienen los textos (con CanvasGroup)
    public float fadeDuration = 1f;
    public float visibleDuration = 3f;
    public float delayBetweenTexts = 1f;
    public float delayBeforeCredits = 4f; // Tiempo en negro tras jumpscare

    private FinalMonsterTrigger finalTrigger;
    private FMOD.Studio.EventInstance musicInstance;

    private void Start()
    {
        finalTrigger = FindFirstObjectByType<FinalMonsterTrigger>();
        musicInstance = finalTrigger.GetCreditsMusicInstance();
        StartCoroutine(PlayCredits());
    }

    IEnumerator PlayCredits()
    {
        yield return new WaitForSeconds(delayBeforeCredits); // Espera en negro

        foreach (GameObject obj in creditObjects)
        {
            CanvasGroup canvasGroup = obj.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                Debug.LogWarning($"{obj.name} no tiene un CanvasGroup.");
                continue;
            }

            obj.SetActive(true);
            yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 0f, 1f));
            yield return new WaitForSeconds(visibleDuration);
            yield return StartCoroutine(FadeCanvasGroup(canvasGroup, 1f, 0f));
            obj.SetActive(false);
            yield return new WaitForSeconds(delayBetweenTexts);
        }
        yield return StartCoroutine(FadeOutMusic());
        // Reinicia la escena al final
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private IEnumerator FadeOutMusic()
    {
        float duration = 2f; // duración del fade
        float elapsed = 0f;
        float startValue = 1f; // valor inicial de intensity

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float intensity = Mathf.Lerp(startValue, 0f, t);

            musicInstance.setParameterByName("Intensity", intensity);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Asegurar que queda en 0
        musicInstance.setParameterByName("Intensity", 0f);
        musicInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }


    IEnumerator FadeCanvasGroup(CanvasGroup cg, float start, float end)
    {
        float elapsed = 0f;

        cg.alpha = start;
        cg.interactable = false;
        cg.blocksRaycasts = false;

        while (elapsed < fadeDuration)
        {
            cg.alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cg.alpha = end;
    }
}
