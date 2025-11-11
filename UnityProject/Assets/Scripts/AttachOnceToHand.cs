using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

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
        if (!isLocked && grabInteractable != null)
        {
            isLocked = true;
            Instructions1.SetActive(false);
            Instructions2.SetActive(true);
            StartCoroutine(HideInstructions2AfterDelay(20f));
        }
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        if (!isLocked || grabInteractable == null || !this.isActiveAndEnabled)
            return;

        var interactor = args.interactorObject;

        // Validaciones defensivas por si Unity está cerrando y objetos ya han sido destruidos
        if (interactor == null || interactor.transform == null)
            return;

        if (grabInteractable.transform == null || grabInteractable.interactionManager == null)
            return;

        if (interactor is UnityEngine.XR.Interaction.Toolkit.Interactors.IXRSelectInteractor selectInteractor)
        {
            if (selectInteractor.IsSelecting(grabInteractable))
                return;

            // Solo realizar el reanclaje si el juego aún se está ejecutando
            if (Application.isPlaying)
            {
                grabInteractable.interactionManager.SelectEnter(selectInteractor, grabInteractable);
            }
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
