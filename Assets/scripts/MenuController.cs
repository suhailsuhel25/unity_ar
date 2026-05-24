using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{
    public GameObject panelMainMenu;
    public GameObject panelAbisMulai;

    public void Mulai()
    {
        if (panelMainMenu != null) panelMainMenu.SetActive(false);
        if (panelAbisMulai != null) panelAbisMulai.SetActive(true);
    }

    public void KeluarAplikasi()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    public void BukaPeralatan()
    {
        Debug.Log("Buka Peralatan");
    }

    public void KembaliKeMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void BukaTutorial()
    {
        SceneManager.LoadScene("Langkah-Langkah");
    }

    public void BukaAstroScanAR()
    {
        SceneManager.LoadScene("AstroScanAR");
    }

    public void BukaJelajahTataSurya()
    {
        SceneManager.LoadScene("JelajahTataSurya");
    }

    public void KembaliKeMissionControl()
    {
        SceneManager.LoadScene("MissionControl");
    }
}
