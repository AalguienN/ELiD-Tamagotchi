using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonthlyCalendarManager : MonoBehaviour
{
    #region variables
    private int savedDay = -1;
    private int savedMonth = -1;
    private int savedYear = -1;
    private int currentDay = -1;
    private int currentMonth = -1;
    private int currentYear = -1;
    private Month actualMonth;

    ArrayList events;

    [Header("Reference objects to display info")]
    public TMP_Text monthText;
    public Transform dayContainer;
    public GameObject calendarDayPrefab;
    private List<GameObject> daysInCalendarDisplay = new List<GameObject>();
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        events = SaveManager.events; //Copy the data from the Save Manager

        //Setup calendar
        for(int i = 0; i < 42; i++) {
            GameObject gO = Instantiate(calendarDayPrefab);
            daysInCalendarDisplay.Add(gO);
            gO.transform.SetParent(dayContainer);
            gO.transform.localPosition = new Vector3(-0.3928572f+0.1309524f*(i%7),0.25f-0.125f*(i/7),0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Get the actual time (might be improved in order to avoid cheating)
        DateTime date = DateTime.Now;
        currentDay = date.Day;
        currentMonth = date.Month;
        currentYear = date.Year;

        //Compare it to the saved in the last update in case the day changes
        if(currentDay != savedDay || currentMonth != savedMonth || currentYear != savedYear) {
            savedDay = currentDay; savedMonth = currentMonth; savedYear = currentYear;

            actualMonth = MonthConstants.GetMonth(currentMonth, currentYear);
            UpdateCalendarDisplay();
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

    public void RemoveCalendarEvent(string eId) {
        SaveManager.removeEvent(eId);
        UpdateCalendarDisplay();
    }


    public void UpdateCalendarDisplay() {
        monthText.text = actualMonth.Name + " - " + currentYear;
        DateTime firstMonthDay = new DateTime(currentYear, currentMonth, 1,0,0,0,0);
        int firstDayInWeek = (int)(firstMonthDay.DayOfWeek - 1) % 7; //Lunes = 0, Martes = 1 ....
        print(firstDayInWeek);
        int weekOffset = 0;
        if(firstDayInWeek < 4) weekOffset = -7;
        for(int i = weekOffset; i < 42+weekOffset; i++) {
            GameObject gO = daysInCalendarDisplay[i-weekOffset];
            if(i<firstDayInWeek) {
                int previousMonthInt = (currentMonth-1);
                int previousYearInt = previousMonthInt==0 ? currentYear-1 : currentYear;
                if(previousMonthInt==0) previousMonthInt = 12;
                int previousMonthDay = MonthConstants.GetMonth(previousMonthInt, previousYearInt).Days - (firstDayInWeek-1) + i;

                gO.GetComponentInChildren<TMP_Text>().text = gO.name = previousMonthDay.ToString();
            }
            else if(i>actualMonth.Days+firstDayInWeek-1) {
                gO.GetComponentInChildren<TMP_Text>().text = gO.name = (i-actualMonth.Days-(firstDayInWeek-1)).ToString();
            }
            else {
                gO.GetComponentInChildren<TMP_Text>().text = gO.name = (i-firstDayInWeek+1).ToString();
            }
            gO.GetComponentInChildren<TMP_Text>().color = (currentDay == (i-firstDayInWeek+1)) ? Color.red : Color.black;
        }
    }
    #endregion
}
