using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject panelLangkahAR;
    public GameObject panelJelajah;

    // BUKA HALAMAN LANGKAH AR
    public void OpenLangkahAR()
    {
        mainMenu.SetActive(false);
        panelLangkahAR.SetActive(true);
        panelJelajah.SetActive(false);
    }

    // BUKA HALAMAN JELAJAH
    public void OpenJelajah()
    {
        mainMenu.SetActive(false);
        panelLangkahAR.SetActive(false);
        panelJelajah.SetActive(true);
    }

    // BALIK KE MENU
    public void BackToMenu()
    {
        mainMenu.SetActive(true);
        panelLangkahAR.SetActive(false);
        panelJelajah.SetActive(false);
    }

    // BALIK KE MISSION CONTROL SCENE
    public void BackToMissionControl()
    {
        SceneManager.LoadScene("MissionControl");
    }
}