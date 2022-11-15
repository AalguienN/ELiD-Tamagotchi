using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainAudio : MonoBehaviour
{


    public int rainState;
    public int currentDay;

    // Start is called before the first frame update
    void Awake()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        currentDay = SaveManager.getCurrentDay();

        if (currentDay == 3) {
            rainState = 1;
        } else if (currentDay == 5) {
            rainState = 2;
        } else {
            rainState = 0;
        }

        
        // No es lo mas eficiente pero xD

        var emitter = GetComponent<FMODUnity.StudioEventEmitter>();

        emitter.SetParameter("Weather",rainState);

    }
}
