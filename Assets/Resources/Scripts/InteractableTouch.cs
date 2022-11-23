using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class InteractableTouch : MonoBehaviour
{
    //Singleton
    public static InteractableTouch instance;
    private void Awake()
    {
        instance = this;
    }

    #region Variables
    public Camera gameCamera;
    public GameObject sticky;
    public float grabR;

    public float pixelW;
    public float pixelH;

    public Vector3 pointer;
    Vector3 mousePos;
    Vector3 pixelPointer;
    Vector3 adjustZ;
    public int stickNum;

    Vector2 rectangle;
    Vector3 rectangleCenter;

    Vector3 initialPos;

    public bool hold = false;
    public bool busy = false;
    public bool tried = false;
    public bool visible = true;
    public bool isBlue = false; //Is the current stick in hand blue?
    bool isActive;

    #endregion

    #region Start Set-up
    // Start is called before the first frame update
    void Start()
    {
        //resolution setup
        //Screen.SetResolution(720, 1080, true);
        pixelW = gameCamera.pixelWidth;
        pixelH = gameCamera.pixelHeight;

        //default stick position
        pointer = new Vector3((float)pixelW * 0.5f, (float)pixelH * 0.15f, 3f);
        pixelPointer = new Vector3((float)pixelW * 0.5f, (float)pixelH * 0.15f, 0);
        sticky.transform.position = gameCamera.ScreenToWorldPoint(pointer);

        //Hacer que salga el palo normal
        changeWoodColor(false);

        //Grab distance
        grabR = pixelW * 0.2f;

        //NO MULTITOUCH
        Input.multiTouchEnabled = false;

        //Defining campfire rectangle hitbox -> rectangel is where the family is
        rectangle = new Vector2(pixelW * .2f, pixelH * .3f);
        rectangleCenter = new Vector3(pixelW * .5f, pixelH * .5f);
        adjustZ = new Vector3(0, 0, 3f);


        stickNum = SaveManager.getStickNum();
        if(stickNum <= 0)
        {
            sticky.SetActive(false);
        }
        // stickNum = 2; testing

        visible = sticky.activeSelf;
        initialPos = sticky.transform.position;
    }

    #endregion

    #region Actual Code
    void Update()
    {
        
        stickNum = SaveManager.getStickNum();
        if (SaveManager.getBlueStickNum() > 0 && !isBlue)
        {
            print("is blue");
            if(SaveManager.getBlueStickNum() >= 999)
            {
                changeWoodColor(true,true);
            }
            else
            {
                changeWoodColor(true);
            }
            isBlue = true;
        }
        else if (SaveManager.getBlueStickNum() <= 0 && isBlue)
        {
            print("is not blue");
            changeWoodColor(false);
            isBlue = false;
        }
        if (!visible && stickNum > 0) //Reappear sticks when reget
        {
            //if blue wood variable > 0 then recolor the normal wood and activate isBlue
            
            sticky.SetActive(true);
            StartCoroutine(Recall());
        }
        else if (visible && stickNum <= 0){
            sticky.SetActive(false);
        }
        visible = sticky.activeSelf;

        //if not looking at this camera dont do shit, or if busy or if empty , at this point just dont do anything
        if (!visible) { return; }
        if (!CameraManagement.getActiveCamera().Equals("CamBonfire")) { return; }
        if (Input.touchCount == 0) { return; }

        Touch fingertouch = Input.GetTouch(0);
        //untouch with the finger
        if (fingertouch.phase == TouchPhase.Ended)
        {
            if (!hold) { return; }
            hold = false;
            CameraManagement.blockCamera = false;

            //check if rectangle
            mousePos = fingertouch.position;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100.0f) && hit.collider.CompareTag("Bonfire")) {
                Debug.Log("Palo soltado a la hoguera");
                if (!isBlue) {
                    Consume();
                }
                else {
                    ConsumeBlue();
                }
                if (stickNum <= 0)
                {
                    //disable
                    sticky.SetActive(false);
                }
                Debug.Log("Recall");
                StartCoroutine("Recall");
            }
            else
            {
                Debug.Log("Return");
                StartCoroutine("ReturnStick");
            }
            // if (mousePos.x > (rectangleCenter.x - rectangle.x) && mousePos.x < (rectangleCenter.x + rectangle.x) 
            //     && mousePos.y > (rectangleCenter.y - rectangle.y) && mousePos.y < (rectangleCenter.y + rectangle.y))

            // {
            //         Debug.Log("Palo soltado a la hoguera");
            //         Consume();
            //         if (stickNum <= 0)
            //         {
            //             //disable
            //             sticky.GetComponent<MeshRenderer>().enabled = false;
            //             visible = false;
            //         }
            //         busy = true;
            //         Debug.Log("Recall");
            //         StartCoroutine("Recall");
            // }
            // else
            // {
            //     busy = true;
            //     Debug.Log("Return");
            //     StartCoroutine("ReturnStick");
            // }
        }

        //touch with the finger 
        if (fingertouch.phase == TouchPhase.Began)
        {
            mousePos = fingertouch.position;
            RaycastHit hit; 
            Ray ray = Camera.main.ScreenPointToRay(mousePos); 
            if (Physics.Raycast (ray,out hit,100.0f)){
                if(hit.collider.gameObject.CompareTag("Stick")) {
                    hold = true;
                    CameraManagement.blockCamera = true;
                }
            }
            // if (mousePos.x > (pixelPointer.x - grabR) && mousePos.x < (pixelPointer.x + grabR) 
            //     && mousePos.y > (pixelPointer.y - grabR) && mousePos.y < (pixelPointer.y + grabR))
            // {
            //    hold = true;
            // }
        }

        //a toothpick changes everything
        if (hold)
        {
            mousePos = new Vector3(fingertouch.position.x, fingertouch.position.y, adjustZ.z);
            sticky.transform.position = gameCamera.ScreenToWorldPoint(mousePos);
            if(sticky.GetComponent<Collider>().enabled) {
                sticky.GetComponent<Collider>().enabled = false;
            }
            return;
        }
        else if(!sticky.GetComponent<Collider>().enabled) {
            sticky.GetComponent<Collider>().enabled = true;
        }

    }

    //back with the moves
    public void changeWoodColor(bool isBlue, bool isChild = false)
    {
        //here change color
        if (isBlue)
        {
            if(isChild)
            {
                sticky.transform.GetChild(0).gameObject.SetActive(false);
                sticky.transform.GetChild(1).gameObject.SetActive(true);
                sticky.transform.GetChild(2).gameObject.SetActive(false);
            }
            else
            {
                sticky.transform.GetChild(0).gameObject.SetActive(false);
                sticky.transform.GetChild(1).gameObject.SetActive(false);
                sticky.transform.GetChild(2).gameObject.SetActive(true);
            }
            
        }
        else
        {
            sticky.transform.GetChild(0).gameObject.SetActive(true);
            sticky.transform.GetChild(1).gameObject.SetActive(false);
            sticky.transform.GetChild(2).gameObject.SetActive(false);
        }
    }
    public IEnumerator ReturnStick()
    {

        Vector3 currPos, moveDis;
        Vector3 pointerWorld = initialPos;

        float t = 0;
        while(sticky.transform.position != initialPos)
        {
            t += Time.deltaTime;
            sticky.transform.position = Vector3.Lerp(sticky.transform.position, initialPos, t);
            if (hold) { yield break; }
            yield return new WaitForEndOfFrame();
        }
        // for (int i = 0; i < 80; i++)
        // {
        //     currPos = sticky.transform.position;
        //     moveDis = pointerWorld - currPos;
        //     sticky.transform.Translate(moveDis * .12f);
        //     yield return new WaitForEndOfFrame();
        // }
    }

    //back without the moves
    public IEnumerator Recall()
    {
        sticky.transform.position = initialPos;
        int x = GameManager.instance.frameRate;
        Vector3 scaleChange = new Vector3(1f / ((float)x), 1f / ((float)x), 1f / ((float)x));
        sticky.transform.localScale = new Vector3(0, 0, 0);
        for (int i = 0; i < x; i++)
        {
            yield return new WaitForEndOfFrame();
            sticky.transform.localScale += scaleChange;
        }
    }

    //eat the fod
    public void Consume()
    {
        if (!SaveManager.hasBurntFirstStick)
        {
            SaveManager.hasBurntFirstStick = true;
            SaveManager.saveAll();
        }
        SaveManager.setStickNum(--stickNum);
        Debug.Log(stickNum + " palos restantes");
        //Modificado por Adri�n
        GameObject.FindGameObjectWithTag("Bonfire2").GetComponent<BonfireState>().addFuel(Fuel.types.stick);
    }
    //Im blue
    public void ConsumeBlue()
    {
        if (!SaveManager.hasBurntFirstBlueStick)
        {
            SaveManager.hasBurntFirstBlueStick = true;
            SaveManager.saveAll();
        }
        if(SaveManager.getStickNum() >= 999) //Muñeco, disfrazado de un numero muy alto (chapuza pero va :D)
        {
            SaveManager.setStickNum(stickNum - 999);
            SaveManager.setBlueStickNum(SaveManager.getBlueStickNum() - 999);
            SaveManager.setBlueWood(true);
            SaveManager.hasBurntLastBlueStick = true;
            SaveManager.saveAll();
            GameObject.FindGameObjectWithTag("Bonfire2").GetComponent<BonfireState>().addFuel(Fuel.types.blueStick);
            GameObject.FindGameObjectWithTag("Bonfire2").GetComponent<BonfireState>().hp = GameObject.FindGameObjectWithTag("Bonfire2").GetComponent<BonfireState>().maxHp;
            SaveManager.fuelList[SaveManager.fuelList.Count-1].duration = Mathf.Infinity;
        }
        else
        {
            SaveManager.setStickNum(--stickNum);
            SaveManager.setBlueStickNum(SaveManager.getBlueStickNum() - 1); //Soy Pau, he a�adido el azul
            SaveManager.setBlueWood(true);
            print("Consumido un palo azul");
            GameObject.FindGameObjectWithTag("Bonfire2").GetComponent<BonfireState>().addFuel(Fuel.types.blueStick);
        }
    }
    #endregion
}
