using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PixelCrushers.DialogueSystem;

public class DialogueEventStarter : MonoBehaviour
{
    public static DialogueEventStarter instance;
    private void Awake()
    {
        instance = this;
    }

    public string[] conversations = new string[7];
    public Transform encapuchado, player;

    public void startConversation(int day)
    {
        DialogueManager.StartConversation(conversations[day - 1]);
    }

    IEnumerator sex() {
        yield return new WaitForSeconds(5);
        print("lol");
        (DialogueManager.dialogueUI as StandardDialogueUI).OnContinue();
    }

    public void giveFirstStick()
    {
        
    }

    public void firstBurn()
    {
        print("worked1");
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
