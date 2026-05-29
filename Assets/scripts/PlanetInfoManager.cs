using UnityEngine;
using TMPro;

public class PlanetInfoManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject infoPanel;
    public TMP_Text titleText;
    public TMP_Text bodyText;

    private string currentPlanet = "";

    public void SetCurrentPlanet(string planetName)
    {
        currentPlanet = planetName;
        Debug.Log("Planet aktif: " + currentPlanet);
    }

    public void ShowInfo()
    {
        infoPanel.SetActive(true);

        switch (currentPlanet)
        {
            case "Earth":
                titleText.text = "EARTH";
                bodyText.text =
                    "Bumi adalah planet ketiga dari Matahari dan satu-satunya planet yang diketahui mendukung kehidupan. Sekitar 71% permukaannya tertutup air.";
                break;

            case "Saturnus":
                titleText.text = "SATURNUS";
                bodyText.text =
                    "Saturnus adalah planet keenam dari Matahari dan terkenal karena sistem cincinnya yang sangat besar dan indah. Planet ini tersusun terutama dari gas hidrogen dan helium.";
                break;

            case "Jupiter":
                titleText.text =
                    "JUPITER";
                bodyText.text =
                    "Jupiter adalah planet terbesar di Tata Surya. Planet ini memiliki Bintik Merah Besar, yaitu badai raksasa yang telah berlangsung selama ratusan tahun.";
                break;

            default:
                titleText.text = "INFORMASI";
                bodyText.text =
                    "Arahkan kamera ke marker planet terlebih dahulu.";
                break;
        }
    }

    public void HideInfo()
    {
        infoPanel.SetActive(false);
    }
}