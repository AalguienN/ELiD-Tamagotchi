using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Time;

public class MonthlyCalendarManager : MonoBehaviour
{
    public int day;
    public int month;

    // Start is called before the first frame update
    void Start()
    {
        enum[7]
    }

    // Update is called once per frame
    void Update()
    {
        DateTime date = DateTime.Now;
        day = date.Day;
        month = date.Day;
    }
}
