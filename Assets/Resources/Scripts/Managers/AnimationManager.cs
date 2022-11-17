using System.Collections;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    #region Singleton
    public static AnimationManager instance;
    private void Awake()
    {
        instance = this;
    }
    #endregion

    [SerializeField] float handAnimationThreshold = 3f;

    private void Start()
    {
    }

    private void Update()
    {
    }

    public static IEnumerator cameraLockAnimation(float seconds)
    {
        CameraManagement.blockCamera = true;
        yield return new WaitForSeconds(seconds);
        CameraManagement.blockCamera = false;
    }
}
