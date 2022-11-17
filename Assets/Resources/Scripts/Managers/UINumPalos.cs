using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UINumPalos : MonoBehaviour
{
    int savedSticks = -1;

    public TMP_Text stickText;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int s = (SaveManager.getStickNum() >= 999) ? SaveManager.getStickNum() - 998 : SaveManager.getStickNum();
        if(savedSticks != s) {
            stickText.text = s.ToString();
        }
    }
}
