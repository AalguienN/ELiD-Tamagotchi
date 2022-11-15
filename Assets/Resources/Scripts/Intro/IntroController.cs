using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    float introDuration = 1f;

    public VideoClip introClip;
    AsyncOperation scene;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            scene = SceneManager.LoadSceneAsync(1, LoadSceneMode.Additive);
            scene.allowSceneActivation = false;
            introDuration = (float)introClip.length + 0.5f;
            print("Video duration: " + introDuration);
            GameObject camera = Camera.main.gameObject;
            var videoPlayer = camera.AddComponent<UnityEngine.Video.VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;
            videoPlayer.targetCameraAlpha = 0.5F;
            videoPlayer.clip = introClip;
            videoPlayer.Play();    
        } catch(System.Exception e){}
        StartCoroutine(UnloadScene(introDuration));
    }

    IEnumerator UnloadScene(float introD)
    {
        yield return new WaitForSeconds(introD);
        scene.allowSceneActivation = true;
        SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(1));
        SceneManager.UnloadScene(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
