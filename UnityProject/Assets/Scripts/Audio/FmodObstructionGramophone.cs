using UnityEngine;

public class FmodObstructionGramophone : MonoBehaviour
{
    public Transform player;   
    public FMODUnity.StudioEventEmitter emitter;
    public LayerMask wallLayers; 

    private float currentObstruction = 0f;

    void Update()
    {
        Vector3 dir = player.position - transform.position;
        float dist = dir.magnitude;

        bool blocked = Physics.Raycast(
            transform.position,
            dir.normalized,
            dist,
            wallLayers
        );

        float targetValue = blocked ? 1f : 0f;

        // Interpolación suave para evitar saltos bruscos
        currentObstruction = Mathf.Lerp(currentObstruction, targetValue, Time.deltaTime * 5f);

        emitter.SetParameter("ObstructionGramophone", currentObstruction);
    }
}
