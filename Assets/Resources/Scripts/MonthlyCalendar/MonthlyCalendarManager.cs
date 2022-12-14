using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MonthlyCalendarManager : MonoBehaviour
{
    public static MonthlyCalendarManager Instance;

    #region variables
    private int savedDay = -1;
    private int savedMonth = -1; 
    private int savedYear = -1;
    private int currentDay = -1;
    private int currentMonth = -1;
    private int currentYear = -1;
    private Month actualMonth;

    public GameObject LluviaParent;
    public GameObject Rayo;

    List<CalendarEvent> events = new List<CalendarEvent>();
    List<Day> weekClimate = new List<Day>();

    [Header("Reference objects to display info")]
    public TMP_Text monthText;
    public Transform dayContainer;
    public GameObject calendarDayPrefab;
    public Sprite[] weatherIcons = new Sprite[3];
    public Sprite circleTexture;
    public Color circleColor;
    public Sprite crossTexture;
    public Color crossColor;
    private List<GameObject> daysInCalendarDisplay = new List<GameObject>();
    public GameObject calendarEventPrefab;
    private List<GameObject> eventObjects = new List<GameObject>();

    [Header("Camera behaviour")]
    public GameObject cam;
    public Vector3 cameraDefaultPosition;
    public float selectedTileOffset;
    private Vector3 cameraDesiredPosition;
    private GameObject selectedObject;
    private bool blockManualZoom = false;
    
    [Header("Other variables")]
    private Vector2 mousePosition;
    #endregion

    void Awake() {
        Instance = this;
    }

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
            gO.transform.rotation = transform.rotation;
            gO.transform.GetChild(1).transform.rotation = Quaternion.Euler(0,150, UnityEngine.Random.Range(-25f,25f));
            gO.transform.GetChild(3).transform.rotation = Quaternion.Euler(0,150, UnityEngine.Random.Range(-25f,25f));
            gO.transform.SetParent(dayContainer);
            gO.transform.localPosition = new Vector3((-0.3928572f+0.1309524f*(i%7)),0.25f-0.125f*(i/7),0);
        }

        transform.GetChild(0).transform.localScale = transform.GetChild(0).transform.localScale*2.5f/6f;

        //Get the actual time (might be improved in order to avoid cheating)
        DateTime date = DateTime.Now;
        currentDay = date.Day;
        currentMonth = date.Month;
        currentYear = date.Year;
        savedDay = currentDay; savedMonth = currentMonth; savedYear = currentYear;

        actualMonth = MonthConstants.GetMonth(currentMonth, currentYear);
        UpdateCalendarDisplay();
        switch(GetDayWeather()) {
            case 0:
                break;
            case 1:
                BonfireState.Instance.weather = BonfireState.globalState.despejado;
                LluviaParent.SetActive(false);
                Rayo.SetActive(false);
                break;
            case 2:
                BonfireState.Instance.weather = BonfireState.globalState.lluvia;
                LluviaParent.SetActive(true);
                Rayo.SetActive(false);
                break;
            case 3:
                BonfireState.Instance.weather = BonfireState.globalState.lluvia;
                LluviaParent.SetActive(true);
                Rayo.SetActive(true);
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        // //TEMP
        // if(Input.GetKeyDown(KeyCode.E))
        //     AddCalendarEvent(new CalendarEvent(UnityEngine.Random.Range(1,9999).ToString(), UnityEngine.Random.Range(1,9999).ToString(), 2022, 10, UnityEngine.Random.Range(1,31)));

        //Click on one day to see its events
        if(!blockManualZoom)
        {
            if(CameraManagement.getActiveCamera()=="CamCalendar") {
                if(Input.GetMouseButtonDown(0)) {
                    mousePosition = Input.mousePosition;
                }
                if (Input.GetMouseButtonUp(0)){
                    if(Vector3.Distance(mousePosition, Input.mousePosition) < 100f) {
                        if(selectedObject) { selectedObject = null; cameraDesiredPosition = cameraDefaultPosition; return; }
                        RaycastHit hit; 
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); 
                        if ( Physics.Raycast (ray,out hit,500.0f)) {
                            if(hit.collider.gameObject.CompareTag("CalendarTag")) {
                                cameraDesiredPosition = hit.collider.gameObject.transform.position + hit.normal*selectedTileOffset;
                                selectedObject = hit.collider.gameObject;
                            }
                            else if(hit.collider.gameObject.GetComponent<InteractableResource>()) {
                                
                            }
                        }
                    }
                }
            }
            else
            {
                cameraDesiredPosition = cameraDefaultPosition;
            }
        }


        //TO BE CHANGED
        cam.transform.position = Vector3.Lerp(cam.transform.position, cameraDesiredPosition, Time.deltaTime*2);
    }

    public void ZoomOut() {
        cameraDesiredPosition = cameraDefaultPosition;
        selectedObject = null;
    }

    public void ZoomIn()
    {
        DateTime firstMonthDay = new DateTime(currentYear, currentMonth, 1,0,0,0,0);
        int firstDayInWeek = (int)(firstMonthDay.DayOfWeek - 1) % 7; //Lunes = 0, Martes = 1 ....
        //print(firstDayInWeek);
        int weekOffset = 0;
        
        if(firstDayInWeek < 4) weekOffset = -7;
        int i = (currentDay+1)-weekOffset;

        if(i<firstDayInWeek) { // If it is a day from the previous month
        }
        else if(i>actualMonth.Days+firstDayInWeek-1) { // If it is a day from the next month
        }
        else { // If it is a day from the current month
            cameraDesiredPosition = daysInCalendarDisplay[i].transform.position + -Camera.main.transform.forward*selectedTileOffset;
            selectedObject = daysInCalendarDisplay[i];
        }
    }

    public void BlockManualZoom(bool block) {
        blockManualZoom = block;
    }

    public List<CalendarEvent> GetCalendarEvent(int year, int month, int day) {
        List<CalendarEvent> eventListReturn = new List<CalendarEvent>();
        foreach (CalendarEvent e in events) {
            if(e.year == year && e.month == month && e.day == day) eventListReturn.Add(e);
        }
        return eventListReturn;
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
        weekClimate = GetComponent<WeeklyCalendar>().InitWeek();

        monthText.text = actualMonth.Name + " - " + currentYear;
        DateTime firstMonthDay = new DateTime(currentYear, currentMonth, 1,0,0,0,0);
        int firstDayInWeek = (int)(firstMonthDay.DayOfWeek - 1) % 7; //Lunes = 0, Martes = 1 ....
        //print(firstDayInWeek);
        int weekOffset = 0;
        if(firstDayInWeek < 4) weekOffset = -7;
        for(int i = weekOffset; i < 42+weekOffset; i++) {
            GameObject gO = daysInCalendarDisplay[i-weekOffset];
            
            if(i<firstDayInWeek) { // If it is a day from the previous month
                int previousMonthInt = (currentMonth-1);
                int previousYearInt = previousMonthInt==0 ? currentYear-1 : currentYear;
                if(previousMonthInt==0) previousMonthInt = 12;
                int previousMonthDay = MonthConstants.GetMonth(previousMonthInt, previousYearInt).Days - (firstDayInWeek-1) + i;
                
                System.DateTime epochToday = new System.DateTime(previousYearInt, previousMonthInt, previousMonthDay, 0, 0, 0, System.DateTimeKind.Utc);
                int globalDay = WeeklyCalendar.GetWorldDay(epochToday);
                int dW = (int)SaveManager.getDay(globalDay.ToString()).weather;

                ChangeTileVisuals(gO, crossTexture, crossColor, previousYearInt, previousMonthInt, previousMonthDay, dW);
            }
            else if(i>actualMonth.Days+firstDayInWeek-1) { // If it is a day from the next month
                int localYear = (currentMonth==12) ? currentYear+1 : currentYear;
                int localMonth = (currentMonth==12) ? 1 : currentMonth+1;
                int localDay = (i-actualMonth.Days-(firstDayInWeek-1));

                System.DateTime epochToday = new System.DateTime(localYear, localMonth, localDay, 0, 0, 0, System.DateTimeKind.Utc);
                int globalDay = WeeklyCalendar.GetWorldDay(epochToday);
                int dW = (int)SaveManager.getDay(globalDay.ToString()).weather;
                
                ChangeTileVisuals(gO, null, Color.white, localYear, localMonth, localDay, dW);
            }
            else { // If it is a day from the current month
                System.DateTime epochToday = new System.DateTime(currentYear, currentMonth, (i-firstDayInWeek+1), 0, 0, 0, System.DateTimeKind.Utc);
                int globalDay = WeeklyCalendar.GetWorldDay(epochToday);
                int dW = (int)SaveManager.getDay(globalDay.ToString()).weather;

                ChangeTileVisuals(gO, (currentDay == (i-firstDayInWeek+1)) ? circleTexture : ((currentDay > (i-firstDayInWeek+1)) ? crossTexture : null),
                    (currentDay == (i-firstDayInWeek+1)) ? circleColor : ((currentDay > (i-firstDayInWeek+1)) ? crossColor : Color.white), currentYear, currentMonth, (i-firstDayInWeek+1), dW);
            }
        }
    }

    void ChangeTileVisuals(GameObject gO, Sprite dayTexture, Color dayColor, int year, int month, int day, int climateIntIcon) {
        List<CalendarEvent> cE = GetCalendarEvent(year,month,day);
        if(cE != null) {
            foreach(CalendarEvent e in cE) {
                GameObject eventObject = Instantiate(calendarEventPrefab);
                eventObject.GetComponentInChildren<TMP_Text>().text = e.eventName; 
                eventObjects.Add(eventObject);
                eventObject.transform.SetParent(gO.transform.GetChild(0).transform);
                eventObject.transform.localRotation = Quaternion.Euler(0,0,UnityEngine.Random.Range(-25f,25f));
                eventObject.transform.localPosition = new Vector3(UnityEngine.Random.Range(-0.15f,0.15f),UnityEngine.Random.Range(-0.2f,0.1f),0);
                eventObject.transform.localScale = new Vector3(0.8f,0.8f,0.8f);
                eventObject.name = e.eventName;
            }
        }
        gO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().sprite = dayTexture;
        gO.transform.GetChild(3).gameObject.GetComponent<SpriteRenderer>().color = dayColor;
        gO.transform.GetChild(1).gameObject.GetComponent<TMP_Text>().text = gO.name = day.ToString();
        gO.transform.GetChild(2).gameObject.GetComponent<SpriteRenderer>().sprite = weatherIcons[climateIntIcon]; 
    }

    public static int GetDayWeather() {
        return (int)SaveManager.getDay(WeeklyCalendar.GetWorldDay(DateTime.Now).ToString()).weather;
    }
    #endregion
}
