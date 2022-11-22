using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Animations;

public class SceneChangeManager : MonoBehaviour
{
    AsyncOperation oper;
    private const string START_SCENE = "FadeIn", END_SCENE = "FadeOut";
    public const int MAIN_SCENE = 1;
    private int currentSceneIndex = 0;
    private static int sceneChangingTo = 0;
    public bool goingToChange = false;

    Animator anim;
    string currentState = "";

    #region Singleton and Setup
    public static SceneChangeManager instance;
    private void Awake()
    {
        instance = this;

        anim = GetComponent<Animator>();
    }
    #endregion

    private void LoadScene(int sceneIndex = 0)
    {
        int sceneToChange;
        if (goingToChange)
        {
            sceneToChange = sceneChangingTo;
            goingToChange = false;
        }
        else
        {
            sceneToChange = sceneIndex;
        }
        StartCoroutine(LoadSceneAsyncronously(sceneToChange));
    }

    public static void LoadSceneTrhoughAnimation(int sceneIndex)
    {
        sceneChangingTo = sceneIndex;
    }

    public static void LoadSceneDirectly(int sceneIndex)
    {
        instance.LoadScene(sceneIndex);
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
        IndirectChange();

        if(currentSceneIndex == MAIN_SCENE)
        {
            ChangeAnimation(START_SCENE);
        }

        if(oper != null)
        {
            if (oper.isDone /*Here check if scene is mainscene and if day 1 and do if it is*/)
            {
                ChangeAnimation(START_SCENE);
                oper = null;
            }
        }

    }

    private void IndirectChange()
    {
        if (currentSceneIndex != SceneManager.GetActiveScene().buildIndex)
        {
            currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        }
        if (currentSceneIndex != sceneChangingTo)
        {
            goingToChange = true;
            ChangeAnimation(END_SCENE);
        }
    }

    

    private void ChangeAnimation(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);

        currentState = newState;
    }

}
