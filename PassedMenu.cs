using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PassedMenu : MonoBehaviour
{
    public void Replay()
    {
        SceneManager.LoadScene("Level-1");
    }
    public void Quit()
    {
        Application.Quit();
    }
}
