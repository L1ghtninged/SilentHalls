using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using UnityEngine.Events;

public class TextScript : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Color baseColor = Color.white;
    public Color hoverColor = Color.blue;

    public Color baseOutlineColor = Color.black;
    public Color hoverOutlineColor = Color.cyan;

    public float baseOutlineThickness = 0.366f;
    public float hoverOutlineThickness = 0.366f;

    public UnityEvent onClick;

    private TextMeshProUGUI text;
    private Vector3 originalScale;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
        originalScale = text.transform.localScale;

        // Nastavení výchozího vzhledu
        SetOutline(baseOutlineColor, baseOutlineThickness);
        text.color = baseColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        text.color = hoverColor;
        text.transform.localScale = originalScale * 1.1f;
        SetOutline(hoverOutlineColor, hoverOutlineThickness);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        text.color = baseColor;
        text.transform.localScale = originalScale;
        SetOutline(baseOutlineColor, baseOutlineThickness);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onClick?.Invoke();
    }

    private void SetOutline(Color color, float thickness)
    {
        // Pracujeme pøímo s materiálem fontu
        text.fontMaterial.SetColor("_OutlineColor", color);
        text.fontMaterial.SetFloat("_OutlineWidth", thickness);
    }
}
