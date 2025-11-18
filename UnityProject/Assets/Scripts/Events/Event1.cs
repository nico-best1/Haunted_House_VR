using Unity.XR.CoreUtils;
using UnityEngine;


public class Event1 : MonoBehaviour
{
    public GameObject wallToDisappear;
    public GameObject door;
    public Transform playerHead; // Esto es la c�mara del VR rig (HMD)
    public Transform wallDirectionReference; // Un punto enfrente de la pared, hacia donde deber�a mirar para verla
    public float maxViewAngle = 60f; // Cu�nto puede desviarse la mirada para que NO la est� viendo
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
                door.gameObject.transform.rotation = Quaternion.Euler(-90, 90, 180); // Resetea la rotaci�n de la puerta
                door.GetComponent<Rigidbody>().isKinematic = true;
                door.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().enabled = false; // Desactiva la puerta para que no se pueda abrir
                AudioManager.Instance.PlayOneShotPosition(FMODEvents.Instance.doorClosedSound, door.transform.position); // Reproduce el sonido de la puerta cerrada
                gramophone.GetComponent<AudioSource>().Stop(); // Desactiva el gram�fono 
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