using UnityEngine;

public class ImpactSound : MonoBehaviour
{
    public float minVelocity = 0.2f;
    public float maxVelocity = 10f; // para normalizar el parámetro de FMOD
    private MaterialImpactData materialData;

    void Start()
    {
        materialData = GetComponent<MaterialImpactData>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float velocity = collision.relativeVelocity.magnitude;
        if (velocity < minVelocity) return;

        // Crear instancia del evento
        var instance = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.Instance.objectsImpact);

        // Ajustar el tipo de material
        instance.setParameterByName("MaterialType", (float)materialData.material);

        // Ajustar la velocidad/volumen según la fuerza del impacto
        // Normalizamos velocity entre 0 y 1 usando minVelocity y maxVelocity
        float normalizedVelocity = Mathf.Clamp01((velocity - minVelocity) / (maxVelocity - minVelocity));
        instance.setParameterByName("objectVelocity", normalizedVelocity);

        // Ajustar posición 3D
        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));

        // Reproducir y liberar la instancia
        instance.start();
        instance.release();
    }
}
