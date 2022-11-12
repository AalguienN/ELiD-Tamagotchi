using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    public string sceneToLoad;
    public float introDuration;

    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadSceneAsync(sceneToLoad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
