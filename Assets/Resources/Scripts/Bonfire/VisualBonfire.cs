using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualBonfire : MonoBehaviour
{
    public float targetLight;

    // Update is called once per frame
    void Update()
    {
        //Change shader variable to targetLight
    }

    public static void SetTargetLight(float target) {
        targetLight = target;
    }
}
