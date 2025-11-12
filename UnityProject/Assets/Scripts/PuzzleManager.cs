using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using FMODUnity;

public class PuzzleManager : MonoBehaviour
{
    public XRSocketInteractor[] sockets;        // Los 3 sockets
    public string[] correctOrder;               // Nombres correctos de objetos
    public GameObject door;

    // FMOD Event References
    [SerializeField] private EventReference successSound;
    [SerializeField] private EventReference failSound;

    private bool puzzleSolved = false;

    public void CheckPuzzle()
    {
        if (puzzleSolved) return;

        // No hacer nada si no están los 3 ocupados
        foreach (var socket in sockets)
        {
            if (!socket.hasSelection)  // usa la propiedad hasSelection
                return;
        }

        // Aquí sigue la lógica normal:
        bool correct = true;
        for (int i = 0; i < sockets.Length; i++)
        {
            XRSocketInteractor socket = sockets[i];
            IXRSelectInteractable interactable = null;

            // Versión segura para obtener el seleccionado
            if (socket.interactablesSelected.Count > 0)
                interactable = socket.interactablesSelected[0];
            // Alternativamente: interactable = socket.firstInteractableSelected;

            if (interactable == null ||
                interactable.transform.name != correctOrder[i])
            {
                correct = false;
                break;
            }
        }

        if (correct)
        {
            AudioManager.Instance.PlayOneShot(successSound, this.transform.position);
            var rbDoor = door.GetComponent<Rigidbody>();
            if (rbDoor != null) rbDoor.isKinematic = false;

            var grabDoor = door.GetComponent<XRGrabInteractable>();
            if (grabDoor != null) grabDoor.enabled = true;

            puzzleSolved = true;
            DisableObjectInteractions();
        }
        else
        {
            AudioManager.Instance.PlayOneShot(failSound, this.transform.position);
        }
    }

    private void DisableObjectInteractions()
    {
        foreach (var socket in sockets)
        {
            IXRSelectInteractable interactable = null;
            if (socket.interactablesSelected.Count > 0)
                interactable = socket.interactablesSelected[0];

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
