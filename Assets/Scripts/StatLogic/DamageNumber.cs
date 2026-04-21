using TMPro;
using UnityEngine;

public class DamageNumber : MonoBehaviour
{
    public float floatUpSpeed = 20f;
    public float lifetime = 1f;
    public TextMeshProUGUI text;

    private float timer;

    public void Initialize(int amount)
    {
        text.text = amount.ToString();
    }

    void Update()
    {
        transform.position += Vector3.up * floatUpSpeed * Time.deltaTime;

        timer += Time.deltaTime;
        if (timer >= lifetime)
        {
            Destroy(gameObject);
        }
        text.color = new Color(text.color.r, text.color.g, text.color.b, 1 - timer / lifetime);
        transform.localScale = Vector3.Lerp(Vector3.one, Vector3.one * 1.2f, timer / lifetime);

    }
}
