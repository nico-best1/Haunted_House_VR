using FMOD.Studio;
using UnityEngine;

public class FinalMonsterTrigger : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject monster;
    public Transform monsterStartPos;
    public Transform playerHead; 
    public GameObject blackoutScreen;
    private FMOD.Studio.EventInstance creditsMusicInstance;
    public TensionAudioController tensionAudioController;

    public GameObject creditsManager;

    [Header("Parámetros")]
    public Transform monsterDirectionReference; // Punto hacia donde el jugador debe mirar 
    public float viewAngleThreshold = 60f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!FinalWallTrigger.wallEventTriggered || !other.CompareTag("Player") || hasTriggered)
            return;

        // Aparece el monstruo
        monster.transform.position = monsterStartPos.position;
        monster.SetActive(true);
        tensionAudioController.FadeOutTension();

        // Empieza a revisar si lo está mirando
        StartCoroutine(CheckIfLookingAtMonster());

        hasTriggered = true;
    }

    private System.Collections.IEnumerator CheckIfLookingAtMonster()
    {
        yield return new WaitForSeconds(0.5f); // Pequeño delay inicial

        while (true)
        {
            Vector3 toTarget = (monsterDirectionReference.position - playerHead.position).normalized;
            float angle = Vector3.Angle(playerHead.forward, toTarget);

            if (angle < viewAngleThreshold)
            {
                yield return new WaitForSeconds(1f); // Espera para que el jugador se gire completamente
                TriggerJumpscare();
                yield break;
            }

            yield return null;
        }
    }


    private void TriggerJumpscare()
    {
        // El monstruo se pone delante del jugador
        monster.transform.position = playerHead.position + playerHead.forward * 1.5f;
        monster.transform.LookAt(playerHead);

        AudioManager.Instance.PlayOneShot(FMODEvents.Instance.finalJumpscare);

        Invoke(nameof(FadeToBlack), 0.4f);
        Invoke(nameof(PlayCreditsMusic), 2.5f);
    }

    private void FadeToBlack()
    {
        if (blackoutScreen != null)
            blackoutScreen.SetActive(true);
    }

    private void PlayCreditsMusic()
    {
        creditsMusicInstance = AudioManager.Instance.CreateInstance(FMODEvents.Instance.creditsMusic);
        creditsMusicInstance.start();
        creditsMusicInstance.setParameterByName("Intensity", 1);

        if (creditsManager != null)
        {
            creditsManager.SetActive(true); // Activa el objeto para que el script Start() comience
        }
    }

    public EventInstance GetCreditsMusicInstance()
    {
        return creditsMusicInstance;
    }
}
