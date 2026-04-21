using UnityEngine;
using UnityEngine.EventSystems;
using static KeyItem;

public class DoorLock : MonoBehaviour
{
    public KeyType keyType;
    public DoorController door;
    public Lock _lock;
    public void OnKeyUsed()
    {
        OpenDoor();
    }

    private void OpenDoor()
    {
        _lock.isOpened = true;
        if (door != null)
        {
            door.ToggleDoor();
        }
        Debug.Log("Door unlocked!");
    }
}