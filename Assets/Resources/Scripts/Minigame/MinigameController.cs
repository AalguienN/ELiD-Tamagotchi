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
    public Vector3 axeHitPosition;
    public Vector3 axeHitRotation;
    Vector3 axeActualPosition;
    Vector3 axeActualRotation;
    bool modifyRotation = false;

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

    public static bool status;
    public GameObject treeFull;
    public GameObject treeChop;
    int savedHits;

    public GameObject particleInstancer;

    void Start() {
        axePrefab.transform.localPosition = axeActualPosition = axeStartPosition;
        axeActualRotation = axeStartRotation;
        axePrefab.transform.localRotation = Quaternion.Euler(axeActualRotation);

        print("Minigame minutes: " + SaveManager.getMinigameEndingDate());

        savedHits = SaveManager.getMinigameHits();
        sticks = SaveManager.getMinigameSticks();
        status = ((System.DateTime.Now-SaveManager.getMinigameEndingDate()).TotalMinutes >= 15);
        Reward(0);
        if(status) 
        {
            if(sticks != 0) StartCoroutine(StartGameAtHalf());
            treeChop.SetActive(false);
            treeFull.SetActive(true);
        }
        else
        {
            StartCoroutine(RunTime());
            treeFull.SetActive(false);
            treeChop.SetActive(true);
        }
    }

    IEnumerator StartGame()
    {
        sticks = 0;
        SaveManager.setMinigameSticks(0);
        savedHits = 0;
        SaveManager.setMinigameHits(0);
        
        treeChop.SetActive(false);
        treeFull.SetActive(true);
        treeChop.transform.GetChild(1).transform.localPosition = new Vector3(0, 0, 0);
        treeChop.transform.GetChild(1).transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        speed = 0;
        gameRunning = true;
        Reward(0);
        float t = 0;
        modifyRotation = true;
        while(t<=1) {
            t += Time.deltaTime/2f;
            axeActualPosition = Vector3.Lerp(axeStartPosition,axeActualPosition,t);
            axeActualRotation = Vector3.Lerp(axeStartRotation,axeActualRotation,t);
            yield return new WaitForEndOfFrame();
        }
        modifyRotation = false;
        continueSpeed = true;
        gameTime = Random.Range(0.0f,10.0f);
    }

    IEnumerator StartGameAtHalf() {
        treeChop.SetActive(false);
        treeFull.SetActive(true);
        treeChop.transform.GetChild(1).transform.localPosition = new Vector3(0, -1.17f, 0);
        treeChop.transform.GetChild(1).transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
        speed = 0;
        gameRunning = true;
        Reward(0);
        float t = 0;
        modifyRotation = true;
        while(t<=1) {
            t += Time.deltaTime/2f;
            axeActualPosition = Vector3.Lerp(axeStartPosition,axeActualPosition,t);
            axeActualRotation = Vector3.Lerp(axeStartRotation,axeActualRotation,t);
            yield return new WaitForEndOfFrame();
        }
        modifyRotation = false;
        continueSpeed = true;
        gameTime = Random.Range(0.0f,10.0f);
    }

    IEnumerator EndGame(int reason)
    {
        print("Game has ended");
        gameRunning = false;
        float t = 0;
        while(modifyRotation) yield return new WaitForEndOfFrame();
        modifyRotation = true;
        
        yield return new WaitForEndOfFrame();
        axeActualPosition = axeStartPosition;
        axeActualRotation = axeStartRotation;
        if(reason==-1) {
            if(CameraManagement.getActiveCamera()=="CamMinigame") {
                gameRunning = true; yield break;
            }
        }
        continueSpeed = false;
        gameTime = 0;
        precision = 0;
        precisionNeedle.transform.rotation = Quaternion.Euler(0f,0f,0f);
        int s = SaveManager.getStickNum();
        int bs = SaveManager.getBlueStickNum();
        if(sticks>=12 && SaveManager.getCurrentDay() >= 3) //Si haces casi full perfect
        {
            SaveManager.setBlueStickNum(bs+1);
            SaveManager.setStickNum(s+1);
        }
        else 
        {
            SaveManager.setStickNum(s+sticks);
        }

        print("Stick number: " + SaveManager.getStickNum());
        
        SaveManager.setMinigameEndingDate(System.DateTime.Now);
        SaveManager.setMinigameHits(0);
        SaveManager.setMinigameSticks(0);
        savedHits = 0;
        sticks = 0;
        StartCoroutine(RunTime());
        treeFull.SetActive(false);
        treeChop.SetActive(true);
        SoundManager.instance.PlaySound("TreeFall");
    }

    IEnumerator RunTime() {
        bool s = true;
        while(s) {
            s = !((System.DateTime.Now-SaveManager.getMinigameEndingDate()).TotalMinutes >= 15);
            
            yield return new WaitForEndOfFrame();
        }
        while(CameraManagement.getActiveCamera()=="CamMinigame") yield return new WaitForEndOfFrame();
        status = true;
        sticks = 0;
        SaveManager.setMinigameSticks(0);
        savedHits = 0;
        SaveManager.setMinigameHits(0);
        
        treeChop.SetActive(false);
        treeFull.SetActive(true);
        treeChop.transform.GetChild(1).transform.localPosition = new Vector3(0, 0, 0);
        treeChop.transform.GetChild(1).transform.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0));
    }

    // Update is called once per frame
    void Update()
    {
        axePrefab.transform.localPosition = Vector3.Lerp(axePrefab.transform.localPosition, axeActualPosition, Time.deltaTime*10f*speed);
        axePrefab.transform.localRotation = Quaternion.Lerp(axePrefab.transform.localRotation, Quaternion.Euler(axeActualRotation), Time.deltaTime*10f*speed);
        if(CameraManagement.getActiveCamera()=="CamMinigame") {
            //axePrefab.transform.position = axeStartPosition+ axeEndPositionOffset;
            if(!gameRunning) {
                if(Input.GetMouseButtonDown(0)) {
                    mousePosition = Input.mousePosition;
                }
                if(Input.GetMouseButtonUp(0)) { 
                    if(Vector3.Distance(mousePosition, Input.mousePosition) < 50f)
                        if(((System.DateTime.Now-SaveManager.getMinigameEndingDate()).TotalMinutes >= 15))
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
            if(!modifyRotation){
                axeActualPosition = Vector3.Lerp(axeMinPosition,axeMaxPosition,1f - Mathf.Abs(precision/80f));
                axeActualRotation = Vector3.Lerp(axeMinRotation,axeMaxRotation,1f - Mathf.Abs(precision/80f));
            }

            if(Input.GetMouseButtonDown(0)) {
                mousePosition = Input.mousePosition;
            }
            if(Input.GetMouseButtonUp(0)) { 
                if(Vector3.Distance(mousePosition, Input.mousePosition) < 50f)
                    StartCoroutine(StopNeedle());
            }
        }
    }

    IEnumerator StopNeedle() {
        savedHits++;
        SaveManager.setMinigameHits(savedHits);
        continueSpeed = false;
        speed += (0.5f-Mathf.Abs(precision)/80f)*2f;
        gameTime = Random.Range(0.0f,10.0f);
        precision01 = 1f - Mathf.Abs(precision/80f);
        
        while(modifyRotation) yield return new WaitForEndOfFrame();
        modifyRotation = true;
        float t = 0;
        Vector3 lastKnownPos = axeActualPosition;
        Vector3 lastKnownRot = axeActualRotation;
        while(axeActualPosition != axeHitPosition) {
            t += Time.deltaTime*3;
            axeActualPosition = new Vector3(
                                                Mathf.Lerp(lastKnownPos.x,axeHitPosition.x,t),
                                                Mathf.Lerp(lastKnownPos.y,axeHitPosition.y,t),
                                                Mathf.Lerp(lastKnownPos.z,axeHitPosition.z,t)
                                            );
            axeActualRotation = new Vector3(
                                                Mathf.Lerp(lastKnownRot.x,axeHitRotation.x,t),
                                                Mathf.Lerp(lastKnownRot.y,axeHitRotation.y,t),
                                                Mathf.Lerp(lastKnownRot.z,axeHitRotation.z,t)
                                            );
            yield return new WaitForEndOfFrame();
        }
        axeActualPosition = axeHitPosition;
        
        if(precision01 > 0.9f)
        {
            print("Perfect");
            //Play sound
            ScreenShake.instance.StartShake(0.1f,5f);
            SoundManager.instance.PlaySound("WoodChopOption (5)");
            Reward(3);
        }
        else if(precision01 > 0.7f) 
        {
            print("Almost");
            ScreenShake.instance.StartShake(0.1f,3f);
            SoundManager.instance.PlaySound("WoodChopOption (4)");
            Reward(2);
        }
        else if(precision01 > 0.5f) 
        {
            print("I'll accept it");
            ScreenShake.instance.StartShake(0.1f,1f);
            SoundManager.instance.PlaySound("WoodChopOption (3)");
            Reward(1);
        }
        else if(precision01 > 0.4f) 
        {
            print("I'll give you another chance");
            ScreenShake.instance.StartShake(0.1f,0.5f);
            SoundManager.instance.PlaySound("WoodChopOption (2)");
            Reward(0);
        }
        else 
        {
            print("You're out");
            Reward(-1);
        }

        yield return new WaitForSeconds(0.5f);
        t=0;
        while(axeActualPosition != axeHitPosition) {
            t += Time.deltaTime/4;
            axeActualPosition = new Vector3(
                                                Mathf.Lerp(axeHitPosition.x,lastKnownPos.x,t),
                                                Mathf.Lerp(axeHitPosition.y,lastKnownPos.y,t),
                                                Mathf.Lerp(axeHitPosition.z,lastKnownPos.z,t)
                                            );
            axeActualRotation = new Vector3(
                                                Mathf.Lerp(axeHitRotation.x,lastKnownRot.x,t),
                                                Mathf.Lerp(axeHitRotation.y,lastKnownRot.y,t),
                                                Mathf.Lerp(axeHitRotation.z,lastKnownRot.z,t)
                                            );
            yield return new WaitForEndOfFrame();
        }
        axeActualPosition = lastKnownPos;
        modifyRotation = false;
        if(savedHits>=5)
        {
            StartCoroutine(EndGame(0));
            yield break;
        }
        yield return new WaitForSeconds(1f);
        continueSpeed = true;
        
    }

    void Reward(int rewardCount) {
        sticks+=rewardCount;
        sticks = Mathf.Max(0,sticks);
        SaveManager.setMinigameSticks(sticks);
        stickTextTemp.text = "Sticks: " + sticks + " Hits: " + savedHits;
    }
}
