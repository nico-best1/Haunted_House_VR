using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MaterialPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public Material correctMaterial;
    public GameObject doorToUnlock;

    public void CheckCup(GameObject cupObject)
    {
        CupFiller cup = cupObject.GetComponent<CupFiller>();
        if (cup == null || cup.liquidRenderer == null)
        {
            Debug.LogWarning("Cup or liquid renderer missing.");
            return;
        }

        Material currentMat = cup.liquidRenderer.sharedMaterial;

        if (currentMat == correctMaterial)
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.successSound);
            UnlockDoor();
        }
        else
        {
            AudioManager.Instance.PlayOneShot(FMODEvents.Instance.failSound);
        }
    }

    private void UnlockDoor()
    {
        if (doorToUnlock != null)
        {
            doorToUnlock.GetComponent<Rigidbody>().isKinematic = false;
            doorToUnlock.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>().enabled = true;
        }
    }
}

