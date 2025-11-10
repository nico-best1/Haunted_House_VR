using UnityEngine;

public class FinalMonsterTrigger : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject monster;
    public Transform monsterStartPos;
    public Transform playerHead; // Usa la cámara (Main Camera)
    public GameObject blackoutScreen;
    public AudioSource audioSource;
    public AudioClip jumpscareSound;
    public AudioClip creditsMusic;
    public TensionAudioController tenseAudio;

    public GameObject creditsManager;

    [Header("Parámetros")]
    public Transform monsterDirectionReference; // Punto hacia donde el jugador debe mirar (normalmente el monstruo)
    public float viewAngleThreshold = 60f;

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!FinalWallTrigger.wallEventTriggered || !other.CompareTag("Player") || hasTriggered)
            return;

        // Aparece el monstruo
        monster.transform.position = monsterStartPos.position;
        monster.SetActive(true);
        tenseAudio?.StopTension();

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

        if (audioSource && jumpscareSound)
        {
            audioSource.clip = jumpscareSound;
            audioSource.Play();
        }

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
        if (audioSource && creditsMusic)
        {
            audioSource.clip = creditsMusic;
            audioSource.loop = true;
            audioSource.Play();
        }

        if (creditsManager != null)
        {
            creditsManager.SetActive(true); // Activa el objeto para que el script Start() comience
        }
    }
}
