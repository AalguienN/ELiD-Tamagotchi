using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TapToRestart : MonoBehaviour
{
    public void RestartGame()
    {
        SaveManager.clearData();
        SceneManager.LoadScene(1);
    }
}
