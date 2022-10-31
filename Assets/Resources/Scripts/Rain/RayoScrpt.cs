using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class RayoScrpt : MonoBehaviour
{
    public float mintime = 5, maxtime = 60, timedelay;
    bool isflick = false;
    private void Start()
    {
        StartCoroutine(encender());
    }
    // Update is called once per frame
    void Update()
    {
        if (!isflick)
        {
          
        
            StartCoroutine(Flickering());
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
