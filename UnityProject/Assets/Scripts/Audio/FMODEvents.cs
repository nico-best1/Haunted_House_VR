using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Puzzle Success")]
    [field: SerializeField] public EventReference successSound { get; private set; }

    [field: Header("Puzzle Fail")]
    [field: SerializeField] public EventReference failSound { get; private set; }

    [field: Header("Fill Cup")]
    [field: SerializeField] public EventReference fillCupSound { get; private set; }

    [field: Header("Puzzle Cube")]
    [field: SerializeField] public EventReference puzzleCubeSound { get; private set; }

    [field: Header("Door Closed")]
    [field: SerializeField] public EventReference doorClosedSound { get; private set; }

    public static FMODEvents Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            Debug.LogError("Multiple FMODEvents instances detected! Destroying duplicate.");
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }
}
