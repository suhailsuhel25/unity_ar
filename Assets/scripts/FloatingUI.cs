using UnityEngine;

public class FloatingUI : MonoBehaviour
{
    public float amplitude = 10f; // Seberapa jauh naik turunnya
    public float frequency = 1f;  // Seberapa cepat geraknya
    Vector2 posInitial;

    void Start() {
        posInitial = GetComponent<RectTransform>().anchoredPosition;
    }

    void Update() {
        RectTransform rect = GetComponent<RectTransform>();
        float newY = posInitial.y + Mathf.Sin(Time.time * frequency) * amplitude;
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, newY);
    }
}