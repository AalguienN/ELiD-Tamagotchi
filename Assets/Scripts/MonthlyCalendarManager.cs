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
        //Get the actual time (might be improved in order to avoid cheating)
        DateTime date = DateTime.Now;
        currentDay = date.Day;
        currentMonth = date.Month;
        if(currentDay != savedDay || currentMonth != savedMonth) {
            savedDay = currentDay;
            savedMonth = currentMonth;
        }
    }

    public CalendarEvent GetCalendarEvent(string id) {
        foreach (CalendarEvent e in events) {
            if(e.id == id) return e;
        }
        return null;
    }

    public void AddCalendarEvent(CalendarEvent e) {
        SaveManager.addEvent(e);
        UpdateCalendarDisplay();
    }

    public void RemoveCalendarEvent(CalendarEvent e) {
        SaveManager.removeEvent(e);
        UpdateCalendarDisplay();
    }

    public void StartupCalendarDisplay() {
        foreach(GameObject dayGameObject in daysInCalendarDisplay) {

        }
    }

    public void UpdateCalendarDisplay() {

    }
}
