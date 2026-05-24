using UnityEngine;

public class AstronotMovement : MonoBehaviour
{
    public float speed = 100f;
    public float rotationSpeed = 90f;
    public float topBoundary = 350f;
    public float bottomBoundary = -350f;
    public float startX = -350f;

    private RectTransform rectTransform;
    private float currentY;
    private float currentAngle;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        // Pivot harus di tengah supaya rotasi = spin di tempat
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        // Reset rotasi
        currentAngle = 0f;
        rectTransform.localRotation = Quaternion.identity;

        // Mulai dari bawah
        currentY = bottomBoundary;
        rectTransform.anchoredPosition = new Vector2(startX, currentY);
    }

    void Update()
    {
        // 1. Naik vertikal
        currentY += speed * Time.deltaTime;
        if (currentY > topBoundary)
        {
            currentY = bottomBoundary;
        }

        // 2. Rotasi di tempat (spin)
        currentAngle += rotationSpeed * Time.deltaTime;
        rectTransform.localEulerAngles = new Vector3(0, 0, currentAngle);

        // 3. Set posisi
        rectTransform.anchoredPosition = new Vector2(startX, currentY);
    }
}