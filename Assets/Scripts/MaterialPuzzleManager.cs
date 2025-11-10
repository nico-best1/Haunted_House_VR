using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class MaterialPuzzleManager : MonoBehaviour
{
    [Header("Puzzle Settings")]
    public Material correctMaterial;
    public AudioSource audioSource;
    public AudioClip successClip;
    public AudioClip errorClip;
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
            PlaySound(successClip);
            UnlockDoor();
        }
        else
        {
            PlaySound(errorClip);
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.Stop();
            audioSource.clip = clip;
            audioSource.Play();
        }
    }

    private void UnlockDoor()
    {
        if (doorToUnlock != null)
        {
            doorToUnlock.GetComponent<Rigidbody>().isKinematic = false;
            doorToUnlock.GetComponent<XRGrabInteractable>().enabled = true;
        }
    }
}

