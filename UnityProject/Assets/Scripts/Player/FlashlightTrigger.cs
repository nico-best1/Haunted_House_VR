using UnityEngine;

public class FlashlightRaycast : MonoBehaviour
{
    public Light flashlight;
    public float maxDistance = 10f;
    public LayerMask detectionLayers;
    public int raysPerFrame = 10;         
    public float spreadAngle = 10f;        

    void Update()
    {
        if (flashlight != null && flashlight.intensity > 0.01f)
        {
            for (int i = 0; i < raysPerFrame; i++)
            {
                Vector3 randomDirection = GetRandomDirectionWithinCone(flashlight.transform.forward, spreadAngle);

                Ray ray = new Ray(flashlight.transform.position, randomDirection);
                if (Physics.Raycast(ray, out RaycastHit hit, maxDistance, detectionLayers))
                {
                    HighlightOnLight highlight = hit.collider.GetComponent<HighlightOnLight>();
                    if (highlight != null)
                    {
                        highlight.Illuminate();
                    }
                }
            }
        }
    }
    Vector3 GetRandomDirectionWithinCone(Vector3 forward, float angle)
    {
        Quaternion randomRotation = Quaternion.AngleAxis(Random.Range(-angle, angle), Vector3.up)
                                   * Quaternion.AngleAxis(Random.Range(-angle, angle), Vector3.right);
        return randomRotation * forward;
    }
}
