using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class HighlightOnLight : MonoBehaviour
{
    public Color emissionColor = Color.grey;
    public float effectDuration = 2f;
    public float flickerSpeed = 10f;
    public float maxEmission = 1.5f;

    private Renderer objRenderer;
    private Material[] matInstances;
    private float timer;
    private bool isLit;

    void Start()
    {
        objRenderer = GetComponent<Renderer>();

        // Instanciar todos los materiales
        Material[] originalMats = objRenderer.materials;
        matInstances = new Material[originalMats.Length];
        for (int i = 0; i < originalMats.Length; i++)
        {
            matInstances[i] = new Material(originalMats[i]);
            matInstances[i].EnableKeyword("_EMISSION");
            matInstances[i].SetColor("_EmissionColor", Color.black);
        }
        objRenderer.materials = matInstances;
    }

    void Update()
    {
        if (isLit && timer > 0f)
        {
            timer -= Time.deltaTime;
            float randomFlicker = Mathf.PerlinNoise(Time.time * flickerSpeed, 0f);
            float intensity = Mathf.Lerp(0f, maxEmission, randomFlicker);
            Color emission = emissionColor * intensity;

            foreach (var mat in matInstances)
                mat.SetColor("_EmissionColor", emission);
        }
        else if (isLit)
        {
            foreach (var mat in matInstances)
                mat.SetColor("_EmissionColor", Color.black);

            isLit = false;
        }
    }

    public void Illuminate()
    {
        timer = effectDuration;
        isLit = true;
    }
}
