using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public AudioClip [] sounds;
    public static SoundManager instance;
    public AudioClip[] music;
    public AudioSource source;
    public AudioSource Msource;
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    public void PlaySound(int i) 
    {
        source.clip = sounds[i];
        source.Play(0);
    }

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
}
