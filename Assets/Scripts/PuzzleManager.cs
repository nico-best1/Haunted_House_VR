using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PuzzleManager : MonoBehaviour
{
    public XRSocketInteractor[] sockets;        // Los 3 sockets
    public string[] correctOrder;               // Nombres correctos de objetos
    public AudioClip successClip;
    public AudioClip failClip;
    public GameObject door;

    private bool puzzleSolved = false;

    public void CheckPuzzle()
    {
        if (puzzleSolved) return;

        // No hacer nada si no están los 3 ocupados
        foreach (var socket in sockets)
        {
            if (socket.GetOldestInteractableSelected() == null)
                return;
        }

        // Aquí sigue la lógica normal:
        bool correct = true;
        for (int i = 0; i < sockets.Length; i++)
        {
            IXRSelectInteractable interactable = sockets[i].GetOldestInteractableSelected();
            if (interactable.transform.name != correctOrder[i])
            {
                correct = false;
                break;
            }
        }

        if (correct)
        {
            SoundManager.Instance.PlaySFX(successClip);
            door.GetComponent<Rigidbody>().isKinematic = false;
            door.GetComponent<XRGrabInteractable>().enabled = true;
            puzzleSolved = true;
            DisableObjectInteractions();
        }
        else
        {
            SoundManager.Instance.PlaySFX(failClip);
        }
    }


    private void DisableObjectInteractions()
    {
        foreach (var socket in sockets)
        {
            IXRSelectInteractable interactable = socket.GetOldestInteractableSelected();
            if (interactable != null)
            {
                GameObject obj = interactable.transform.gameObject;

                XRGrabInteractable grab = obj.GetComponent<XRGrabInteractable>();
                if (grab != null) grab.enabled = false;

                Rigidbody rb = obj.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            }
        }
    }
}
