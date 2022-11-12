using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UINumPalos : MonoBehaviour
{
    int savedSticks = -1;

    public TMP_Text stickText;
    public GameObject child;
    public float t = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        int s = (SaveManager.getStickNum() >= 999) ? SaveManager.getStickNum() - 998 : SaveManager.getStickNum();
        
        if(s == 0)
        {
            t = Mathf.Lerp(t, 0, Time.deltaTime);
        }   
        else
        {
            t = Mathf.Lerp(t, 1, Time.deltaTime);
        }

        child.GetComponent<Image>().color = new Color(1,1,1,t);
        child.GetComponentInChildren<TMP_Text>().color = new Color(1,1,1,t);

        if(savedSticks != s) {
            stickText.text = s.ToString();
        }
    }
}
