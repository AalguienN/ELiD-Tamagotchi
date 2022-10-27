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


}
