using UnityEngine;

public class WindAudioController : MonoBehaviour
{           
    private FMOD.Studio.EventInstance windInstance;

    [Header("FADE SETTINGS ---------------")]
    public float fadeDuration = 3f;       // Tiempo del Fade In/Out
    private float fadeValue = 0f;         // Se usará en un parámetro en FMOD
    private const string paramName = "WindIntensity"; // Nombre del parámetro de FMOD

    private bool isFading = false;
    private bool isPlaying = false;
    public GameObject windSource; 

    void Start()
    {
        // Creamos instancia y la hacemos 3D
        windInstance = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.Instance.wind);

        // Adjuntar a objeto para 3D
        FMODUnity.RuntimeManager.AttachInstanceToGameObject(
            windInstance,
            windSource,
            GetComponent<Rigidbody>()
        );

        windInstance.start();
        windInstance.setParameterByName(paramName, 0f);
        isPlaying = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            FadeIn();        
        }
    }

    // ===========================================================
    //      MÉTODOS PÚBLICOS PARA CONTROLAR EL AUDIO
    // ===========================================================

    public void FadeIn()
    {
        if (isFading || !isPlaying) return;
        StartCoroutine(FadeAudio(1f));
    }

    public void FadeOut()
    {
        if (isFading || !isPlaying) return;
        StartCoroutine(FadeAudio(0f));
    }

    // Cerrar y liberar cuando ya no se use
    public void StopAndRelease()
    {
        windInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        windInstance.release();
        isPlaying = false;
    }


    // ===========================================================
    //              LÓGICA DE FADE
    // ===========================================================

    private System.Collections.IEnumerator FadeAudio(float target)
    {
        isFading = true;
        float startValue = fadeValue;
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            fadeValue = Mathf.Lerp(startValue, target, timer / fadeDuration);
            windInstance.setParameterByName(paramName, fadeValue);
            yield return null;
        }

        fadeValue = target;
        windInstance.setParameterByName(paramName, fadeValue);
        isFading = false;
    }


    void OnDestroy()
    {
        StopAndRelease();
    }
}
