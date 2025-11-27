using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PendulumRoomEvent_FMOD : MonoBehaviour
{
    public GameObject entryDoor;

    public GameObject exitDoor;
    public GameObject exitWall;

    public GameObject pendulum;

    public Transform playerHead;

    // CAJONES
    public List<Transform> drawers;
    public float drawerMoveDistance = 0.2f;
    public float drawerMoveSpeed = 2f;
    private List<Vector3> drawerOriginalPositions = new List<Vector3>();

    // LUCES
    public List<Light> flickerLights;
    public float flickerDuration = 3f;
    public float minFlickerInterval = 0.05f;
    public float maxFlickerInterval = 0.2f;

    // EVENTOS FMOD ------------------------
    private FMOD.Studio.EventInstance clockSound;       // LOOP
    private FMOD.Studio.EventInstance buildUpTension;   // Solo una vez

    public float eventDuration = 20f; // duración total del evento

    private bool eventStarted = false;
    private bool exitDoorRevealed = false;
    private bool eventFinished = false;

    private Quaternion pendulumStartRot;
    private Vector3 originalHeadPosition;
    // -------------------------------------

    void Start()
    {
        // CLOCK único desde el comienzo del juego
        // Crear la instancia
        clockSound = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.Instance.clockSound);

        // Configurar parámetro inicial
        clockSound.setParameterByName("ClockReverb", 0);

        // Asignar 3D attributes (posición y orientación)
        if (pendulum != null)
        {
            FMODUnity.RuntimeManager.AttachInstanceToGameObject(clockSound, pendulum, pendulum.GetComponent<Rigidbody>());
        }

        // Iniciar el sonido
        clockSound.start();


        // Pasar la misma instancia al péndulo
        PendulumIdleController pc = pendulum.GetComponent<PendulumIdleController>();
        if (pc != null)
            pc.AssignClockInstance(clockSound);

        buildUpTension = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.Instance.buildUpTension);

        pendulumStartRot = pendulum.transform.localRotation;
        if (exitDoor != null) exitDoor.SetActive(false);
        if (exitWall != null) exitWall.SetActive(true);
        if (playerHead != null) originalHeadPosition = playerHead.localPosition;

        foreach (Transform d in drawers) drawerOriginalPositions.Add(d.localPosition);
    }

    void OnTriggerEnter(Collider other)
    {
        if (eventStarted) return;
        if (!other.CompareTag("Player")) return;

        eventStarted = true;
        StartCoroutine(EventSequence());
    }


    // ============================================================
    //  SECUENCIA PRINCIPAL
    // ============================================================
    private IEnumerator EventSequence()
    {
        PendulumIdleController pc = pendulum.GetComponent<PendulumIdleController>();

        if (entryDoor != null)
        {
            entryDoor.transform.rotation = Quaternion.Euler(-90, 0, 180);
            entryDoor.GetComponent<Rigidbody>().isKinematic = true;
            entryDoor.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().enabled = false;
            AudioManager.Instance.PlayOneShotPosition(FMODEvents.Instance.doorClosedSound, entryDoor.transform.position);
        }

        yield return new WaitForSeconds(12f);

        // Solo manejamos el reverb del reloj
        StartCoroutine(IncreaseClockReverb());

        StartCoroutine(MoveDrawers());
        StartCoroutine(FlickerLights());

        yield return new WaitForSeconds(eventDuration);

        buildUpTension.start();
        yield return new WaitForSeconds(5f);

        StopAllSounds();
        eventFinished = true;

        pendulum.transform.localRotation = pendulumStartRot;
        playerHead.localPosition = originalHeadPosition;

        if (flickerLights.Count > 0)
            flickerLights[0].enabled = true;

        yield return new WaitForSeconds(2f);
        yield return new WaitUntil(() => !IsPlayerLookingAt(pendulum.transform.position));
        StartCoroutine(RevealExitWhenNotLooking());
    }


    // Cerrar todo cuando acabe BuildUp
    void StopAllSounds()
    {
        buildUpTension.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        buildUpTension.release();
        clockSound.setParameterByName("ClockReverb", 0);
    }


    // ============================================================
    //  CLOCK — aumenta reverb durante el evento
    // ============================================================
    IEnumerator IncreaseClockReverb()
    {
        float t = 0f;
        float maxReverb = 1f;
        float riseDuration = eventDuration * 0.7f; 

        while (t < eventDuration)
        {
            t += Time.deltaTime;

            float value;
            if (t < riseDuration)
            {
                // Subida rápida a 1
                value = Mathf.Clamp01(t / riseDuration);
            }
            else
            {
                // Mantener en 1 el resto del evento
                value = maxReverb;
            }

            clockSound.setParameterByName("ClockReverb", value);

            yield return null;
        }
    }



    // ============================================================
    //  DRAWERS — con sonido OneShot repartido por la habitación
    // ============================================================
    IEnumerator MoveDrawers()
    {
        while (!eventFinished)
        {
            for (int i = 0; i < drawers.Count; i++)
                StartCoroutine(OpenCloseDrawer(drawers[i], drawerOriginalPositions[i]));

            yield return new WaitForSeconds(Random.Range(1f, 2f));
        }
    }

    IEnumerator OpenCloseDrawer(Transform drawer, Vector3 originalPos)
    {
        Vector3 openPos = originalPos + drawer.forward * drawerMoveDistance;
        yield return MoveTo(drawer, openPos);
        PlayDrawerSoundAt(drawer.position);

        yield return MoveTo(drawer, originalPos);
        PlayDrawerSoundAt(drawer.position);
    }

    void PlayDrawerSoundAt(Vector3 pos)
    {
        AudioManager.Instance.PlayOneShotPosition(FMODEvents.Instance.drawerSound, pos);
    }

    IEnumerator MoveTo(Transform obj, Vector3 target)
    {
        while (Vector3.Distance(obj.localPosition, target) > 0.001f)
        {
            obj.localPosition = Vector3.MoveTowards(obj.localPosition, target, drawerMoveSpeed * Time.deltaTime);
            yield return null;
        }
    }


    IEnumerator FlickerLights()
    {
        if (flickerLights.Count == 0) yield break;

        Light light = flickerLights[0];  // Tomamos la única luz

        float timer = 0f;
        bool wasOn = true; // Para detectar cambios de estado

        while (timer < flickerDuration)
        {
            if (light != null)
            {
                bool newState = Random.value > 0.5f;
                light.enabled = newState;

                // Si antes estaba encendida y ahora se apaga, reproducimos sonido
                if (wasOn && !newState)
                {
                    AudioManager.Instance.PlayOneShotPosition(FMODEvents.Instance.flickerLights, light.transform.position);
                }

                wasOn = newState;
            }

            float wait = Random.Range(minFlickerInterval, maxFlickerInterval);
            yield return new WaitForSeconds(wait);
            timer += wait;
        }

        if (light != null)
            light.enabled = true;  // Aseguramos que termine encendida
    }



    // ============================================================
    // SALIDA
    // ============================================================
    bool IsPlayerLookingAt(Vector3 target)
    {
        Vector3 dir = (target - playerHead.position).normalized;
        return Vector3.Dot(playerHead.forward, dir) > 0.7f;
    }

    IEnumerator RevealExitWhenNotLooking()
    {
        if (exitDoorRevealed) yield break;
        exitDoorRevealed = true;

        while (true)
        {
            float angle = Vector3.Angle(playerHead.forward, exitWall.transform.position - playerHead.position);
            if (angle > 60f)
            {
                exitWall.SetActive(false);
                exitDoor.SetActive(true);
                break;
            }
            yield return null;
        }
    }
}
