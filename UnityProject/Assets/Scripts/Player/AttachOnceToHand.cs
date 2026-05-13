using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class AttachOnceToHand : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private bool isLocked = false;

    public GameObject Instructions1;
    public GameObject Instructions2;

    void Awake()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnSelectEntered);
            grabInteractable.selectExited.AddListener(OnSelectExited);
        }
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        if (!isLocked)
        {
            isLocked = true;

            Instructions1.SetActive(false);
            Instructions2.SetActive(true);

            StartCoroutine(HideInstructions2AfterDelay(20f));
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (!isLocked)
            return;

        // Guardar referencia segura
        var interactor = args.interactorObject as IXRSelectInteractor;

        if (interactor == null)
            return;

        // Re-seleccionar un frame después
        StartCoroutine(ReattachNextFrame(interactor));
    }

    private IEnumerator ReattachNextFrame(IXRSelectInteractor interactor)
    {
        yield return null;

        // Verificaciones defensivas
        if (this == null ||
            grabInteractable == null ||
            interactor == null ||
            !Application.isPlaying)
        {
            yield break;
        }

        // Evitar doble selección
        if (!interactor.IsSelecting(grabInteractable))
        {
            grabInteractable.interactionManager.SelectEnter(interactor, grabInteractable);
        }
    }

    private IEnumerator HideInstructions2AfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (Instructions2 != null)
        {
            Instructions2.SetActive(false);
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnSelectEntered);
            grabInteractable.selectExited.RemoveListener(OnSelectExited);
        }
    }
}