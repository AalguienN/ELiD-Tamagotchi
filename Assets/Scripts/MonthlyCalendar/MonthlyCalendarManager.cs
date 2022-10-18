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
    public Sprite[] weatherIcons = new Sprite[3];
    private List<GameObject> daysInCalendarDisplay = new List<GameObject>();
    public GameObject calendarEventPrefab;
    private List<GameObject> eventObjects = new List<GameObject>();

    [Header("Camera behaviour")]
    public Vector3 cameraDefaultPosition;
    public Vector3 selectedTileOffset;
    private Vector3 cameraDesiredPosition;
    private GameObject selectedObject;
    #endregion

    #region Methods
    // Start is called before the first frame update
    void Start()
    {
        events = SaveManager.events; //Copy the data from the Save Manager
        cameraDesiredPosition = cameraDefaultPosition;

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

        //TEMP
        if(Input.GetKeyDown(KeyCode.E))
            AddCalendarEvent(new CalendarEvent(+"-10-2202",Random.Range(1,9999),));

        //Click on one day to see its events
        if ( Input.GetMouseButtonDown (0)){ 
            if(selectedObject) { selectedObject = null; cameraDesiredPosition = cameraDefaultPosition; return; }
            RaycastHit hit; 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
            if ( Physics.Raycast (ray,out hit,100.0f)) {
                if(hit.collider.gameObject.CompareTag("CalendarTag")) {
                    cameraDesiredPosition = hit.collider.gameObject.transform.position + selectedTileOffset;
                    selectedObject = hit.collider.gameObject;
                }
                else if(hit.collider.gameObject.GetComponent<InteractableResource>()) {
                    
                }
            }
        }

        //TO BE CHANGED
        Camera.main.transform.position = Vector3.Lerp(Camera.main.transform.position, cameraDesiredPosition, Time.deltaTime*2);
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

    //Behaviour for setting the UI correctly in order to display the days and other info
    public void UpdateCalendarDisplay() {
        //Remove all event objects (non optimal but I assume that every phone will handle it)
        foreach(GameObject e in eventObjects) {
            Destroy(e);
        }
        eventObjects.Clear();

        monthText.text = actualMonth.Name + " - " + currentYear;
        DateTime firstMonthDay = new DateTime(currentYear, currentMonth, 1,0,0,0,0);
        int firstDayInWeek = (int)(firstMonthDay.DayOfWeek - 1) % 7; //Lunes = 0, Martes = 1 ....
        //print(firstDayInWeek);
        int weekOffset = 0;
        if(firstDayInWeek < 4) weekOffset = -7;
        for(int i = weekOffset; i < 42+weekOffset; i++) {
            GameObject gO = daysInCalendarDisplay[i-weekOffset];
            if(i<firstDayInWeek) {
                int previousMonthInt = (currentMonth-1);
                int previousYearInt = previousMonthInt==0 ? currentYear-1 : currentYear;
                if(previousMonthInt==0) previousMonthInt = 12;
                int previousMonthDay = MonthConstants.GetMonth(previousMonthInt, previousYearInt).Days - (firstDayInWeek-1) + i;

                ChangeTileVisuals(gO, Color.grey, previousMonthDay.ToString(), Color.white, previousMonthInt, previousYearInt);
            }
            else if(i>actualMonth.Days+firstDayInWeek-1) {
                ChangeTileVisuals(gO, Color.grey, (i-actualMonth.Days-(firstDayInWeek-1)).ToString(), Color.white, (currentMonth==12) ? 1 : currentMonth+1, (currentMonth==12) ? currentYear+1 : currentYear);
            }
            else {
                ChangeTileVisuals(gO, Color.white, (i-firstDayInWeek+1).ToString(), (currentDay == (i-firstDayInWeek+1)) ? Color.red : Color.black, currentMonth, currentYear);
                //WeeklyCalendar.GetWeather(currentDay);
            }
        }
    }

    void ChangeTileVisuals(GameObject gO, Color tileColor, string textString, Color textColor, int month, int year) {
        CalendarEvent cE = GetCalendarEvent(textString+"-"+month+"-"+year);
        if(cE != null) {
            GameObject eventObject = Instantiate(calendarEventPrefab);
            eventObjects.Add(eventObject);
            eventObject.transform.SetParent(gO);
            eventObject.transform.localPosition = new Vector3(-0.3928572f+0.1309524f,0.25f-0.125f,0);
            eventObjects.name = cE.eventName;
        }
        gO.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = tileColor;
        gO.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponent<TMP_Text>().text = gO.name = textString;
        gO.transform.GetChild(0).transform.GetChild(0).gameObject.GetComponentInChildren<TMP_Text>().color = textColor;
    }
    #endregion
}
