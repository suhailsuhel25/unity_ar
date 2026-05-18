using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("MissionControl");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}