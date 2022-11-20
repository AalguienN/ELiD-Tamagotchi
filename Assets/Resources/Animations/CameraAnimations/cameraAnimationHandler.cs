using UnityEngine;

public class cameraAnimationHandler : MonoBehaviour
{
    public static cameraAnimationHandler instance;

    Animator anim;

    private string currentState;

    public const string START_ANIMATION = "StartingAnimation";
    public const string IDLE_ANIMATION = "IdleAnim";
    public const string RIGHT_ARROW = "RightArrowAnim";
    public const string LEFT_ARROW = "LeftArrowAnim";
    public const string FINALE_ANIM = "EndingAnimation";
    public const string FINALE_IDLE = "EndingIdle";

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

    public void startFirstConversation()
    {
        DialogueEventStarter.instance.startConversation(1);

    }

    public void caputxaEnabler(bool isEnabled)
    {
        if (isEnabled)
        {
            DialogueEventStarter.instance.enableCaputxa();
        }
        else
        {
            DialogueEventStarter.instance.disableCaputxa();
        }
    }

    public void caputxaInteractionEnabler(bool isEnabled = true)
    {
        if (isEnabled)
        {
            DialogueEventStarter.instance.enableCaputxaInteraction();
        }
        else
        {
            DialogueEventStarter.instance.disableCaputxaInteraction();
        }
    }

    public void BlackScreenPlay()
    {
        ChangeAnimation(FINALE_IDLE);
        SaveManager.finaleAnimationHasEnded = true;
        //Load new scene asynchrounously
    }

    public void ChangeAnimation(string newState)
    {
        if (currentState == newState) return;

        anim.Play(newState);

        currentState = newState;
    }
}
