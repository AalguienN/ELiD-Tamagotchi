using System.IO;
using System.Collections.Generic;
using System;
using UnityEngine;

public class SaveManager : MonoBehaviour {
    #region Variables
    [Header("Variables to be saved")]
    [HideInInspector] private static int fireState, stickNum, blueStickNum, currentDay, mgHits,mgSticks;
    [HideInInspector] private static DateTime startingDay, minigameEndingDate;
    [HideInInspector] private static bool startedGame;
    private bool aux_startedGame = false;
    DateTime lastConexion;
    public static int lastConexionDay;
    public static int timeSinceLastConexion;

    public static List<CalendarEvent> events = new List<CalendarEvent>();
    public static List<Day> days = new List<Day>();
    public static List<Fuel> fuelList = new List<Fuel>();
    #endregion

    #region DialogueVariables
    public static bool hasBurntFirstStick = false;
    public static bool hasBurntFirstBlueStick = false;
    public static bool hasBurntLastBlueStick = false;
    public static bool hasTurnedRight = false;
    public static bool hasTurnedLeft = false;
    public static bool blueWood = false;
    public static bool isCaputxaActive = false;
    public static int canOnlyTurn = 0;
    public static bool canCaputxaBeInteracted = false;
    public static bool hasBeenCaputxaInteracted = false;
    public static bool hasBeenDialoguePlayed = false;
    public static bool hasPinochioRun = false;


    public static bool finaleAnimationHasEnded = false;

    #endregion

    #region Save and Load
    void Awake()
    {
        DoOnAwake();
    }

    void DoOnAwake()
    {
        //Here load
        fireState = ES3.Load("lastFireState", 0);
        stickNum = ES3.Load("stickNum", 0) < 0 ? 0 : ES3.Load("stickNum", 0);
        blueStickNum = ES3.Load("blueStickNum", 0) < 0 ? 0 : ES3.Load("blueStickNum", 0);
        lastConexion = ES3.Load("lastConexion", DateTime.Now);
        lastConexionDay = ES3.Load("lastConexionDay", 0);
        startedGame = ES3.Load("startedGame", false);
        events = ES3.Load("eventHandler", new List<CalendarEvent>());
        days = ES3.Load("dayHandler", new List<Day>());
        fuelList = ES3.Load("fuelList", new List<Fuel>());
        startingDay = ES3.Load("startingDay", System.DateTime.Now);
        minigameEndingDate = ES3.Load("minigameEndingDate", new DateTime());
        hasBurntFirstStick = ES3.Load("hasBurntFirstStick", false);
        hasBurntFirstBlueStick = ES3.Load("hasBurntFirstBlueStick", false);
        hasBurntLastBlueStick = ES3.Load("hasBurntLastBlueStick", false);
        hasTurnedLeft = ES3.Load("hasTurnedLeft", false);
        hasTurnedRight = ES3.Load("hasTurnedRight", false);
        canOnlyTurn = ES3.Load("canOnlyTurn", 0);
        blueWood = ES3.Load("blueWood", false);
        mgHits = ES3.Load("mgHits", 0);
        mgSticks = ES3.Load("mgSticks", 0);
        hasPinochioRun = ES3.Load("hasPinochioRun", false);
        finaleAnimationHasEnded = ES3.Load("finaleAnimationHasEnded", false);


        if (!startedGame)
        {
            ES3.Save("startedGame", true);
            aux_startedGame = true;
            cameraAnimationHandler.instance.ChangeAnimation(cameraAnimationHandler.START_ANIMATION); //Make this wait until load finished
            setStartingDay();
            startedGame = true;
        }

        

        //Here calculate seconds since last conexion.
        DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        int currentEpochTime = (int)(DateTime.Now - epochStart).TotalSeconds;

        int difference = currentEpochTime - (int)(lastConexion - epochStart).TotalSeconds;
        timeSinceLastConexion = difference; //Check if works idk

        currentDay = WeeklyCalendar.GetDaysSinceStart(System.DateTime.Now, startingDay) + 1;
        if(difference < 0)
        {
            print("Viaje en el tiempo");
            currentDay = 0;
        }
        ES3.Save("currentDay", currentDay);
    }

    private void Start()
    {
        DoOnStart();
    }

    private void DoOnStart()
    {
        ////////////////////////////////////////////////////
        //Make sure caputxa only appears in different day
        if (lastConexionDay < getCurrentDay() || aux_startedGame)
        {
            if (getCurrentDay() == 4)
            {
                DialogueEventStarter.instance.disableCaputxa();
                DialogueEventStarter.instance.enablePinochio();
            }
            else
            {
                DialogueEventStarter.instance.enableCaputxa();
            }
            hasBeenDialoguePlayed = false;
            canCaputxaBeInteracted = true;
            aux_startedGame = false;
        }
        else
        {
            canCaputxaBeInteracted = false;
            hasBeenDialoguePlayed = true;
            DialogueEventStarter.instance.disableCaputxa();
        }
        /////////////////////////////////////////////////
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
            saveAll();
    }

    private void OnApplicationQuit()
    {
        onExit();
    }




    private void OnApplicationFocus(bool focus)
    {
        if (!focus)
            onExit();
        else
        {
            DoOnAwake();
            DoOnStart();
        }
    }

    public void onExit()
    {
        if (!hasBeenDialoguePlayed && hasBeenCaputxaInteracted)
        {
            switch (getCurrentDay())
            {
                case 1:
                    clearData();
                    break;

                case 4:
                    //ES3.Save("lastConexionDay", lastConexionDay - 1);
                    //ES3.Save("hasPinochioRun", false);
                    break;

                default:
                    //print("Repeating day dialogue");
                    //ES3.Save("lastConexionDay", lastConexionDay - 1);
                    break;
            }
        }
        else
        {
            saveAll();
        }
    }

    public static void saveAll()
    {
        print("Saving All");
        ES3.Save("stickNum", stickNum);
        ES3.Save("blueStickNum", blueStickNum);
        ES3.Save("lastFireState", (int) GameObject.FindWithTag("Bonfire2").GetComponent<BonfireState>().hp);
        ES3.Save("lastConexion", DateTime.Now);
        ES3.Save("lastConexionDay", getCurrentDay() < lastConexionDay ? lastConexionDay : getCurrentDay());
        ES3.Save("eventHandler", events);
        ES3.Save("dayHandler", days);
        ES3.Save("fuelList", fuelList);
        ES3.Save("startedGame", startedGame);
        ES3.Save("hasBurntFirstStick", hasBurntFirstStick);
        ES3.Save("hasBurntFirstBlueStick", hasBurntFirstBlueStick);
        ES3.Save("hasBurntLastBlueStick", hasBurntLastBlueStick);
        ES3.Save("hasTurnedLeft", hasTurnedLeft);
        ES3.Save("hasTurnedRight", hasTurnedRight);
        ES3.Save("canOnlyTurn", canOnlyTurn);
        ES3.Save("hasPinochioRun", hasPinochioRun);
        ES3.Save("finaleAnimationHasEnded", finaleAnimationHasEnded);
    }
    #endregion

    #region Properties Handler
    //Properties handler
    public static int getFireState()
    {
        return ES3.Load("lastFireState", fireState);
    }

    public static DateTime getMinigameEndingDate()
    {
        return minigameEndingDate;
    }

    public static void setMinigameEndingDate(DateTime endingDate)
    {
        minigameEndingDate = endingDate;
        ES3.Save("minigameEndingDate", minigameEndingDate);
    }

    public static int getMinigameHits()
    {
        return ES3.Load("minigameSavedHits", mgHits);
    }

    public static void setMinigameHits(int hits)
    {
        ES3.Save("minigameSavedHits", hits);
        mgHits = hits;
    }
    public static int getMinigameSticks()
    {
        return ES3.Load("minigameSavedSticks", mgSticks);
    }

    public static void setMinigameSticks(int sticks)
    {
        ES3.Save("minigameSavedSticks", sticks);
        mgSticks = sticks;
    }

    public static int getStickNum()
    {
        return ES3.Load("stickNum", stickNum);
    }

    public static int getBlueStickNum()
    {
        return ES3.Load("blueStickNum", blueStickNum);
    }

    public static void setStickNum(int newStickNum)
    {
        ES3.Save("stickNum", newStickNum);
        stickNum = newStickNum;
    }
    public static void setBlueStickNum(int newStickNum)
    {
        ES3.Save("blueStickNum", newStickNum);
        blueStickNum = newStickNum;
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
        startingDay = System.DateTime.Now;
        ES3.Save("startingDay", startingDay);
    }

    public static int getSecondsSinceLastConexion()
    {
        return timeSinceLastConexion;
    }

    public static int getCanOnlyTurn()
    {
        return canOnlyTurn;
    }

    public static void setCanOnlyTurn(int num)
    {
        //ES3.Save("canOnlyTurn", num);
        canOnlyTurn = num; //from -1 to 2
    }

    public static void setBlueWood(bool bW)
    {
        blueWood = bW;
        ES3.Save("blueWood", bW);
    }

    public static bool getBlueWood()
    {
        bool bW = false; 
        try {
            ES3.Load("blueWood", bW);
        }
        catch(Exception e) { }
        return bW;
    }

    public static void clearData()
    {
        DirectoryInfo dataDir = new DirectoryInfo(Application.persistentDataPath);
        FileInfo[] files = dataDir.GetFiles();
        foreach (FileInfo file in files) {
            try{
                file.Delete();
            } catch(Exception e){}
        }
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
