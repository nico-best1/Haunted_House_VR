using UnityEngine;

public enum MaterialType
{
    Book = 0,
    Glass = 1,
    Wood = 2,
}

public class MaterialImpactData : MonoBehaviour
{
    public MaterialType material;
}
