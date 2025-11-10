using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketEventCaller : MonoBehaviour
{
    public PuzzleManager puzzleManager;

    private void Start()
    {
        var socket = GetComponent<XRSocketInteractor>();
        socket.selectEntered.AddListener((args) =>
        {
            puzzleManager.CheckPuzzle();
        });
    }
}
