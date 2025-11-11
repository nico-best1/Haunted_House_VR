using UnityEngine;

public class CupFiller : MonoBehaviour
{
    public Renderer liquidRenderer;         

    public void SetMaterial(Material newMat)
    {
        if (liquidRenderer != null && newMat != null)
        {
            liquidRenderer.material = newMat;
        }
    }
}
