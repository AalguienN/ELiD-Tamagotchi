using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dialogueTest : MonoBehaviour
{
    [Range(1,7)]public int dialogueToPlay = 1;
    private void Start()
    {
        DialogueEventStarter.instance.startConversation(dialogueToPlay);
    }
}
