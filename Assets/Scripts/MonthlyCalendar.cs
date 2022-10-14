using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonthlyCalendar
{
    public ArrayList events;

    public MonthlyCalendar() {
        events = SaveManager.events;
    }

    public CalendarEvent getCalendarEvent(string id) {
        foreach (CalendarEvent e in events) {
            if(e.id == id) return e;
        }
        return null;
    }
}
