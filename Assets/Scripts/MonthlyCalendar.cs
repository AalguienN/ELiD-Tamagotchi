using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonthlyCalendar : MonoBehaviour
{
    public ArrayList events;

    public void Start() {
        events = SaveManager.events;
    }

    public CalendarEvent getCalendarEvent(string id) {
        return SaveManager.getEvent(id);
    }

    
}
