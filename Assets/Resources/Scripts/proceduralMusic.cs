using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class proceduralMusic : MonoBehaviour
{
    public float bpm = 1;
    private int beat;
    private SoundManager sm;
    private string[] chords = { "PianoChords_96BPM", "Bass_96BPM", "Strings_96BPM" };
    private string[] mellody = { "WoodwindMelodyLively_96BPM", "WoodwindMelodyQuiet_96BPM", "PianoMelody_96BPM" };
    private string[] mellody2 = { "BellMelody_96BPM", "WoodwindPlaning_96BPM" };

    private string playing;



    // Start is called before the first frame update
    void Start()
    {
        sm = GetComponent<SoundManager>();
        int currentDay = WeeklyCalendar.GetCurrentDay(System.DateTime.Now);
        switch (currentDay) {
            case 1:
            case 2:
                bpm = 79;
                break;
            default:
                bpm = 96;
                break;
        }

        StartCoroutine(PlayForestL(60/bpm));

    }

    public IEnumerator PlayForestL(float beat) {
        int count = 0;
        float t = ((AudioClip)Resources.Load(sm.getPath("BellMelody_96BPM"))).length - 12*4*(60 / 79);
        while (true) {
            count++;
            PlayForest();
            yield return new WaitForSeconds(t);
            
        }
    }



    public void PlayForest() {
        

        //Play harmony
        int r = Random.Range(0, 100);
        if (r < 33)
        {
            sm.PlaySound(chords[Random.Range(0, chords.Length)]);
        }
        else if (r < 66)
        {
            string a = chords[Random.Range(0, chords.Length)];
            string b = chords[Random.Range(0, chords.Length)];
            while (a == b)
            {
                b = chords[Random.Range(0, chords.Length)];
            }
            sm.PlaySound(a);
            sm.PlaySound(b);
        }
        else
        {
            sm.PlaySound(chords[0]);
            sm.PlaySound(chords[1]);
            sm.PlaySound(chords[2]);
        }
        //playMelody
        r = Random.Range(0, 100);
        if (r < 50)
        {
            sm.PlaySound(mellody[2]);
            sm.PlaySound(mellody[Random.Range(0, 1)]);
        }
        else
        {
            sm.PlaySound(mellody2[0]);
            sm.PlaySound(mellody2[1]);
        }
        

    }



}
