using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TooltipUI : MonoBehaviour
{
    public static TooltipUI instance;
    public TextMeshProUGUI tooltipText;

    void Awake()
    {
        if(instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
        }
    }
    private void Start()
    {
        gameObject.SetActive(false);
    }
    private void Update()
    {
        
        transform.position = Input.mousePosition;
    }
    public void SetAndShowTooltip(string text)
    {
        transform.position = Input.mousePosition;
        gameObject.SetActive(true);
        tooltipText.text = text;
    }
    public void HideTooltip()
    {
        gameObject.SetActive(false);
        tooltipText.text = string.Empty;
    }


}
