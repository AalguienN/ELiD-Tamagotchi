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


    public void startCurrentDayDialogue()
    {
        startConversation(SaveManager.getCurrentDay());
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

    IEnumerator sex() {
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
        StartCoroutine(sex());
    }

    public void negationAnimation() {

    }

    public void activateCameraMovementRight() {

    }

    public void activateBlueStick() {

    }

    public void apareceCara() { 
    
    }


}
