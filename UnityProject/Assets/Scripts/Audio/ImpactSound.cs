using UnityEngine;

public class ImpactSound : MonoBehaviour
{
    public float minVelocity = 0.2f;
    private MaterialImpactData materialData;

    void Start()
    {
        materialData = GetComponent<MaterialImpactData>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        float velocity = collision.relativeVelocity.magnitude;
        if (velocity < minVelocity) return;

        var instance = FMODUnity.RuntimeManager.CreateInstance(FMODEvents.Instance.objectsImpact);

        instance.setParameterByName("MaterialType", (float)materialData.material);

        instance.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(transform));

        instance.start();
        instance.release();
    }
}
