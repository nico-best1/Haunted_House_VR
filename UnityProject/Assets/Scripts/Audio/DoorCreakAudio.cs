using UnityEngine;
using FMODUnity;
using FMOD.Studio;

public class DoorCreakFMOD : MonoBehaviour
{
    public string intensityParam = "DoorCreakVelocity";
    public GameObject door;                         // el pivot real de la puerta
    public float movementThreshold = 0.2f;         // sensibilidad
    public float intensityMultiplier = 4f;         // cuanto sube el chirrido

    private EventInstance creakInstance;
    private float lastAngle;

    void Start()
    {
        creakInstance = AudioManager.Instance.CreateInstance(FMODEvents.Instance.doorCreak);
        RuntimeManager.AttachInstanceToGameObject(creakInstance, door, door.GetComponent<Rigidbody>());
        lastAngle = door.transform.localEulerAngles.y;
    }

    void Update()
    {
        float currentAngle = door.transform.localEulerAngles.y;
        float angularSpeed = Mathf.Abs(Mathf.DeltaAngle(lastAngle, currentAngle)) / Time.deltaTime;

        // Normalizamos la velocidad para FMOD
        float intensity = Mathf.Clamp01(angularSpeed * intensityMultiplier);

        if (intensity > movementThreshold)
        {
            creakInstance.start(); // reproduce solo si se está moviendo
            creakInstance.setParameterByName(intensityParam, intensity);
        }
        else
        {
            creakInstance.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        }

        lastAngle = currentAngle;
    }

    void OnDestroy()
    {
        creakInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        creakInstance.release();
    }
}
