using UnityEngine;

public class RoomSensor : MonoBehaviour
{
    [SerializeField] string roomName;

    TrackerManager tracker;

    public void Init(TrackerManager manager)
    {
        tracker = manager;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tracker)
        {
            tracker.OnRoomEnter(roomName);
        }
    }
}