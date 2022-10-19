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
    public Vector3 axeEndPositionOffset;
    public Vector3 axeEndRotationOffset;

    [Header("Needle variables")]
    public GameObject precisionNeedle;
    public float precision;
    private float precision01;
    public float speed = 1f;
    private bool continueSpeed = false;
    private bool gameRunning = false;

    private float gameTime;

    [Header("Stick variables")]
    public TMP_Text stickTextTemp;
    public int sticks;

    void Start() {
         axePrefab.transform.position = axeStartPosition;
    }

    void StartGame()
    {
        sticks = 0;
        continueSpeed = true;
        gameRunning = true;
        gameTime = Random.Range(0.0f,10.0f);
    }

    IEnumerator EndGame()
    {
        yield return new WaitForSeconds(1f);
        yield return new WaitForEndOfFrame();
        gameRunning = false;
        continueSpeed = false;
        gameTime = 0;
        speed = 0;
        precision = 0;
        precisionNeedle.transform.rotation = Quaternion.Euler(0f,0f,0f);
        int s = SaveManager.getStickNum();
        SaveManager.setStickNum(s+sticks);
    }

    // Update is called once per frame
    void Update()
    {
        //axePrefab.transform.position = axeStartPosition+ axeEndPositionOffset;
        if(!gameRunning) {
            if(Input.GetMouseButtonDown(0)) { 
                StartGame(); 
            }
            return;
        }
        if(!continueSpeed)
            return;
        gameTime += Time.deltaTime;
        speed = Mathf.Clamp(speed+Time.deltaTime*Time.deltaTime*speed, 1f, 15f);
        precision = Mathf.Sin(gameTime*speed)*80f;
        precisionNeedle.transform.rotation = Quaternion.Euler(0f,0f,precision);

        if(Input.GetMouseButtonDown(0)) {
            StartCoroutine(StopNeedle());
        }
    }

    IEnumerator StopNeedle() {
        continueSpeed = false;
        speed += (0.5f-Mathf.Abs(precision)/80f)*2f;
        gameTime = Random.Range(0.0f,10.0f);
        precision01 = 1f - Mathf.Abs(precision/80f);
        if(precision01 > 0.9f)
        {
            print("Perfect");
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
            StartCoroutine(EndGame());
            Reward(-4);
        }
        StartCoroutine(MoveAxe());
        yield return new WaitForSeconds(1f);
        continueSpeed = true;
    }

    void Reward(int rewardCount) {
        sticks+=rewardCount;
        sticks = Mathf.Max(0,sticks);
        stickTextTemp.text = "Sticks: " + sticks;
    }

    IEnumerator MoveAxe() {
        float t = 0;
        while(axePrefab.transform.position != axeEndPositionOffset) {
            t += Time.deltaTime*3;
            axePrefab.transform.position = new Vector3(
                                                        Mathf.Lerp(axeStartPosition.x,axeEndPositionOffset.x,t),
                                                        Mathf.Lerp(axeStartPosition.y,axeEndPositionOffset.y,t),
                                                        Mathf.Lerp(axeStartPosition.z,axeEndPositionOffset.z,t)
                                                    );
            axePrefab.transform.rotation = Quaternion.Euler(new Vector3(
                                                                Mathf.Lerp(axeStartRotation.x,axeEndRotationOffset.x,t),
                                                                Mathf.Lerp(axeStartRotation.y,axeEndRotationOffset.y,t),
                                                                Mathf.Lerp(axeStartRotation.z,axeEndRotationOffset.z,t)
                                                            ));
            yield return new WaitForEndOfFrame();
        }
        axePrefab.transform.position = axeEndPositionOffset;
        yield return new WaitForSeconds(0.15f);
        t=0;
        while(axePrefab.transform.position != axeStartPosition) {
            t += Time.deltaTime;
            axePrefab.transform.position = new Vector3(
                                                                Mathf.Lerp(axeEndPositionOffset.x,axeStartPosition.x,t),
                                                                Mathf.Lerp(axeEndPositionOffset.y,axeStartPosition.y,t),
                                                                Mathf.Lerp(axeEndPositionOffset.z,axeStartPosition.z,t)
                                                            );
            axePrefab.transform.rotation = Quaternion.Euler(new Vector3(
                                                                Mathf.Lerp(axeEndRotationOffset.x,axeStartRotation.x,t),
                                                                Mathf.Lerp(axeEndRotationOffset.y,axeStartRotation.y,t),
                                                                Mathf.Lerp(axeEndRotationOffset.z,axeStartRotation.z,t)
                                                            ));
            yield return new WaitForEndOfFrame();
        }
    }
}
