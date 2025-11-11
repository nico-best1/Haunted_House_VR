using UnityEngine;


public class SocketEventCaller : MonoBehaviour
{
    public PuzzleManager puzzleManager;

    private void Start()
    {
        var socket = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactors.XRSocketInteractor>();
        socket.selectEntered.AddListener((args) =>
        {
            puzzleManager.CheckPuzzle();
        });
    }
}
