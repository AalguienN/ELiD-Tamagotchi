using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;
using UnityEngine.UI;

public class DialogueEventStarter : MonoBehaviour
{
    public static DialogueEventStarter instance;
    private void Awake()
    {
        instance = this;
    }

    public string[] conversations = new string[7];
    public Transform encapuchado, player;
    public Button continueBtn;
    public GameObject caputxa;


    private void Update()
    {
        if (Input.touchCount == 0) { return; }
        Touch fingertouch = Input.GetTouch(0);
        if (fingertouch.phase == TouchPhase.Began)
        {
            print("worked1");
            Vector3 mousePos = fingertouch.position;
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(mousePos);
            if (Physics.Raycast(ray, out hit, 100.0f))
            {
                if (hit.collider.gameObject.CompareTag("Caputxa") && SaveManager.canCaputxaBeInteracted)
                {
                    startCurrentDayDialogue();
                }
            }
        }
    }


    public void startCurrentDayDialogue()
    {
        print(SaveManager.getCurrentDay());
        startConversation(SaveManager.getCurrentDay());
    }

    public void endOfDialogue()
    {
        SaveManager.hasBeenDialoguePlayed = true;
    }
    
    public void disableCaputxaInteraction()
    {
        SaveManager.canCaputxaBeInteracted = false;
    }

    public void enableCaputxaInteraction()
    {
        SaveManager.canCaputxaBeInteracted = true;
    }

    public void enableCaputxa()
    {
        caputxa.SetActive(true);
    }

    public void disableCaputxa()
    {
        caputxa.SetActive(false);
    }

    public bool isCaputxaEnabled()
    {
        return caputxa.activeSelf;
    }

    public void lockCamera(float seconds)
    {
        StartCoroutine(AnimationManager.cameraLockAnimation(seconds));
    }


    public void lockCameraToCenter()
    {
        SaveManager.setCanOnlyTurn(0); //Just center
    }
    public void lockCameraToLeft()
    {
        print("lockCameraToLeft");
        SaveManager.setCanOnlyTurn(-1); //Just left
        StartCoroutine(lockCameraToLeft_());
    }
    IEnumerator lockCameraToLeft_()
    {
        yield return new WaitForSeconds(5);
        print("Ienumerator");
        if (!SaveManager.hasTurnedLeft)
        {
            cameraAnimationHandler.instance.ChangeAnimation(cameraAnimationHandler.LEFT_ARROW);
            lockCameraToLeft();
        }
    }

    public void lockCameraToRight()
    {
        print("lockCameraToRight");
        SaveManager.setCanOnlyTurn(1); //Just right
        StartCoroutine(lockCameraToRight_());
    }
    IEnumerator lockCameraToRight_()
    {
        yield return new WaitForSeconds(5);
        if (!SaveManager.hasTurnedRight)
        {
            cameraAnimationHandler.instance.ChangeAnimation(cameraAnimationHandler.RIGHT_ARROW);
            lockCameraToRight();
        }
    }

    public void unlockCamera()
    {
        SaveManager.setCanOnlyTurn(2); //Unlocks
    }

    public void enableBlueWood()
    {
        
    }


    public void disableContinue() {
        continueBtn.interactable = false;
    }

    public void enableContinue()
    {
        continueBtn.interactable = true;
    }

    public void startConversation(int day)
    {
        DialogueManager.StartConversation(conversations[day - 1]);
    }

    IEnumerator waitForFirstBurn() {
        while (!SaveManager.hasBurntFirstStick)
        {
            yield return null;
        }
        (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue();
    }

    public void giveFirstStick()
    {
        
    }
    public void firstBurn()
    {
        print("firstBurn");
        SaveManager.setStickNum(SaveManager.getStickNum() + 1);
        StartCoroutine(waitForFirstBurn());
    }

}
