using UnityEngine;
using FMODUnity;

public class FMODEvents : MonoBehaviour
{
    [field: Header("Creepy Sounds")]
    [field: SerializeField] public EventReference creepySounds { get; private set; }

    [field: Header("Ambience")]
    [field: SerializeField] public EventReference ambience { get; private set; }

    [field: Header("Cauldron Boiling")]
    [field: SerializeField] public EventReference cauldronBoiling { get; private set; }

    [field: Header("Clock Sound")]
    [field: SerializeField] public EventReference clockSound { get; private set; }

    [field: Header("Build Up Tension")]
    [field: SerializeField] public EventReference buildUpTension { get; private set; }

    [field: Header("Drawer Sound")]
    [field: SerializeField] public EventReference drawerSound { get; private set; }

    [field: Header("Flashlight On")]
    [field: SerializeField] public EventReference flashlightOn { get; private set; }

    [field: Header("Flashlight Off")]
    [field: SerializeField] public EventReference flashlightOff { get; private set; }

    [field: Header("Credits Music")]
    [field: SerializeField] public EventReference creditsMusic { get; private set; }

    [field: Header("Final Jumpscare")]
    [field: SerializeField] public EventReference finalJumpscare { get; private set; }

    [field: Header("Objects Impact")]
    [field: SerializeField] public EventReference objectsImpact { get; private set; }

    [field: Header("Gramophone Music")]
    [field: SerializeField] public EventReference gramophoneMusic { get; private set; }

    [field: Header("Tension")]
    [field: SerializeField] public EventReference tension { get; private set; }

    [field: Header("Wind")]
    [field: SerializeField] public EventReference wind { get; private set; }

    [field: Header("Flicker Lights")]
    [field: SerializeField] public EventReference flickerLights { get; private set; }

    [field: Header("Strong String Jumpscare")]
    [field: SerializeField] public EventReference strongStringJumpscare { get; private set; }

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

    [field: Header("Player Footsteps")]
    [field: SerializeField] public EventReference playerFootsteps { get; private set; }

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
