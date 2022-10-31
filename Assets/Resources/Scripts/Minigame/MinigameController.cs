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

    [Header("Stick variables")]
    public TMP_Text stickTextTemp;
    public int sticks;

    [Header("Other variables")]
    private float gameTime;
    private bool gameRunning = false;
    private Vector2 mousePosition;

    void Start() {
        Reward(0);
        axePrefab.transform.position = axeStartPosition;
    }

    void StartGame()
    {
        sticks = 0;
        Reward(0);
        continueSpeed = true;
        gameRunning = true;
        gameTime = Random.Range(0.0f,10.0f);
    }

    IEnumerator EndGame(int reason)
    {
        gameRunning = false;
        yield return new WaitForSeconds(1f);
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
        InteractableTouch.instance.stickNum+=sticks;
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

            if(Input.GetMouseButtonUp(0)) {
                RaycastHit hit; 
                Ray ray = Camera.main.ScreenPointToRay(Input.GetTouch(0).position); 
                if ( Physics.Raycast (ray,out hit,100.0f)) {
                    if(hit.collider.gameObject.CompareTag("Tree")) {
                        StartCoroutine(StopNeedle());
                    }
                }
            }
        }
        else {
            if(gameRunning) StartCoroutine(EndGame(-1));
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
            StartCoroutine(EndGame(0));
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
        while(axePrefab.transform.localPosition != axeEndPositionOffset) {
            t += Time.deltaTime*3;
            axePrefab.transform.localPosition = new Vector3(
                                                        Mathf.Lerp(axeStartPosition.x,axeEndPositionOffset.x,t),
                                                        Mathf.Lerp(axeStartPosition.y,axeEndPositionOffset.y,t),
                                                        Mathf.Lerp(axeStartPosition.z,axeEndPositionOffset.z,t)
                                                    );
            axePrefab.transform.localRotation = Quaternion.Euler(new Vector3(
                                                                Mathf.Lerp(axeStartRotation.x,axeEndRotationOffset.x,t),
                                                                Mathf.Lerp(axeStartRotation.y,axeEndRotationOffset.y,t),
                                                                Mathf.Lerp(axeStartRotation.z,axeEndRotationOffset.z,t)
                                                            ));
            yield return new WaitForEndOfFrame();
        }
        axePrefab.transform.localPosition = axeEndPositionOffset;
        yield return new WaitForSeconds(0.5f);
        t=0;
        while(axePrefab.transform.localPosition != axeStartPosition) {
            t += Time.deltaTime/4;
            axePrefab.transform.localPosition = new Vector3(
                                                                Mathf.Lerp(axeEndPositionOffset.x,axeStartPosition.x,t),
                                                                Mathf.Lerp(axeEndPositionOffset.y,axeStartPosition.y,t),
                                                                Mathf.Lerp(axeEndPositionOffset.z,axeStartPosition.z,t)
                                                            );
            axePrefab.transform.localRotation = Quaternion.Euler(new Vector3(
                                                                Mathf.Lerp(axeEndRotationOffset.x,axeStartRotation.x,t),
                                                                Mathf.Lerp(axeEndRotationOffset.y,axeStartRotation.y,t),
                                                                Mathf.Lerp(axeEndRotationOffset.z,axeStartRotation.z,t)
                                                            ));
            yield return new WaitForEndOfFrame();
        }
    }
}
