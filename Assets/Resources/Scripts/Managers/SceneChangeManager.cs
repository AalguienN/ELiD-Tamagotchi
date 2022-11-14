using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChangeManager : MonoBehaviour
{
    AsyncOperation oper;

    public void LoadScene(int sceneIndex)
    {
        StartCoroutine(LoadSceneAsyncronously(sceneIndex));
    }

    IEnumerator LoadSceneAsyncronously(int sceneIndex)
    {
        oper = SceneManager.LoadSceneAsync(sceneIndex);

        while (!oper.isDone)
        {
            yield return null;
        }
    }

    private void Update()
    {
        if(oper != null)
        {
            if (oper.isDone /*Here check if scene is mainscene and if day 1 and do if it is*/)
            {
                //Here put the start animation
            }
        }
    }
}
