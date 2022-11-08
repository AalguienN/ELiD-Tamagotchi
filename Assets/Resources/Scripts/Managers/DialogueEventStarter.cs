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

}
