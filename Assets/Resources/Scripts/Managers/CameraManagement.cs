using System.Collections;
using UnityEngine;
using Cinemachine;

public class CameraManagement : MonoBehaviour
{
    #region Singleton
    public static CameraManagement instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    #region Variables
    [Header("Variables")]
    [SerializeField] private float cameraChangeTime = .8f; //Time between camera blends
    [SerializeField] private float cameraSwitchThresholdHorizontal = 15; //Horizontal clamp distance (the number for each side)
    [SerializeField] private float cameraSwitchThresholdVertical = 30; //Vertical clamp distance (the number for each side)
    [SerializeField] private float velocityMultiplier = 1; //You know how to read mate?
    [SerializeField] private float returnAnimationTime = .5f; //Well, i think it's quite self explanatory

    private CinemachineVirtualCamera[] vcams; //LOTS
    private CinemachineBlenderSettings blender; //OF
    private CinemachineVirtualCamera activeCamera; //FUCKING
    private Quaternion activeCameraInitialRotation; //VARI
    private Quaternion[] initialRotations; //A
    private int dir; //B
    bool canChange = true; //L
    Touch mainTouch; //ES
    Vector2 startPos;
    #endregion

    #region Main Methods
    private void Start()
    {
        setUpCameras(); //Lol
    }

    private void Update()
    {
        moveCamera(); //Input management frame by frame
    }
    #endregion

    #region Camera Management
    private void setUpCameras()
    {
        cameraSwitchThresholdVertical += 5; //The number five is because it is used
        cameraSwitchThresholdHorizontal += 5;// later to not skip the clamp point because the movement was to fast.

        /*Creates and configures every "cameraBlend" from chinemachine with it's animation and shit
         i don't want to explain every single line so if you're curious read.*/
        vcams = GetComponentsInChildren<CinemachineVirtualCamera>();
        initialRotations = new Quaternion[vcams.Length];
        for (int i = 0; i < vcams.Length; i++)
        {
            initialRotations[i] = vcams[i].transform.rotation;
        }
        GetComponentInChildren<CinemachineBrain>().m_CustomBlends = ScriptableObject.CreateInstance<CinemachineBlenderSettings>();
        blender = GetComponentInChildren<CinemachineBrain>().m_CustomBlends;
        blender.m_CustomBlends = new CinemachineBlenderSettings.CustomBlend[vcams.Length * 2];

        activeCamera = vcams[vcams.Length - 1];
        activeCameraInitialRotation = initialRotations[vcams.Length - 1];

        int k = 0;
        for (int i = 0; i < vcams.Length; i++)
        {
            vcams[i].m_Priority = i;
            for (int j = 1; j < 3; j++)
            {
                int num = i + j - 1 + k; //**skeletonface** "im literally dead rn"
                blender.m_CustomBlends[num].m_From = vcams[i].name;
                blender.m_CustomBlends[num].m_To = vcams[i + j > vcams.Length - 1 ? i + j - vcams.Length : i + j].name;
                blender.m_CustomBlends[num].m_Blend.m_Style = CinemachineBlendDefinition.Style.EaseIn;
                blender.m_CustomBlends[num].m_Blend.m_Time = cameraChangeTime;
            }
            k++; //k is a funny letter
        }
    }

    /*Takes care of setting the priority order when the camera switches, made automatic by a lot of work
     and death desire*/
    public void SwitchCamera(int dir)
    {
        int[] newPriority = new int[vcams.Length];
        bool isDirRight = dir > 0 ? true : false;
        int changeLength = isDirRight ? vcams.Length - 1 : 0;
        if (dir == 0 || !canChange) { return; }
        for (int i = 0; i < vcams.Length; i++)
        {
            if (vcams[i].m_Priority != changeLength)
            {
                newPriority[i] = vcams[i].m_Priority + (isDirRight ? 1 : -1);
            }
            else
            {
                newPriority[i] = isDirRight ? 0 : vcams.Length - 1;
            }
        }
        for (int i = 0; i < vcams.Length; i++)
        {
            if(newPriority[i] == vcams.Length - 1)
            {
                activeCamera = vcams[i];
                activeCameraInitialRotation = initialRotations[i];
            }
            vcams[i].m_Priority = newPriority[i];

        }
        canChange = false;
        StartCoroutine(waitForEndOfAnimation(cameraChangeTime * 0.6f)); //Assures you wont be able to change until at leas a 60% of the animation has been completed
    }
    #endregion

    #region Input Management

    /*Input management*/
    private void moveCamera()
    {
        if(Input.touchCount == 0) { return; } //Assures the code only works when touching

        mainTouch = Input.GetTouch(0); //Get's the first touch on screen

        Vector2 iniAng = activeCameraInitialRotation.eulerAngles;

        /*Since the camera rotates from 0 to 360 i had to come up with a solution for clamping the camera when the angle goes from 0 to -360 or something, since the code
         adds the velocity directly and the camera won't work with 375� but 15�*/

        float x1 = (iniAng.x - cameraSwitchThresholdVertical < 0) ? iniAng.x - cameraSwitchThresholdVertical + 360 : iniAng.x - cameraSwitchThresholdVertical, 
              x2 = (iniAng.x + cameraSwitchThresholdVertical > 360) ? iniAng.x + cameraSwitchThresholdVertical - 360 : iniAng.x + cameraSwitchThresholdVertical;
        float y1 = (iniAng.y - cameraSwitchThresholdHorizontal < 0) ? iniAng.y - cameraSwitchThresholdHorizontal + 360 : iniAng.y - cameraSwitchThresholdHorizontal
            , y2 = (iniAng.y + cameraSwitchThresholdHorizontal > 360) ? iniAng.y + cameraSwitchThresholdHorizontal - 360 : iniAng.y + cameraSwitchThresholdHorizontal;

        float xHigh = x2 > x1 ? x2 : x1, //Makes life easier by setting the higher and the lower in new variables
            xLow = x2 > x1 ? x1 : x2,
            yHigh = y2 > y1 ? y2 : y1,
            yLow = y2 > y1 ? y1 : y2;
        
        switch (mainTouch.phase)
        {
            case TouchPhase.Began:
                    startPos = mainTouch.position;
                    break;
            case TouchPhase.Moved:
                if (!canChange) { return; }
                Vector2 velocity = mainTouch.deltaPosition;
                float yVel = velocity.x * Time.deltaTime * velocityMultiplier,
                    xVel = -velocity.y * Time.deltaTime * velocityMultiplier;

                activeCamera.transform.eulerAngles += new Vector3(xVel, yVel, 0); //This is what handles movement

                #region Useless Code
                /*No voy a borrar este codigo por lo que me ha costado que funcione, pero luego se me ha ocurrido una idea mejor que es lo que he dejado
                 por ser mas facil de entender, un 20% de todo el trabajo que he hecho el dia 22/10/2022 ha sido in�til, y quiero dejar constancia en el 
                comentario de este codigo
                @paraxodon*/
                //float y = activeCamera.transform.eulerAngles.y;
                //if (y1 > y2 ? (y >= yLow && y <= yLow + 10 || y > yHigh - 10 && y <= yHigh) : y >= yHigh || y <= yLow)
                //{
                //    activeCamera.transform.eulerAngles -= new Vector3(0, velocity.x * movementReducer, 0);
                //}

                //float x = activeCamera.transform.eulerAngles.x;
                //if (x1 > x2 ? (x >= xLow && x <= xLow + 10 || x > xHigh - 10 && x <= xHigh) : x >= xHigh || x <= xLow)
                //{
                //    activeCamera.transform.eulerAngles -= new Vector3(-velocity.y * movementReducer, 0, 0);
                //}
                #endregion

                /*Camera clamping.*/
                float numHy = Mathf.Abs(activeCamera.transform.eulerAngles.y - yHigh);
                float numLy = Mathf.Abs(activeCamera.transform.eulerAngles.y - yLow);
                if (numHy >= 0 && numHy < 5f || numLy >= 0 && numLy < 5f)
                {
                    activeCamera.transform.eulerAngles -= new Vector3(0, yVel, 0);
                }

                float numHx = Mathf.Abs(activeCamera.transform.eulerAngles.x - xHigh);
                float numLx = Mathf.Abs(activeCamera.transform.eulerAngles.x - xLow);
                if (numHx >= 0 && numHx < 5f || numLx >= 0 && numLx < 5f)
                {
                    activeCamera.transform.eulerAngles -= new Vector3(xVel, 0, 0);
                }

                break; 

            case TouchPhase.Ended:
                dir = activeCamera.transform.eulerAngles.y > activeCameraInitialRotation.eulerAngles.y ? 1 : -1; //Get's the camera direction by using the y initial rotation value

                float numH = Mathf.Abs(activeCamera.transform.eulerAngles.y - yHigh) - 5f;
                float numL = Mathf.Abs(activeCamera.transform.eulerAngles.y - yLow) - 5f;
                print("H: " + yHigh + " L: " + yLow);
                if ((numH >= 0 && numH < 1f || numL >= 0 && numL < 1f) && Vector2.Distance(startPos, mainTouch.position) > 100f)
                {
                    SwitchCamera(dir);
                }
                else StartCoroutine(returnToCenter()); //Return to center animation
                StartCoroutine(waitForEndOfAnimation(returnAnimationTime)); //Idk why i did this, but if it's not broken don't fix it
                break;
        }
    }
    #endregion

    #region Animation Handling

    //Self explanatory code idk
    private IEnumerator returnToCenter()
    {
        float timeToReturn = 0;
        while (activeCamera.transform.rotation != activeCameraInitialRotation)
        {
            activeCamera.transform.rotation = Quaternion.Lerp(activeCamera.transform.rotation, activeCameraInitialRotation, timeToReturn);
            timeToReturn += (Time.deltaTime / returnAnimationTime); //I had to do math to get this shit to calculate how many frames is x seconds
            yield return null; //Wait one frame
            if(Input.touchCount != 0) { break; }
        }
    }


    IEnumerator waitForEndOfAnimation(float seconds)
    {
        yield return new WaitForSeconds(seconds); //O_o
        canChange = true;
    }
    #endregion

    #region Utilities
    public static string getActiveCamera() //I don't know what this method does, it's super cryptic...
    {
        return instance.activeCamera.name;
    }
    #endregion
}
