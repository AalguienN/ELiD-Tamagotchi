using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        //Grab distance
        grabR = pixelW * 0.2f;

        //NO MULTITOUCH
        Input.multiTouchEnabled = false;

        //Defining campfire rectangle hitbox -> rectangel is where the family is
        rectangle = new Vector2(pixelW * .2f, pixelH * .3f);
        rectangleCenter = new Vector3(pixelW * .5f, pixelH * .5f);
        adjustZ = new Vector3(0, 0, 3f);


        stickNum = SaveManager.getStickNum();
        // stickNum = 2; testing


        initialPos = sticky.transform.position;
    }

    #endregion

    #region Actual Code
    void Update()
    {
        //if not looking at this camera dont do shit, or if busy or if empty , at this point just dont do anything
        if (busy || !visible) { return; }
        if (!CameraManagement.getActiveCamera().Equals("CamBonfire")) { return; }
        if (Input.touchCount == 0) { return; }

        Touch fingertouch = Input.GetTouch(0);

        //untouch with the finger
        if (fingertouch.phase == TouchPhase.Ended)
        {
            if (!hold) { return; }
            hold = false;

            //check if rectangle
            mousePos = fingertouch.position;
            if (mousePos.x > (rectangleCenter.x - rectangle.x) && mousePos.x < (rectangleCenter.x + rectangle.x) 
                && mousePos.y > (rectangleCenter.y - rectangle.y) && mousePos.y < (rectangleCenter.y + rectangle.y))

            {
                    Debug.Log("Palo soltado a la hoguera");
                    Consume();
                    if (stickNum <= 0)
                    {
                        //disable
                        sticky.GetComponent<MeshRenderer>().enabled = false;
                        visible = false;
                    }
                    busy = true;
                    Debug.Log("Recall");
                    StartCoroutine("Recall");
            }
            else
            {
                busy = true;
                Debug.Log("Return");
                StartCoroutine("ReturnStick");
            }
        }

        //a toothpick changes everything
        if (hold)
        {
            mousePos = new Vector3(fingertouch.position.x, fingertouch.position.y, adjustZ.z);
            sticky.transform.position = gameCamera.ScreenToWorldPoint(mousePos);
            return;
        }

        //touch with the finger 
        if (fingertouch.phase == TouchPhase.Began)
        {
            mousePos = fingertouch.position;
            if (mousePos.x > (pixelPointer.x - grabR) && mousePos.x < (pixelPointer.x + grabR) 
                && mousePos.y > (pixelPointer.y - grabR) && mousePos.y < (pixelPointer.y + grabR))
            {
                    hold = true;
            }
        }
    }

    //back with the moves
    public IEnumerator ReturnStick()
    {

        Vector3 currPos, moveDis;
        Vector3 pointerWorld = initialPos;
        for (int i = 0; i < 40; i++)
        {
            currPos = sticky.transform.position;
            moveDis = pointerWorld - currPos;
            sticky.transform.Translate(moveDis * .12f);
            yield return new WaitForEndOfFrame();
        }
        busy = false;
    }

    //back without the moves
    public IEnumerator Recall()
    {
        sticky.transform.position = initialPos;
        int x = GameManager.instance.frameRate;
        Vector3 scaleChange = new Vector3(0.1f / ((float)x), .25f / ((float)x), 0.1f / ((float)x));
        sticky.transform.localScale = new Vector3(0, 0, 0);
        for (int i = 0; i < x; i++)
        {
            yield return new WaitForEndOfFrame();
            sticky.transform.localScale += scaleChange;
        }
        busy = false;
    }

    //eat the fod
    public void Consume()
    {
        SaveManager.setStickNum(--stickNum);
        Debug.Log(stickNum + " palos restantes");
    }
    //Im blue
    public void ConsumeBlue()
    {
        SaveManager.setStickNum(--stickNum);
        Debug.Log(stickNum + " palos restantes");
    }
    #endregion
}
