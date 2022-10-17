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


    }
    //hola, Mi plan para quien lea esto es hacer una version que vaya con mouse y luego ampliarlo a tocar
    // Update is called once per frame
    void Update()
    {
        if (busy) { return; }

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
    /*
    public IEnumerator ReturnStick() {
        return 0;
    }
    */
}
