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

    private void Start()
    {
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

        // Reinicia la escena al final
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
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
