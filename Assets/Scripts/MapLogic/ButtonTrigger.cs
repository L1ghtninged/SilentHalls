using UnityEngine;

public class ButtonTrigger : MonoBehaviour, IClickable
{
    public DoorController door;
    public ButtonObject button;
    public void OnClick()
    {
        if (door != null)
        {
            door.ToggleDoor();
            button.isOpened = !button.isOpened;
        }
    }
}
