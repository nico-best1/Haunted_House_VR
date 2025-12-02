using UnityEngine;
using FMODUnity;

public class DoorCreakOneShotsVR : MonoBehaviour
{

    [Tooltip("Velocidad mínima de giro para que suene (grados/segundo)")]
    public float creakThreshold = 15f;

    [Tooltip("Tiempo mínimo entre chirridos para evitar spam")]
    public float cooldown = 0.25f;

    private float previousAngle;
    private float lastTimePlayed;

    void Start()
    {
        previousAngle = transform.localEulerAngles.x;
    }

    void Update()
    {
        float currentAngle = transform.localEulerAngles.y;

        float delta = Mathf.Abs(Mathf.DeltaAngle(previousAngle, currentAngle));
        float speed = delta / Time.deltaTime; 

        if (speed > creakThreshold && Time.time - lastTimePlayed > cooldown)
        {
            RuntimeManager.PlayOneShot(FMODEvents.Instance.doorCreak, transform.position);
            lastTimePlayed = Time.time;
        }

        previousAngle = currentAngle;
    }
}
