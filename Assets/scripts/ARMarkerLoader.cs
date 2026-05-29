using UnityEngine;
using UnityEngine.SceneManagement;

public class ARMarkerLoader : MonoBehaviour
{
    public void BackToMissionControl()
    {
        SceneManager.LoadScene("MissionControl");
    }
}