using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableResource : MonoBehaviour
{
    public Camera gameCamera;
    public GameObject sticky;
    public float grabR;

    public float pixelW;
    public float pixelH;

    public Vector3 pointer;
    Vector3 mousePos;
    Vector3 pixelPointer;

    Vector3 adjustZ;

    Vector2 rectangle;
    Vector3 rectangleCenter;

    public bool hold = false;
    public bool busy = false;
    public bool tried = false;
    // Start is called before the first frame update
    void Start()
    {
        //resolution setup
        Screen.SetResolution(720, 1080, true);
        pixelW = gameCamera.pixelWidth;
        pixelH = gameCamera.pixelHeight;

        //default stick position
        pointer = new Vector3((float)pixelW*0.5f,(float)pixelH*0.15f,10);
        pixelPointer = new Vector3((float)pixelW * 0.5f, (float)pixelH * 0.15f, 0);
        sticky.transform.position = gameCamera.ScreenToWorldPoint(pointer);

        //Grab distance
        grabR = pixelW * 0.2f;

        //NO MULTITOUCH
        Input.multiTouchEnabled = false;

        //Defining campfire rectangle hitbox
        rectangle = new Vector2(pixelW*.2f,pixelH*.3f);
        rectangleCenter = new Vector3(pixelW* .5f, pixelH * .5f);
        adjustZ = new Vector3(0, 0, 10);


    }
    //hola, Mi plan para quien lea esto es hacer una version que vaya con mouse y luego ampliarlo a tocar
    // Update is called once per frame
    void Update()
    {
        if (busy) { return; }

        if (Input.GetMouseButtonUp(0))
        {
            hold = false;
            busy = true;
            //check if rectangle
            mousePos = Input.mousePosition;
            if (mousePos.x > (rectangleCenter.x - rectangle.x) && mousePos.x < (rectangleCenter.x + rectangle.x))
            {
                if (mousePos.y > (rectangleCenter.y - rectangle.y) && mousePos.y < (rectangleCenter.y + rectangle.y))
                {
                    Debug.Log("bailecito");
                    Consume();
                    StartCoroutine("Recall");
                }
            }
            else
            {
                
                StartCoroutine("ReturnStick");
            }
        }

        if (hold)
        {
            mousePos = Input.mousePosition + adjustZ;
            sticky.transform.position = gameCamera.ScreenToWorldPoint(mousePos);
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            mousePos = Input.mousePosition;
            if (mousePos.x > (pixelPointer.x - grabR) && mousePos.x < (pixelPointer.x + grabR))
            {
                if (mousePos.y > (pixelPointer.y - grabR) && mousePos.y < (pixelPointer.y + grabR))
                {
                    hold = true;
                    Debug.Log("Anivia la criofénix");
                }
            }
        }
    }
    
    public IEnumerator ReturnStick() {

        Vector3 currPos, moveDis;
        Vector3 pointerWorld = gameCamera.ScreenToWorldPoint(pointer);
        for (int i = 0; i < 45; i++) {
            currPos = sticky.transform.position;
            moveDis = pointerWorld-currPos;
            sticky.transform.Translate(moveDis* .1f);
            yield return new WaitForEndOfFrame();
        }
        busy = false;
    }

    public IEnumerator Recall()
    {
        sticky.transform.position = gameCamera.ScreenToWorldPoint(pointer);
        int x = 120;
        Vector3 scaleChange = new Vector3(0.5f/((float)x), 1f/((float)x), 0.5f/((float)x));
        sticky.transform.localScale = new Vector3(0, 0, 0);
        for(int i = 0; i<x; i++)
        {
            yield return new WaitForEndOfFrame();
            sticky.transform.localScale += scaleChange;
        }
        busy = false;
    }
    public void Consume()
    {

    }
    
}
