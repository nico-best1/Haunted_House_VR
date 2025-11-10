using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PendulumRoomEvent : MonoBehaviour
{
    public GameObject entryDoor;
    public AudioSource entryDoorAudio;

    public GameObject exitDoor;
    public GameObject exitWall;

    public GameObject pendulum;
    public AudioSource clockAudio;

    public List<Light> flickerLights;
    public float flickerDuration = 3f;
    public List<AudioClip> flickerSounds;  // Agregado para los sonidos de parpadeo
    public float flickerVolume = 1f;
    public float minFlickerInterval = 0.05f;
    public float maxFlickerInterval = 0.2f;

    public AudioSource flickerAudio;  // Si aún quieres un audio extra para el parpadeo

    public Transform playerHead;

    [Header("Drawers")]
    public List<Transform> drawers;
    public float drawerMoveDistance = 0.2f;
    public float drawerMoveSpeed = 2f;
    private List<Vector3> drawerOriginalPositions = new List<Vector3>();

    [Header("Drawer Sounds")]
    public List<AudioSource> drawerHitSounds;

    [Header("Ambient Event Sounds")]
    public AudioSource buildUpTension;

    private bool eventStarted = false;
    private bool exitDoorRevealed = false;
    private bool eventFinished = false;  // Bandera para indicar si el evento ha terminado

    private Quaternion pendulumStartRot;
    private Vector3 originalHeadPosition;

    void Start()
    {
        if (pendulum != null)
            pendulumStartRot = pendulum.transform.localRotation;

        if (exitDoor != null)
            exitDoor.SetActive(false);

        if (exitWall != null)
            exitWall.SetActive(true);

        if (clockAudio != null)
        {
            clockAudio.Stop();
            clockAudio.volume = 0f;
        }

        if (playerHead != null)
            originalHeadPosition = playerHead.localPosition;

        foreach (Transform drawer in drawers)
        {
            if (drawer != null)
                drawerOriginalPositions.Add(drawer.localPosition);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (eventStarted) return;
        if (!other.CompareTag("Player")) return;

        eventStarted = true;
        StartCoroutine(EventSequence());
    }

    private IEnumerator EventSequence()
    {
        PendulumIdleController idleController = pendulum.GetComponent<PendulumIdleController>();
        if (idleController != null)
            idleController.enabled = true;

        if (entryDoor != null)
        {
            entryDoor.transform.rotation = Quaternion.Euler(-90, 0, 180);
            entryDoor.GetComponent<Rigidbody>().isKinematic = true;
            entryDoor.GetComponent<XRGrabInteractable>().enabled = false;
        }

        if (entryDoorAudio != null)
            entryDoorAudio.Play();

        yield return new WaitForSeconds(12f);

        idleController.StopPendulum();

        if (clockAudio != null)
        {
            clockAudio.Play();
            clockAudio.volume = 0f;
        }

        StartCoroutine(MoveDrawers());

        float duration = 20f;
        StartCoroutine(FlickerLights()); // Llamamos a la nueva lógica de parpadeo

        float buildUpStartTime = duration - buildUpTension.clip.length;
        StartCoroutine(PlayBuildUpAfterDelay(buildUpStartTime));

        float timer = 0f;
        while (timer < duration)
        {
            float normalizedTime = timer / duration;
            float angle = Mathf.Sin(Time.time * Mathf.Lerp(2f, 6f, normalizedTime)) * Mathf.Lerp(5f, 15f, normalizedTime);

            if (pendulum != null)
                pendulum.transform.localRotation = pendulumStartRot * Quaternion.Euler(0, 0, angle);

            if (clockAudio != null)
            {
                clockAudio.pitch = Mathf.Lerp(1f, 2f, normalizedTime);
                clockAudio.volume = Mathf.Lerp(0.2f, 1f, normalizedTime);
            }

            if (playerHead != null)
            {
                float shakeAmount = Mathf.Lerp(0f, 0.02f, normalizedTime);
                Vector3 offset = Random.insideUnitSphere * shakeAmount;
                playerHead.localPosition = originalHeadPosition + new Vector3(offset.x, offset.y, 0);
            }

            timer += Time.deltaTime;
            yield return null;
        }

        eventFinished = true;  // Marcar como terminado el evento

        if (clockAudio != null)
        {
            clockAudio.Stop();
            clockAudio.volume = 1f;
        }

        if (pendulum != null)
            pendulum.transform.localRotation = pendulumStartRot;

        if (playerHead != null)
            playerHead.localPosition = originalHeadPosition;

        foreach (var light in flickerLights)
        {
            if (light != null)
                light.enabled = true;
        }

        yield return new WaitForSeconds(2f);
        yield return new WaitUntil(() => !IsPlayerLookingAt(pendulum.transform.position));
        StartCoroutine(RevealExitWhenNotLooking());
    }

    private IEnumerator MoveDrawers()
    {
        while (!eventFinished)
        {
            List<IEnumerator> activeMovements = new List<IEnumerator>();

            for (int i = 0; i < drawers.Count; i++)
            {
                if (drawers[i] == null) continue;
                StartCoroutine(OpenCloseDrawer(drawers[i], drawerOriginalPositions[i]));
            }


            yield return new WaitForSeconds(Random.Range(1f, 2f));  // Pausa más larga para evitar saturación
        }
    }


    private IEnumerator OpenCloseDrawer(Transform drawer, Vector3 originalPosition)
    {
        Vector3 openPos = originalPosition + drawer.forward * drawerMoveDistance;

        float speed = drawerMoveSpeed; // Un valor constante para la velocidad.

        yield return StartCoroutine(MoveTo(drawer, openPos, speed));
        PlayRandomDrawerSound();

        yield return StartCoroutine(MoveTo(drawer, originalPosition, speed));
        PlayRandomDrawerSound();
    }

    private void PlayRandomDrawerSound()
    {
        if (drawerHitSounds.Count == 0) return;

        AudioSource randomSound = drawerHitSounds[Random.Range(0, drawerHitSounds.Count)];
        if (randomSound != null)
            randomSound.Play();
    }

    private IEnumerator MoveTo(Transform obj, Vector3 target, float speed)
    {
        while (Vector3.Distance(obj.localPosition, target) > 0.001f)
        {
            obj.localPosition = Vector3.MoveTowards(obj.localPosition, target, speed * Time.deltaTime);
            yield return null;
        }
    }

    private bool IsPlayerLookingAt(Vector3 target)
    {
        Vector3 dirToTarget = (target - playerHead.position).normalized;
        float dot = Vector3.Dot(playerHead.forward, dirToTarget);
        return dot > 0.7f;
    }

    private IEnumerator RevealExitWhenNotLooking()
    {
        if (exitDoorRevealed) yield break;
        exitDoorRevealed = true;

        Vector3 toWall = (exitWall.transform.position - playerHead.position).normalized;
        float maxViewAngle = 60f;

        while (true)
        {
            float angle = Vector3.Angle(playerHead.forward, toWall);
            if (angle > maxViewAngle)
            {
                // Desactiva la pared y activa la puerta
                if (exitWall != null)
                    exitWall.SetActive(false);

                if (exitDoor != null)
                {
                    exitDoor.SetActive(true);
                    exitDoor.transform.rotation = Quaternion.Euler(-90, 90, 180); // Ajusta si es necesario
                    var rb = exitDoor.GetComponent<Rigidbody>();
                    if (rb != null) rb.isKinematic = true;

                    var grab = exitDoor.GetComponent<XRGrabInteractable>();
                    if (grab != null) grab.enabled = false;
                }
                pendulum.GetComponent<PendulumIdleController>()?.StartPendulum();
                break;
            }

            yield return null;
        }
    }

    private IEnumerator FlickerLights()
    {
        List<Coroutine> flickerCoroutines = new List<Coroutine>();

        foreach (Light light in flickerLights)
        {
            if (light != null)
            {
                Coroutine flickerCoroutine = StartCoroutine(FlickerSingleLight(light));
                flickerCoroutines.Add(flickerCoroutine);
            }
        }

        yield return new WaitForSeconds(flickerDuration);

        foreach (Light light in flickerLights)
        {
            if (light != null)
                light.enabled = true;
        }
    }

    private IEnumerator FlickerSingleLight(Light light)
    {
        float timer = 0f;

        while (timer < flickerDuration)
        {
            light.enabled = Random.value > 0.5f;

            if (flickerSounds != null && flickerSounds.Count > 0)
            {
                AudioClip randomClip = flickerSounds[Random.Range(0, flickerSounds.Count)];
                AudioSource.PlayClipAtPoint(randomClip, light.transform.position, flickerVolume);
            }

            float waitTime = Random.Range(minFlickerInterval, maxFlickerInterval);
            yield return new WaitForSeconds(waitTime);
            timer += waitTime;
        }

        light.enabled = true;
    }

    private IEnumerator PlayBuildUpAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (buildUpTension != null)
            buildUpTension.Play();
    }
}
