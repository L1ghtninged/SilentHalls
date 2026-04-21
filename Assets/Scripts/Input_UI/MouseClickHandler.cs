using UnityEngine;

public class MouseClickHandler : MonoBehaviour
{
    public float maxDistance = 5f;
    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // levé tlaèítko
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            {
                IClickable clickable = hit.collider.GetComponent<IClickable>();
                if (clickable != null)
                {
                    clickable.OnClick();
                }
            }
        }
    }
}
public interface IClickable
{
    void OnClick();
}