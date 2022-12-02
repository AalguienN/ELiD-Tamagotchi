using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RainSoundManager : MonoBehaviour
{
    // Start is called before the first frame update
    public float rainIntensity;
    void Start()
    {
        rainIntensity = SaveManager.getCurrentDay() == 6 ? 2 : 1;
        var emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        emitter.SetParameter("Weather",rainIntensity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
