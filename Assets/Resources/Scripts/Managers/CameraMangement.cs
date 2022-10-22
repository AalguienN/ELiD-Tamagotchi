using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMangement : MonoBehaviour
{
    #region Singleton
    public static CameraMangement instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    private CinemachineVirtualCamera[] vcams;
    private CinemachineBlenderSettings blender;
    [SerializeField] private float cameraChangeTime = 2;
    [SerializeField] private float cameraSwitchThresholdY = 15;
    [SerializeField] private float cameraSwitchThresholdX = 30;
    [SerializeField] private float movementReducer = 0.05f;
    [SerializeField] private float returnTime = .5f;
    bool canChange = true;

    private CinemachineVirtualCamera activeCamera;
    private Quaternion activeCameraInitialRotation;
    private Quaternion[] initialRotations;
    private int dir;
    Touch mainTouch;
    private int constantToAssureWorking = 5;

    private void Start()
    {
        setUpCameras();
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchCamera(1);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            SwitchCamera(-1);
        }
#endif

        moveCamera();

    }
    private void setUpCameras()
    {
        cameraSwitchThresholdX += 5;
        cameraSwitchThresholdY += 5;
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
                int num = i + j - 1 + k; //Este codigo es demasiado raro, en su momento supe que era
                blender.m_CustomBlends[num].m_From = vcams[i].name;
                blender.m_CustomBlends[num].m_To = vcams[i + j > vcams.Length - 1 ? i + j - vcams.Length : i + j].name;
                blender.m_CustomBlends[num].m_Blend.m_Style = CinemachineBlendDefinition.Style.EaseIn;
                blender.m_CustomBlends[num].m_Blend.m_Time = cameraChangeTime;
            }
            k++;
        }
    }

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
        StartCoroutine(waitForEndOfAnimation(cameraChangeTime * 0.6f));
    }

    private enum clampCases
    {
        Higher,
        Lower,
        Normal
    };
    private void moveCamera()
    {
        if(Input.touchCount == 0) { return; }

        mainTouch = Input.GetTouch(0);

        Vector2 iniAng = activeCameraInitialRotation.eulerAngles;
        float x1 = (iniAng.x - cameraSwitchThresholdX < 0) ? iniAng.x - cameraSwitchThresholdX + 360 : iniAng.x - cameraSwitchThresholdX,
              x2 = (iniAng.x + cameraSwitchThresholdX > 360) ? iniAng.x + cameraSwitchThresholdX - 360 : iniAng.x + cameraSwitchThresholdX;
        float y1 = (iniAng.y - cameraSwitchThresholdY < 0) ? iniAng.y - cameraSwitchThresholdY + 360 : iniAng.y - cameraSwitchThresholdY
            , y2 = (iniAng.y + cameraSwitchThresholdY > 360) ? iniAng.y + cameraSwitchThresholdY - 360 : iniAng.y + cameraSwitchThresholdY;

        float xHigh = x2 > x1 ? x2 : x1,
            xLow = x2 > x1 ? x1 : x2,
            yHigh = y2 > y1 ? y2 : y1,
            yLow = y2 > y1 ? y1 : y2;

        switch (mainTouch.phase)
        {
            case TouchPhase.Moved:
                if (!canChange) { return; }
                Vector2 velocity = mainTouch.deltaPosition;
                activeCamera.transform.eulerAngles += new Vector3(-velocity.y * movementReducer, velocity.x * movementReducer, 0);
                dir = (int) Mathf.Sign(velocity.x);

                /*No voy a borrar este codigo por lo que me ha costado que funcione, pero luego se me ha ocurrido una idea mejor que es lo que he dejado
                 por ser mas facil de entender, un 20% de todo el trabajo que he hecho el dia 22/10/2022 ha sido inútil, y quiero dejar constancia en el 
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

                float numHy = Mathf.Abs(activeCamera.transform.eulerAngles.y - yHigh);
                float numLy = Mathf.Abs(activeCamera.transform.eulerAngles.y - yLow);
                if (numHy >= 0 && numHy < 5f || numLy >= 0 && numLy < 5f)
                {
                    activeCamera.transform.eulerAngles -= new Vector3(0, velocity.x * movementReducer, 0);
                }

                float numHx = Mathf.Abs(activeCamera.transform.eulerAngles.x - xHigh);
                float numLx = Mathf.Abs(activeCamera.transform.eulerAngles.x - xLow);
                if (numHx >= 0 && numHx < 5f || numLx >= 0 && numLx < 5f)
                {
                    activeCamera.transform.eulerAngles -= new Vector3(-velocity.y * movementReducer, 0, 0);
                }

                break; 

            case TouchPhase.Ended:
                float numH = Mathf.Abs(activeCamera.transform.eulerAngles.y - yHigh) - 5f;
                float numL = Mathf.Abs(activeCamera.transform.eulerAngles.y - yLow) - 5f;
                print("H: " + yHigh + " L: " + yLow);
                if (numH >= 0 && numH < 1f || numL >= 0 && numL < 1f)
                {
                    SwitchCamera(dir);
                }
                StartCoroutine(returnToCenter());
                StartCoroutine(waitForEndOfAnimation(returnTime));
                break;
        }
    }

    private IEnumerator returnToCenter()
    {
        float timeToReturn = 0;
        while (activeCamera.transform.rotation != activeCameraInitialRotation)
        {
            activeCamera.transform.rotation = Quaternion.Lerp(activeCamera.transform.rotation, activeCameraInitialRotation, timeToReturn);
            timeToReturn += (Time.deltaTime / returnTime);
            yield return null;
            if(Input.touchCount != 0) { break; }
        }
    }


    IEnumerator waitForEndOfAnimation(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        canChange = true;
    }

    public static string getActiveCamera()
    {
        return instance.activeCamera.name;
    }
}
