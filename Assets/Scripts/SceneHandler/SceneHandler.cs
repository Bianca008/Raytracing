using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler : MonoBehaviour
{
    public void OpenRoomA()
    {
        SceneManager.LoadScene("RoomAscene");
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}
