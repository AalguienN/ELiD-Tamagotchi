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
    bool canChange = true;

    private CinemachineVirtualCamera activeCamera;

    private void Start()
    {
        vcams = GetComponentsInChildren<CinemachineVirtualCamera>();
        GetComponentInChildren<CinemachineBrain>().m_CustomBlends =  ScriptableObject.CreateInstance<CinemachineBlenderSettings>();
        blender = GetComponentInChildren<CinemachineBrain>().m_CustomBlends;
        blender.m_CustomBlends = new CinemachineBlenderSettings.CustomBlend[vcams.Length * 2];

        activeCamera = vcams[vcams.Length - 1];

        int k = 0;
        for (int i = 0; i < vcams.Length; i++)
        {
            vcams[i].m_Priority = i;
            for (int j = 1; j < 3; j++)
            {
                int num = i + j - 1 + k; //Este codigo es demasiado complejo como para explicarlo, en su momento lo supe (hace 20 segundos)
                blender.m_CustomBlends[num].m_From = vcams[i].name;
                blender.m_CustomBlends[num].m_To = vcams[i + j > vcams.Length - 1 ? i + j - vcams.Length : i + j].name;
                blender.m_CustomBlends[num].m_Blend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
                blender.m_CustomBlends[num].m_Blend.m_Time = cameraChangeTime;
            }
            k++;
        }
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SwitchCamera(-1);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            SwitchCamera(1);
        }
#endif
    }
    public void SwitchCamera(int dir)
    {
        int[] newPriority = new int[vcams.Length];
        bool isDirRight = dir > 0 ? true : false;
        int changeLength = isDirRight ? vcams.Length : 0;
        if (dir == 0 || !canChange) { return; }
        for (int i = 0; i < vcams.Length; i++)
        {
            if (vcams[i].m_Priority != changeLength)
            {
                newPriority[i] = vcams[i].m_Priority + (isDirRight ? 1 : -1);
            }
            else
            {
                newPriority[i] = isDirRight ? 0 : vcams.Length;
            }
        }
        for (int i = 0; i < vcams.Length; i++)
        {
            if(newPriority[i] == vcams.Length - 1)
            {
                activeCamera = vcams[i];
            }
            vcams[i].m_Priority = newPriority[i];
        }
        canChange = false;
        StartCoroutine(waitForEndOfAnimation(0f));
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
