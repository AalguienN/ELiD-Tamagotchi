using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

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
    //[SerializeField] Animator anim;
    //[HideInInspector] public bool lockHand = false;

    private void Start()
    {
    }

    private void Update()
    {
        //animateHand();
    }

    //private void animateHand() //This should also go to the animation script :P
    //{
    //    if (CameraManagement.getActiveCamera() != "CamBonfire") { return; }
    //    Vector2 iniAng = CameraManagement.instance.activeCameraInitialRotation.eulerAngles;
       
    //    float num = Mathf.Abs(CameraManagement.instance.activeCamera.transform.eulerAngles.x - 
    //        ((CameraManagement.instance.strangeCase ? CameraManagement.instance.xLow : CameraManagement.instance.xHigh) - CameraManagement.instance.securityConstant)) - handAnimationThreshold;
    //    if (num <= handAnimationThreshold)
    //    {
    //        float value = (Mathf.Abs((num / handAnimationThreshold) - 1));
    //        anim.SetFloat("Blend", value);
    //    }
    //    else
    //    {
    //        anim.SetFloat("Blend", 0);
    //    }
    //}

    //public void changeLockState()
    //{
    //    lockHand = !lockHand;
    //}
}
