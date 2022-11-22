using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class IntroController : MonoBehaviour
{
    float introDuration = 1f;

    public VideoClip introClip;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            introDuration = (float)introClip.length + 0.5f;
            GameObject camera = Camera.main.gameObject;
            var videoPlayer = camera.AddComponent<UnityEngine.Video.VideoPlayer>();
            videoPlayer.playOnAwake = false;
            videoPlayer.renderMode = UnityEngine.Video.VideoRenderMode.CameraNearPlane;
            videoPlayer.targetCameraAlpha = 0.5F;
            videoPlayer.clip = introClip;
            videoPlayer.Play();    
        } catch(System.Exception e){
            Debug.Log("Error ocurred during start animation");
        }
        StartCoroutine(UnloadScene(introDuration));
    }

    IEnumerator UnloadScene(float introD)
    {
        yield return new WaitForSeconds(introD);
        print("LoadingNewScene");
        int sceneToLoad = SceneChangeManager.MAIN_SCENE; //Aquí falta el codigo para decidir que escena carga
        SceneChangeManager.LoadSceneDirectly(sceneToLoad);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
