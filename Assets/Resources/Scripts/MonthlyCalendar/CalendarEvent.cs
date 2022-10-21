using System;
using System.Collections;
using System.Collections.Generic;

public class CalendarEvent {
    public CalendarEvent(string ID, string eventN, int y, int m, int d) {
        id = ID;
        eventName = eventN;
        year = y;
        month = m;
        day = d;
    }
    public string id;
    public string eventName;
    public int year;
    public int month;
    public int day;
}