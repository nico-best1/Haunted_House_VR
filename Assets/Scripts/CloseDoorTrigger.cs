using UnityEngine;


public class CloseDoorTrigger : MonoBehaviour
{
    [Header("Door Settings")]
    public GameObject door;
    public GameObject pendulum;
    public AudioSource slamAudio;
    public float r; 

    private bool hasTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (hasTriggered) return;
        if (!other.CompareTag("Player")) return;

        hasTriggered = true;

        PendulumIdleController idleController = pendulum.GetComponent<PendulumIdleController>();
        if (idleController != null)
            idleController.StopPendulum();


        CloseDoor();
    }

    private void CloseDoor()
    {
        if (door != null)
        {
            door.transform.rotation = Quaternion.Euler(-90, r, 180);

            Rigidbody rb = door.GetComponent<Rigidbody>();
            if (rb != null)
                rb.isKinematic = true;

            UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = door.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grab != null)
                grab.enabled = false;
        }

        if (slamAudio != null)
        {
            slamAudio.Play();
        }
    }
}
