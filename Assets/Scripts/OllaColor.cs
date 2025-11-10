using UnityEngine;

public class OllaMaterial : MonoBehaviour
{
    public Renderer ollaRenderer;           
    public AudioSource fillSound;          

    private void OnTriggerEnter(Collider other)
    {
        CupFiller cup = other.GetComponent<CupFiller>();
        if (cup != null && ollaRenderer != null)
        {
            Material mat = ollaRenderer.sharedMaterial; 
            cup.SetMaterial(mat);

            if (fillSound != null)
                fillSound.Play();
        }
    }
}
