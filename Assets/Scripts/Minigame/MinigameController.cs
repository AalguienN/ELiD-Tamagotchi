using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinigameController : MonoBehaviour
{
    public GameObject precisionNeedle;
    public float precision;
    public float speed = 1f;
    private bool continueSpeed = true;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!continueSpeed) return;
        speed = Mathf.Clamp(speed+Time.deltaTime*Time.deltaTime*speed, 1f, 5f);
        precision = Mathf.Sin(Time.time*speed)*80f;
        precisionNeedle.transform.rotation = Quaternion.Euler(0f,0f,precision);

        if(Input.GetMouseButtonDown(0)) {
            StartCoroutine(StopNeedle());
        }
    }

    IEnumerator StopNeedle() {
        continueSpeed = false;
        speed += (1-Mathf.Abs(precision)/40f)/10f;
        yield return new WaitForSeconds(1f);
        continueSpeed = true;
    }
}
