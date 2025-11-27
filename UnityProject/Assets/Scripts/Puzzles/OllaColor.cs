using UnityEngine;

public class OllaMaterial : MonoBehaviour
{
    public Renderer ollaRenderer;                     

    private void OnTriggerEnter(Collider other)
    {
        CupFiller cup = other.GetComponent<CupFiller>();
        if (cup != null && ollaRenderer != null)
        {
            Material mat = ollaRenderer.sharedMaterial; 
            cup.SetMaterial(mat);
            
            AudioManager.Instance.PlayOneShotPosition(FMODEvents.Instance.fillCupSound, transform.position);
        }
    }
}
