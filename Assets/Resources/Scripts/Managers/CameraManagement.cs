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
    [SerializeField] public float cameraWallThresholdHorizontal = 15; //Horizontal clamp distance (the number for each side)
    [SerializeField] public float cameraWallThresholdVertical = 30; //Vertical clamp distance (the number for each side)
    [SerializeField] private float cameraSwitchThreshold = 2f; //Value getting the change of camara activated when it's this close to the borders
    [SerializeField] private float velocityMultiplier = 1; //You know how to read mate?
    [SerializeField] private float velocityReducerOnChangeThreshold = .3f; //When enters change threshold it slows down to a x% (0.3 30% example)
    [SerializeField] private float returnAnimationTime = .5f; //Well, i think it's quite self explanatory
    [SerializeField][Range(-1,1)] private int movementDirection = -1; 
    [HideInInspector] public float securityConstant = 5f; //Make sure everything works

    private CinemachineVirtualCamera[] vcams; //LOTS
    private CinemachineBlenderSettings blender; //OF
    [HideInInspector] public CinemachineVirtualCamera activeCamera; //FUCKING
    [HideInInspector] public Quaternion activeCameraInitialRotation; //VARI
    private Quaternion[] initialRotations; //A
    private int dir; //B
    bool canChange = true; //L
    [HideInInspector] public Touch mainTouch; //ES
    public bool strangeCase = false;

    public static bool blockCamera;

    [HideInInspector] public float x1, x2, y1, y2, xHigh, xLow, yHigh, yLow;

    #endregion

    #region Main Methods
    private void Start()
    {
        setUpCameras(); //Lol
        handleBarriers(); //Lol


        if ((activeCameraInitialRotation.eulerAngles.x - cameraWallThresholdVertical < 0) ||
        (activeCameraInitialRotation.eulerAngles.x + cameraWallThresholdVertical > 360))
        {
            strangeCase = true;
        }

#if UNITY_EDITOR
        //velocityMultiplier = 4;
#endif
    }

    private void Update()
    {
        if (!blockCamera)
        {
            moveCamera(); //Input management frame by frame
        }
        else
        {
            returnToCenter();
        }

        if (getActiveCamera().Equals("CamMinigame") && !SaveManager.hasTurnedRight)
        {
            SaveManager.hasTurnedRight = true;
            SaveManager.saveAll();
        }
        if (getActiveCamera().Equals("CamCalendar") && !SaveManager.hasTurnedLeft)
        {
            SaveManager.hasTurnedLeft = true;
            SaveManager.saveAll();
            DialogueEventStarter.instance.enableContinue();
        }

    }
#endregion

    #region Camera Management
    private void setUpCameras()
    {
        cameraWallThresholdVertical += securityConstant; //The number five is because it is used
        cameraWallThresholdHorizontal += securityConstant;// later to not skip the clamp point because the movement was to fast.

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
        if(SaveManager.getCanOnlyTurn() == 1) {
            if (dir == 1 && getActiveCamera().Equals("CamMinigame") || dir == -1 && getActiveCamera().Equals("CamBonfire"))
            {
                returnToCenter();
                return;
            }
        }
        else if (SaveManager.getCanOnlyTurn() == -1)
        {
            if (dir == -1 && getActiveCamera().Equals("CamCalendar") || dir == 1 && getActiveCamera().Equals("CamBonfire"))
            {
                returnToCenter();
                return;
            }
        }
        else if(SaveManager.getCanOnlyTurn() == 0)
        {
            returnToCenter();
            return;
        }
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
                ScreenShake.instance.setInitialRotation(initialRotations[i]);
            }
            vcams[i].m_Priority = newPriority[i];

        }
        /**/

        handleBarriers();

        /**/
        canChange = false;
        StartCoroutine(waitForEndOfAnimation(cameraChangeTime * 0.6f)); //Assures you wont be able to change until at leas a 60% of the animation has been completed

    }
    #endregion

    #region Input Management

    private void handleBarriers()
    {
        Vector2 iniAng = activeCameraInitialRotation.eulerAngles;

        /*Since the camera rotates from 0 to 360 i had to come up with a solution for clamping the camera when the angle goes from 0 to -360 or something, since the code
         adds the velocity directly and the camera won't work with 375� but 15�*/

        x1 = (iniAng.x - cameraWallThresholdVertical < 0) ? iniAng.x - cameraWallThresholdVertical + 360 : iniAng.x - cameraWallThresholdVertical;
        x2 = (iniAng.x + cameraWallThresholdVertical > 360) ? iniAng.x + cameraWallThresholdVertical - 360 : iniAng.x + cameraWallThresholdVertical;
        y1 = (iniAng.y - cameraWallThresholdHorizontal < 0) ? iniAng.y - cameraWallThresholdHorizontal + 360 : iniAng.y - cameraWallThresholdHorizontal;
        y2 = (iniAng.y + cameraWallThresholdHorizontal > 360) ? iniAng.y + cameraWallThresholdHorizontal - 360 : iniAng.y + cameraWallThresholdHorizontal;

        xHigh = x2 > x1 ? x2 : x1; //Makes life easier by setting the higher and the lower in new variables
        xLow = x2 > x1 ? x1 : x2;
        yHigh = y2 > y1 ? y2 : y1;
        yLow = y2 > y1 ? y1 : y2;
    }

    /*Input management*/
    private void moveCamera()
    {
        if(Input.touchCount == 0) { return; } //Assures the code only works when touching

        mainTouch = Input.GetTouch(0); //Get's the first touch on screen

        float numHY = Mathf.Abs(activeCamera.transform.eulerAngles.y - yHigh) - securityConstant;
        float numLY = Mathf.Abs(activeCamera.transform.eulerAngles.y - yLow) - securityConstant;

        bool switchThresholdHorizontal = (numHY >= cameraSwitchThreshold && numHY < securityConstant || numLY >= cameraSwitchThreshold && numLY < securityConstant);

        float numHX = Mathf.Abs(activeCamera.transform.eulerAngles.x - xHigh) - securityConstant;
        float numLX = Mathf.Abs(activeCamera.transform.eulerAngles.x - xLow) - securityConstant;
        
        bool switchThresholdVertical = ((strangeCase ? numLX : numHX) <= 1f);

        Vector2 velocity = mainTouch.deltaPosition; //Velocity configuration (to move through the screen)
        float yVel = velocity.x *  Time.fixedDeltaTime * movementDirection * (!switchThresholdHorizontal ? velocityMultiplier : velocityMultiplier * velocityReducerOnChangeThreshold),
            xVel = -velocity.y * Time.fixedDeltaTime * movementDirection * (!switchThresholdHorizontal ? velocityMultiplier : velocityMultiplier * velocityReducerOnChangeThreshold);


        switch (mainTouch.phase)
        {
            case TouchPhase.Moved:
                if (!canChange) { return; }

                activeCamera.transform.eulerAngles += (InteractableTouch.instance.hold ? new Vector3(0,0,0) : new Vector3(xVel , yVel, 0)); //This is what handles movement

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
                if (numHy >= 0 && numHy < securityConstant || numLy >= 0 && numLy < securityConstant)
                {
                    activeCamera.transform.eulerAngles -= new Vector3(0, yVel, 0);
                }

                float numHx = Mathf.Abs(activeCamera.transform.eulerAngles.x - xHigh);
                float numLx = Mathf.Abs(activeCamera.transform.eulerAngles.x - xLow);
                if (numHx >= 0 && numHx < securityConstant || numLx >= 0 && numLx < securityConstant)
                {
                    activeCamera.transform.eulerAngles -= new Vector3(xVel, 0, 0);
                }

                if (!switchThresholdHorizontal) //so it doesn't change at last moment
                {
                    dir = (int)Mathf.Sign(yVel); //Get's the camera direction by using the y initial rotation value (before it ends moving)
                }


                break; 

            case TouchPhase.Ended:
                if (switchThresholdHorizontal)
                {
                    SwitchCamera(dir);
                    StartCoroutine(returnToCenter());
                }
                else {
                    if (switchThresholdVertical){
                        //SaveManager.setBlueStickNum(SaveManager.getBlueStickNum() + 1);
                        //SaveManager.setStickNum(SaveManager.getStickNum() + 1); //TESTING
                        //print("added blue: " + SaveManager.getBlueStickNum());
                    }
                    StartCoroutine(returnToCenter());
                } //Return to center animation

                break;
        }
    }
    #endregion

    #region Animation Handling

    //Self explanatory code idk



    public IEnumerator returnToCenter()
    {
        float timeToReturn = 0;
        while (activeCamera.transform.rotation != activeCameraInitialRotation)
        {
            activeCamera.transform.rotation = Quaternion.Lerp(activeCamera.transform.rotation, activeCameraInitialRotation, timeToReturn);
            timeToReturn += (Time.deltaTime / returnAnimationTime); //I had to do math to get this shit to calculate how many frames is x seconds
            yield return null; //Wait one frame
            if(Input.touchCount != 0) { break; }
        }
        if (!getActiveCamera().Equals("CamBonfire") && DialogueEventStarter.instance.isCaputxaEnabled() && SaveManager.hasBeenDialoguePlayed)
        {
            DialogueEventStarter.instance.disableCaputxa();
        }

        //DAY 4 TURNING AND CAPUTXA APEARING
        if(SaveManager.getCurrentDay() == 4 && SaveManager.hasPinochioRun && !getActiveCamera().Equals("CamBonfire")){
            print("enabling caputxa after pinochio run");
            DialogueEventStarter.instance.enableCaputxa();
            SaveManager.hasPinochioRun = false;
            SaveManager.saveAll();
        }
        else if(SaveManager.getCurrentDay() == 5 && !getActiveCamera().Equals("CamBonfire") && SaveManager.hasBeenDialoguePlayed)
        {
            DialogueEventStarter.instance.disableCaputxas();
        }
    }


    IEnumerator waitForEndOfAnimation(float seconds)
    {
        canChange = false;
        yield return new WaitForSeconds(seconds); //O_o
        canChange = true;
    }

    public void blockCameraMethod(float seconds)
    {
        SaveManager.hasBeenCaputxaInteracted = true;
        StartCoroutine(blockingCamera(seconds));
    }

    IEnumerator blockingCamera(float seconds)
    {
        blockCamera = true;
        yield return new WaitForSeconds(seconds); //O_o
        blockCamera = false;
    }

    
    #endregion

    #region Utilities
    public static string getActiveCamera() //I don't know what this method does, it's super cryptic...
    {
        return instance.activeCamera.name;
    }
    #endregion
}
