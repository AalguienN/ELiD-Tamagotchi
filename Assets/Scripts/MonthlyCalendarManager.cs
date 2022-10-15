using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonthlyCalendarManager : MonoBehaviour
{
    int savedDay;
    int savedMonth;
    int currentDay;
    int currentMonth;

    ArrayList events;

    public GameObject[] daysInCalendarDisplay = new GameObject[35];

    // Start is called before the first frame update
    void Start()
    {
        events = SaveManager.events;
        StartupCalendarDisplay();
    }

    // Update is called once per frame
    void Update()
    {
        DateTime date = DateTime.Now;
        currentDay = date.Day;
        currentMonth = date.Month;
        if(currentDay != savedDay || currentMonth != savedMonth) {
            savedDay = currentDay;
            savedMonth = currentMonth;
        }
    }

    public CalendarEvent getCalendarEvent(string id) {
        foreach (CalendarEvent e in events) {
            if(e.id == id) return e;
        }
        return null;
    }

    // public void addCalendarEvent(CalendarEvent event) {
    //     SaveManager.addEvent(event);
    //     UpdateCalendarDisplay();
    // }

    // public void removeCalendarEvent(CalendarEvent event) {
    //     SaveManager.removeEvent(event);
    //     UpdateCalendarDisplay();
    // }

    public void StartupCalendarDisplay() {
        foreach(GameObject dayGameObject in daysInCalendarDisplay) {
            
        }
    }

    public void UpdateCalendarDisplay() {

    }
}
