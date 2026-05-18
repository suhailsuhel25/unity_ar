using UnityEngine;

public class AstronotMovement : MonoBehaviour
{
    public float speed = 900f;
    public float rotationSpeed = 200f;
    public float batasKanan = 600f;   // keluar dari sisi kanan layar
    public float batasKiri = -600f;  // mulai dari luar sisi kiri layar
    public float topBoundary = 300f;   // Batas atas
    public float bottomBoundary = -300f;  // Batas bawah

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        // Mulai dari tengah layar
        rectTransform.anchoredPosition = new Vector2(230, rectTransform.anchoredPosition.y);
    }

    void Update()
    {
        // Bergerak ke atas dan ke bawah
        rectTransform.anchoredPosition += new Vector2(0, speed * Time.deltaTime);
        // Berputar searah jarum jam
        rectTransform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

        // Reset ke bawah jika sudah di atas
        if (rectTransform.anchoredPosition.y > topBoundary)
        {
            rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, bottomBoundary);
        }
    }
}