using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SaveManager : MonoBehaviour {
    #region Variables
    [Header("Variables to be saved")]
    [HideInInspector] private static int fireState, stickNum, currentDay;
    DateTime lastConexion;
    public static int timeSinceLastConexion;

    public static ArrayList events = new ArrayList();
    #endregion

    #region Save and Load
    void Awake()
    {
        //Here load
        fireState = ES3.Load("lastFireState", fireState);
        stickNum = ES3.Load("stickNum", stickNum);
        currentDay = ES3.Load("currentDay", currentDay);
        lastConexion = ES3.Load("lastConexion", DateTime.Now);
        events = ES3.Load("eventHandler", events);


        //Here calculate seconds since last conexion.
        TimeSpan timeSpan = lastConexion - DateTime.Now;
        timeSinceLastConexion = Convert.ToInt32(timeSpan.TotalSeconds); //Check if works idk
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

    public void saveAll()
    {
        ES3.Save("stickNum", stickNum);
        ES3.Save("currentDay", currentDay);
        ES3.Save("lastFireState", fireState);
        ES3.Save("lastConexion", DateTime.Now);
        ES3.Save("eventHandler", events);
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
}
