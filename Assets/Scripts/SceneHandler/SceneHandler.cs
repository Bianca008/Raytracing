using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void OpenRoomA()
    {
        SceneManager.LoadScene("RoomAscene");
    }

    public void OpenRoomB()
    {
        SceneManager.LoadScene("RoomBscene");
    }

    public void ExitButton()
    {
        Application.Quit();
    }

    public void BackButton()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
