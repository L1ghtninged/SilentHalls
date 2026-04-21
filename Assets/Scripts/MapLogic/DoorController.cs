using Assembly_CSharp;
using UnityEngine;
using System.Collections;

public class DoorController : MonoBehaviour
{
    private int x, y;
    private bool isNorthSouth;
    public bool isOpened = false;
    public int count = 0;
    public int openCount = 1;
    public GameObject doorObject;      // dve¯e k posunu (nap¯. panel uvnit¯ oblouku)

    [Header("Animace otev¯enÌ")]
    public float moveHeight = 3f;
    public float moveDuration = 1f;

    private Vector3 closedPosition;
    private Vector3 openedPosition;
    private Coroutine movementCoroutine;

    public void Initialize(int posX, int posY, bool ns, bool isOpened)
    {
        x = posX;
        y = posY;
        isNorthSouth = ns;
        this.isOpened = isOpened;

        closedPosition = doorObject.transform.localPosition;
        openedPosition = closedPosition + Vector3.up * moveHeight;

        // Nastav startovnÌ pozici podle stavu
        doorObject.transform.localPosition = isOpened ? openedPosition : closedPosition;
    }

    public void ToggleDoor()
    {
        count++;
        if (count != openCount) return;
        SoundManager.Instance.PlayOpenDoor();
        isOpened = !isOpened;

        // P¯epni logiku pr˘chodnosti
        bool wasWalkable = EntityPositions.IsWalkable(x, y);
        EntityPositions.UpdateTileWalkable(x, y, !wasWalkable);

        // Spusù animaci
        if (movementCoroutine != null)
            StopCoroutine(movementCoroutine);

        movementCoroutine = StartCoroutine(MoveDoor(isOpened ? openedPosition : closedPosition));
    }

    private IEnumerator MoveDoor(Vector3 targetPosition)
    {
        Vector3 startPosition = doorObject.transform.localPosition;
        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / moveDuration);
            doorObject.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }

        doorObject.transform.localPosition = targetPosition;
    }
}
