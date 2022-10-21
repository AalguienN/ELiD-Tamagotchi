using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTicketColors : MonoBehaviour
{
    public Color[] colors;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SpriteRenderer>().color = colors[Random.Range(0,colors.Length-1)];
    }
}
