using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraAnimationHandler : MonoBehaviour
{
    public static cameraAnimationHandler instance;

    Animator anim;

    private string currentState;

    public const string START_ANIMATION = "StartingAnimation";
    public const string IDLE_ANIMATION = "IdleAnim";

    private void Awake()
    {
        instance = this;
        anim = GetComponent<Animator>();
    }

    public void Start()
    {
        currentState = IDLE_ANIMATION;
    }

    public void ReturnToIDLE()
    {
        ChangeAnimation(IDLE_ANIMATION);
    }

    public void ChangeAnimation(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);

        currentState = newState;
    }
}
