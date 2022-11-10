using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualBonfire : MonoBehaviour
{
    public float targetLight;
    static float _sTargetLight;

    // Update is called once per frame
    void Update()
    {
        if(targetLight != _sTargetLight)
        {
            targetLight = _sTargetLight;
        }
    }

    public static void SetTargetLight(float target) {
        _sTargetLight = target;
    }
}
