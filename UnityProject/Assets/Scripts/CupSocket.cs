using UnityEngine;

public class CupSocket : MonoBehaviour
{
    public MaterialPuzzleManager puzzleManager;

    private void OnTriggerEnter(Collider other)
    {
        puzzleManager.CheckCup(other.gameObject);
    }
}
