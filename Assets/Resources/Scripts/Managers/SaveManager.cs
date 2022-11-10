using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SaveManager : MonoBehaviour {
    #region Variables
    [Header("Variables to be saved")]
    [HideInInspector] private static int fireState, stickNum, currentDay, startingDay;
    [HideInInspector] private static bool startedGame;
    DateTime lastConexion;
    public static int timeSinceLastConexion;

    public static List<CalendarEvent> events = new List<CalendarEvent>();
    public static List<Day> days = new List<Day>();
    #endregion

    #region DialogueVariables
    public static bool hasBurntFirstStick = false;
    public static bool miniGameActive = false;
    public static bool blueWood = false;
    public static bool isCaputxaActive = false;

    #endregion

    #region Save and Load
    void Awake()
    {
        //Here load
        fireState = ES3.Load("lastFireState", fireState);
        stickNum = ES3.Load("stickNum", 1);
        currentDay = ES3.Load("currentDay", currentDay);
        lastConexion = ES3.Load("lastConexion", DateTime.Now);
        startedGame = ES3.Load("startedGame", false);
        events = ES3.Load("eventHandler", new List<CalendarEvent>());
        days = ES3.Load("dayHandler", new List<Day>());
        startingDay = ES3.Load("startingDay", 0);
        hasBurntFirstStick = ES3.Load("hasBurntFirstStick", false);

        currentDay = startingDay - WeeklyCalendar.GetCurrentDay(System.DateTime.Now);
        ES3.Save("currentDay",currentDay);
        
        if(!startedGame)
        {
            ES3.Save("startedGame", true);
            setStartingDay();
            startedGame = true;
            print("Welcome, new Player");
        }

        //Here calculate seconds since last conexion.
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int currentEpochTime = (int)(DateTime.Now - epochStart).TotalSeconds;

        int difference = currentEpochTime - (int)(lastConexion - epochStart).TotalSeconds;
        timeSinceLastConexion = difference; //Check if works idk
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            saveAll();
    }

    private void OnApplicationQuit()
    {
        saveAll();
    }

    private void OnApplicationFocus(bool focus)
    {
        if(!focus)
            saveAll();
    }

    public static void saveAll()
    {
        ES3.Save("stickNum", stickNum);
        ES3.Save("lastFireState", (int) GameObject.FindWithTag("Bonfire2").GetComponent<BonfireState>().hp);
        ES3.Save("lastConexion", DateTime.Now);
        ES3.Save("eventHandler", events);
        ES3.Save("dayHandler", days);
        ES3.Save("startedGame", startedGame);
        ES3.Save("hasBurntFirstStick", hasBurntFirstStick);
    }
    #endregion

    #region Properties Handler
    //Properties handler
    public static int getFireState()
    {
        return ES3.Load("lastFireState", fireState);
    }

    public static int getStickNum()
    {
        return ES3.Load("stickNum", stickNum);
    }

    public static void setStickNum(int newStickNum)
    {
        ES3.Save("stickNum", newStickNum);
        stickNum = newStickNum;
    }

    public static int getCurrentDay()
    {
        return ES3.Load("currentDay", currentDay);
    }

    public static void setCurrentDay(int newCurrentDay)
    {
        ES3.Save("currentDay", newCurrentDay);
        currentDay = newCurrentDay;
    }

    public static void setStartingDay()
    {
        startingDay = WeeklyCalendar.GetCurrentDay(System.DateTime.Now);
    }

    public static int getSecondsSinceLastConexion()
    {
        return timeSinceLastConexion;
    }
    #endregion

    #region Event Handler
    //Event handler
    public static void addEvent(CalendarEvent id)
    {
        events.Add(id);
        ES3.Save("eventHandler", events);
    }
    public static void removeEvent(string id)
    {
        foreach (CalendarEvent x in events)
        {
            if (x.id == id)
            {
                events.Remove(x);
            }
        }
        ES3.Save("eventHandler", events);
    }

    public static CalendarEvent getEvent(string id)
    {
        foreach (CalendarEvent x in events)
        {
            if(x.id == id)
            {
                return x;
            }
        }
        return null;
    }
    #endregion 

    #region Day Handler
    //Event handler
    public static void addDay(Day id)
    {
        days.Add(id);
        ES3.Save("dayHandler", days);
    }
    public static void removeDay(string id)
    {
        Day d = null;
        foreach (Day x in days)
        {
            if (x.id.ToString() == id)
            {
                d = x;
            }
        }
        if(d != null)
            days.Remove(d);
        ES3.Save("dayHandler", days);
    }

    public static Day getDay(string id)
    {
        foreach (Day x in days)
        {
            if (x.id.ToString() == id)
            {
                return x;
            }
        }
        return new Day(int.Parse(id));
    }

    public static void setDay(string id, Day.Tiempo w)
    {
        foreach (Day x in days)
        {
            if (x.id.ToString() == id)
            {
                x.weather = w;
            }
        }
        ES3.Save("dayHandler", days);
    }
    #endregion 
}
