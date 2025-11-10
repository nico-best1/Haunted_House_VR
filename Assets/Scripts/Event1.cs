using Unity.XR.CoreUtils;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Event1 : MonoBehaviour
{
    public GameObject wallToDisappear;
    public GameObject door;
    public Transform playerHead; // Esto es la cámara del VR rig (HMD)
    public Transform wallDirectionReference; // Un punto enfrente de la pared, hacia donde debería mirar para verla
    public float maxViewAngle = 60f; // Cuánto puede desviarse la mirada para que NO la esté viendo
    public AudioClip doorClosed; // Sonido que se reproduce al desaparecer la pared
    public GameObject gramophone;

    private bool eventTriggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (eventTriggered) return;

        if (other.CompareTag("Player"))
        {
            StartCoroutine(CheckWhenNotLooking());
        }
    }

    private System.Collections.IEnumerator CheckWhenNotLooking()
    {
        eventTriggered = true;

        while (true)
        {
            Vector3 toWall = (wallDirectionReference.position - playerHead.position).normalized;
            float angle = Vector3.Angle(playerHead.forward, toWall);

            if (angle > maxViewAngle)
            {
                door.gameObject.transform.rotation = Quaternion.Euler(-90, 90, 180); // Resetea la rotación de la puerta
                door.GetComponent<Rigidbody>().isKinematic = true;
                door.GetComponent<XRGrabInteractable>().enabled = false; // Desactiva la puerta para que no se pueda abrir
                SoundManager.Instance.PlaySFX(doorClosed);
                gramophone.GetComponent<AudioSource>().Stop(); // Desactiva el gramófono 
                wallToDisappear.SetActive(false);
                break;
            }

            yield return null; 
        }
    }

    public bool HasEventTriggered()
    {
        return eventTriggered;
    }

}