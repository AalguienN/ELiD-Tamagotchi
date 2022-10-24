using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.LowLevel;

public class AudioPlayer : MonoBehaviour
{
    public static AudioSource CamAudioSource;
    public static AudioSource WorldAudioSOurce;

    public enum sources { 
        cameraSource,
        worldSource
    }
    // Start is called before the first frame update
    void Awake()
    {
        CamAudioSource =  GameObject.FindWithTag("MainCamera").AddComponent<AudioSource>();

        PlayClip("Keep the fire burning main theme demo 2 late no melody (Low Q)");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*public void PlayLoop(string audioName)
    {
        CamAudioSource.Play((UnityEngine.AudioClip)Resources.Load("Audio/" + audioName));

    }*/

    //Reproduce el audio en diferentes sources
    public void PlayClip(string audioName, sources source = sources.cameraSource, Vector3 position = new Vector3()) {
        switch (source) {
            case sources.cameraSource:
                CamAudioSource.PlayOneShot((UnityEngine.AudioClip)Resources.Load("Audio/" + audioName));
                break;
            case sources.worldSource:
                GameObject aSource = Instantiate(new GameObject());
                aSource.name = "AudioSource";
                aSource.transform.position = position;
                aSource.AddComponent<AudioSource>();
                WorldAudioSOurce = aSource.GetComponent<AudioSource>();
                WorldAudioSOurce.PlayOneShot((UnityEngine.AudioClip)Resources.Load("Audio/" + audioName));
                Destroy(aSource, ((UnityEngine.AudioClip)Resources.Load("Audio/" + audioName)).length);
                break;
        }
        
    }
}
