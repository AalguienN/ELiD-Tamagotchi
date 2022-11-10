using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MinigameController : MonoBehaviour
{
    [Header("Axe variables")]
    public GameObject axePrefab;
    public Vector3 axeStartPosition;
    public Vector3 axeStartRotation;
    public Vector3 axeMinPosition;
    public Vector3 axeMinRotation;
    public Vector3 axeMaxPosition;
    public Vector3 axeMaxRotation;
    Vector3 axeActualPosition;
    Vector3 axeActualRotation;
    public Vector3 axeHitPosition;
    public Vector3 axeHitRotation;

    [Header("Needle variables")]
    public GameObject precisionNeedle;
    public float precision;
    private float precision01;
    public float speed = 1f;
    private bool continueSpeed = false;

    [Header("Stick variables")]
    public TMP_Text stickTextTemp;
    public int sticks;

    [Header("Other variables")]
    private float gameTime;
    private bool gameRunning = false;
    private Vector2 mousePosition;

    void Start() {
        Reward(0);
        axePrefab.transform.localPosition = axeStartPosition;
        axePrefab.transform.localRotation = Quaternion.Euler(axeStartRotation);
    }

    IEnumerator StartGame()
    {
        sticks = 0;
        gameRunning = true;
        Reward(0);
        float t = 0;
        while(t<=1) {
            t += Time.deltaTime/2f;
            axePrefab.transform.localPosition = Vector3.Lerp(axeStartPosition,axeActualPosition,t);
            axePrefab.transform.localRotation = Quaternion.Euler(Vector3.Lerp(axeStartRotation,axeActualRotation,t));
            axeActualPosition = axeMaxPosition;
            axeActualRotation = axeMaxRotation;
            yield return new WaitForEndOfFrame();
        }
        continueSpeed = true;
        gameTime = Random.Range(0.0f,10.0f);
    }

    IEnumerator EndGame(int reason)
    {
        print("Game has ended");
        gameRunning = false;
        float t = 0;
        while(t<=1) {
            t += Time.deltaTime/2f;
            axePrefab.transform.localPosition = Vector3.Lerp(axeActualPosition,axeStartPosition,t);
            axePrefab.transform.localRotation = Quaternion.Euler(Vector3.Lerp(axeActualRotation,axeStartRotation,t));
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForEndOfFrame();
        if(reason==-1) {
            if(CameraManagement.getActiveCamera()=="CamMinigame") {
                gameRunning = true; yield break;
            }
        }
        continueSpeed = false;
        gameTime = 0;
        speed = 0;
        precision = 0;
        precisionNeedle.transform.rotation = Quaternion.Euler(0f,0f,0f);
        int s = SaveManager.getStickNum();
        SaveManager.setStickNum(s+sticks);
        print("Stick number: " + SaveManager.getStickNum());
    }

    // Update is called once per frame
    void Update()
    {
        if(CameraManagement.getActiveCamera()=="CamMinigame") {
            //axePrefab.transform.position = axeStartPosition+ axeEndPositionOffset;
            if(!gameRunning) {
                if(Input.GetMouseButtonDown(0)) {
                    mousePosition = Input.mousePosition;
                }
                if(Input.GetMouseButtonUp(0)) { 
                    if(Vector3.Distance(mousePosition, Input.mousePosition) < 50f)
                        StartCoroutine(StartGame()); 
                }
                return;
            }
            if(!continueSpeed)
                return;
            gameTime += Time.deltaTime;
            speed = Mathf.Clamp(speed+Time.deltaTime*Time.deltaTime*speed, 1f, 15f);
            precision = Mathf.Sin(gameTime*speed)*80f;
            precisionNeedle.transform.rotation = Quaternion.Euler(0f,0f,precision);
            axePrefab.transform.localPosition = axeActualPosition = Vector3.Lerp(axeMinPosition,axeMaxPosition,1f - Mathf.Abs(precision/80f));
            axeActualRotation = Vector3.Lerp(axeMinRotation,axeMaxRotation,1f - Mathf.Abs(precision/80f));
            axePrefab.transform.localRotation = Quaternion.Euler(axeActualRotation);

            if(Input.GetMouseButtonUp(0)) {
                StartCoroutine(StopNeedle());
            }
        }
    }

    IEnumerator StopNeedle() {
        continueSpeed = false;
        speed += (0.5f-Mathf.Abs(precision)/80f)*2f;
        gameTime = Random.Range(0.0f,10.0f);
        precision01 = 1f - Mathf.Abs(precision/80f);
        
        float t = 0;
        while(axePrefab.transform.localPosition != axeHitPosition) {
            t += Time.deltaTime*3;
            axePrefab.transform.localPosition = new Vector3(
                                                        Mathf.Lerp(axeActualPosition.x,axeHitPosition.x,t),
                                                        Mathf.Lerp(axeActualPosition.y,axeHitPosition.y,t),
                                                        Mathf.Lerp(axeActualPosition.z,axeHitPosition.z,t)
                                                    );
            axePrefab.transform.localRotation = Quaternion.Euler(new Vector3(
                                                                Mathf.Lerp(axeActualRotation.x,axeHitRotation.x,t),
                                                                Mathf.Lerp(axeActualRotation.y,axeHitRotation.y,t),
                                                                Mathf.Lerp(axeActualRotation.z,axeHitRotation.z,t)
                                                            ));
            yield return new WaitForEndOfFrame();
        }
        axePrefab.transform.localPosition = axeHitPosition;

        yield return new WaitForSeconds(0.5f);

        if(precision01 > 0.9f)
        {
            print("Perfect");
            //Play sound
            Reward(3);
        }
        else if(precision01 > 0.7f) 
        {
            print("Almost");
            Reward(2);
        }
        else if(precision01 > 0.5f) 
        {
            print("I'll accept it");
            Reward(1);
        }
        else if(precision01 > 0.4f) 
        {
            print("I'll give you another chance");
            Reward(-1);
        }
        else 
        {
            print("You're out");
            StartCoroutine(EndGame(0));
            Reward(-4);
            yield break;
        }
        t=0;
        while(axePrefab.transform.localPosition != axeActualPosition) {
            t += Time.deltaTime/4;
            axePrefab.transform.localPosition = new Vector3(
                                                                Mathf.Lerp(axeHitPosition.x,axeActualPosition.x,t),
                                                                Mathf.Lerp(axeHitPosition.y,axeActualPosition.y,t),
                                                                Mathf.Lerp(axeHitPosition.z,axeActualPosition.z,t)
                                                            );
            axePrefab.transform.localRotation = Quaternion.Euler(new Vector3(
                                                                Mathf.Lerp(axeHitRotation.x,axeActualRotation.x,t),
                                                                Mathf.Lerp(axeHitRotation.y,axeActualRotation.y,t),
                                                                Mathf.Lerp(axeHitRotation.z,axeActualRotation.z,t)
                                                            ));
            yield return new WaitForEndOfFrame();
        }
        axePrefab.transform.localPosition = axeActualPosition;

        yield return new WaitForSeconds(1f);
        continueSpeed = true;
    }

    void Reward(int rewardCount) {
        sticks+=rewardCount;
        sticks = Mathf.Max(0,sticks);
        stickTextTemp.text = "Sticks: " + sticks;
    }
}
