using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraMangement : MonoBehaviour
{
    [SerializeField] private CinemachineVirtualCamera[] vcams = new CinemachineVirtualCamera[3];
    bool canChange = true;


    private void Start()
    {
        for (int i = 0; i < vcams.Length; i++)
        {
            vcams[i].m_Priority = i;

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
        //NOTE TO SELF: Make that camera changes don't stack
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
            vcams[i].m_Priority = newPriority[i];
        }
        canChange = false;
        StartCoroutine(waitForEndOfAnimation(1.5f));
    }

    IEnumerator waitForEndOfAnimation(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        canChange = true;
    }
}
