using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShake : MonoBehaviour
{
    public static ScreenShake instance;

    private float shakeTimeRemaining, shakePower, shakeFadeTime, shakeRotation;
    private Quaternion initialRotation;

    private void Start()
    {
        instance = this;
        setInitialRotation(transform.rotation);
    }

    private void LateUpdate()
    {
        Shake();
        if (Input.GetKeyDown(KeyCode.Space))
        {
            print("CameraShakeTest");
            StartShake(.1f, 3);
        }
    }

    public void StartShake(float length, float power)
    {
        shakeTimeRemaining = length;
        shakePower = power;

        shakeFadeTime = power / length;

        shakeRotation = power;
    }
    private IEnumerator returnToCenter()
    {
        float timeToReturn = 0;
        while (transform.rotation != initialRotation)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, initialRotation, timeToReturn);
            timeToReturn += (Time.deltaTime / .5f);
            yield return null; //Wait one frame
        }
    }

    private void Shake()
    {

        if (shakeTimeRemaining > 0)
        {
            shakeTimeRemaining -= Time.deltaTime;

            float xAmount = Random.Range(-1f, 1f) * shakePower;
            float yAmount = Random.Range(-1f, 1f) * shakePower;
            float zAmount = Random.Range(-1f, 1f) * shakePower;

            transform.eulerAngles += new Vector3(xAmount, yAmount, zAmount);

            shakePower = Mathf.MoveTowards(shakePower, 0f, shakeFadeTime * Time.deltaTime);

            shakeRotation = Mathf.MoveTowards(shakeRotation, 0f, shakeFadeTime * Time.deltaTime);
        }
        else if(transform.rotation != initialRotation)
        {
            StartCoroutine(returnToCenter());
        }

    }

    public void setInitialRotation(Quaternion rotation)
    {
        initialRotation = rotation;
    }

}
