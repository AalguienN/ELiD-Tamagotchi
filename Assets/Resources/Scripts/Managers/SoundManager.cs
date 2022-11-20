using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip[] sounds;
    public static SoundManager instance;
    public AudioClip[] music;

    [Header("AudioSources")]
    public AudioSource source;
    public AudioSource Msource;

    
    public AudioSource CamAudioSource; //For playing onesots on camera position
    public AudioSource WorldAudioSource; //For playing onesots in word space  

    //Loops Have to have diferent sources for each loop to play all together
    public AudioSource MusicAudioSource; //For music loops
    public AudioSource AmbienceAudioSource; //For ambience loops


    void Awake()
    {
        instance = this;
        CamAudioSource = GameObject.FindWithTag("MainCamera").AddComponent<AudioSource>();


        GameObject aux = Instantiate(new GameObject(), transform.parent = this.transform);
        aux.name = "MusicAudioSource";
        MusicAudioSource = aux.AddComponent<AudioSource>(); 
        aux = Instantiate(new GameObject(), transform.parent = this.transform);
        aux.name = "AmbienceAudioSource";
        AmbienceAudioSource = aux.AddComponent<AudioSource>();

    }

    private void Start()
    {
        //PlaySound(1);
        //PlayMusic("ktfb main theme demo 2 late no melody (Low Q)");
        //PlayLoop("FireBurning");
        //PlayLoop("Rain (Needs editing)");
        //PlaySound("FireBurning");
        //Exaples of how to use PlaySound
        //PlaySound("Rain (Needs editing)");
        //PlaySound("FireBurning", GameObject.FindWithTag("Bonfire2").transform.position);
        //PlaySound("FireBurning", new Vector3(0,0,111111111));
    }

    //plays sound given a number *outdated but functional
    public void PlaySound(int i)
    {
        source.clip = sounds[i];
        source.Play(0);
    }

    #region specific Clips
    public void StopSound()
    {
        source.Stop();
    }

    public void PauseSound()
    {
        source.Pause();
    }

    public void UnpauseSound()
    {
        source.UnPause();
    }
    public void PlayMusic(int i)
    {
        Msource.clip = music[i];
        Msource.Play(0);
    }

    public void StopMusic()
    {
        Msource.Stop();
    }

    public void PauseMusic()
    {
        Msource.Pause();
    }

    public void UnpauseMusic()
    {
        Msource.UnPause();
    }
    #endregion

    //Adrian's code start here (ask if needed)
    private void PlayLoop(string audioName, AudioSource audioSource) {
        audioSource.Stop();
        audioSource.clip = (AudioClip)Resources.Load(getPath(audioName));
        audioSource.loop = true;
        audioSource.Play();
    }
    //Play music in loop
    public void PlayMusic(string audioName) {
        PlayLoop(audioName, MusicAudioSource);
    }

    //Play ambience in loop
    public void PlayAmbience(string audioName)
    {
        PlayLoop(audioName, AmbienceAudioSource);
    }

    //Play sound in loop
    public void PlayLoop(string audioName) {
        PlayLoop(audioName, AmbienceAudioSource);
    }

    //Plays sounds on camera position Standard <-------------------------this is the one you want to call most of the time
    public void PlaySound(string audioName)
    {
        PlaySound(audioName, CamAudioSource);
    }

    //Generic play sound : given an audio name and an audio source
    private void PlaySound(string audioName, AudioSource audioSource, bool audioPitching = false) {
        string clip = "";
       
        try
        {
            clip = getPath(audioName);
            audioSource.PlayOneShot((UnityEngine.AudioClip)Resources.Load(clip));
        }
        catch (System.Exception)
        {
            Debug.LogWarning("Can't find AudioClip with name " + audioName);
        }
    }

    //Plays sound anywere in 3d space : given an audio name and a position in 3d space
    public void PlaySound(string audioName, Vector3 position) {
        GameObject aSource = Instantiate(new GameObject(), position, Quaternion.identity);
        aSource.name = "AudioSource";
        aSource.AddComponent<AudioSource>();
        aSource.GetComponent<AudioSource>().spatialBlend = 1;
        PlaySound(audioName, aSource.GetComponent<AudioSource>());
        WorldAudioSource = aSource.GetComponent<AudioSource>();
        Destroy(aSource, ((UnityEngine.AudioClip)Resources.Load(getPath(audioName))).length);
    }

    //Gets path of the audio (can do it with a for loop but originally it was only 2 directories... sooooo we have the if-else nightmare
    public string getPath(string name) {
        string clip = "";
        if ((UnityEngine.AudioClip)Resources.Load("Sound/" + name))
            clip = "Sound/" + name;
        else if ((UnityEngine.AudioClip)Resources.Load("Sound/SoundFX/" + name))
            clip = "Sound/SoundFX/" + name;
        else if ((UnityEngine.AudioClip)Resources.Load("Sound/SFX/" + name))
            clip = "Sound/SFX/" + name;
        else if ((UnityEngine.AudioClip)Resources.Load("Sound/Ambience/" + name))
            clip = "Sound/Ambience/" + name;
        else if ((UnityEngine.AudioClip)Resources.Load("Sound/Music/" + name))
            clip = "Sound/Music/" + name;
        else if ((UnityEngine.AudioClip)Resources.Load("Sound/Forest/" + name))
            clip = "Sound/Forest/" + name;
        else if ((UnityEngine.AudioClip)Resources.Load("Sound/Dark forest/" + name))
            clip = "Sound/Dark forest/" + name;
        else 
            throw new System.Exception("No encontrado el audio" + name);
        
        return clip;
    }

}
