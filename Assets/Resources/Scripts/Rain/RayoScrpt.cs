using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RayoScrpt : MonoBehaviour
{
    public float mintime, maxtime, timedelay;
    bool isflick = false;
    public float rainIntensity;

    private void Start()
    {
        rainIntensity = SaveManager.getCurrentDay() == 6 ? 2 : 1;
        var emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        emitter.SetParameter("Weather",rainIntensity);
        StartCoroutine(encender());

    }
    // Update is called once per frame
    void Update()
    {
        var emitter = GetComponent<FMODUnity.StudioEventEmitter>();
        if (!isflick)
        {       
            StartCoroutine(Flickering());
            emitter.Play();
        }
    }
    IEnumerator Flickering()
    {
        isflick = true;

        gameObject.GetComponent<Animation>().Play();
        timedelay = 1f;
        yield return new WaitForSeconds(timedelay);
        gameObject.GetComponent<Animation>().Stop();
        timedelay = UnityEngine.Random.Range(mintime, maxtime);
        yield return new WaitForSeconds(timedelay);
        isflick = false;

    }
    IEnumerator encender() {
        timedelay = 5f;
        yield return new WaitForSeconds(timedelay);
        gameObject.GetComponent<Animation>().enabled = true;
    }
}
