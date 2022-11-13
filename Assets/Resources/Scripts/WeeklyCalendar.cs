using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//EJEMPLO
/*Estamos a 16 de octubre, entramos al juego por primera vez (hasta ahi todo correcto)
    Al entrar por primera vez, generamos los 7 dias siguientes a el actual

Pasan 3 dias, estamos a 19 de octubre
    El dia 16, 17 y 18 se tienen que eliminar del calendario pq como ya han pasado, no nos interesa guardarlos
    Ademas de eso, hay que generar 3 dias m�s en el futuro, el dia 24,25 y 26 (creo)

Luego la opcion extra que dices de machacar los dias futuros es de que haya una probabilidad de x% de que un dia que ya tenga un valor establecido cambie
    Pero esto ya se har�, es un if con un (random > 0.2) p.ej.

La parte de eliminar los dias, lo mejor ser�a obtener el dia desde el 1 de Enero de 1970 y compararlo por si acaso el jugador juega desde el 25 de diciembre al 4 de enero por ejemplo
*/

//Dejo el ejemplo para que no se pierda la idea de lo que hay que hacer >:^)

public class WeeklyCalendar : MonoBehaviour
{
    public List<Day> week = new List<Day>(); //Array fijo que representa la semana
    private const float climateChangeProbability = 0.2f; //Porcentaje de probabilidad de que el clima cambie de un dia para otro 

    public void Start()
    {
        InitWeek(); //Se inicializa la semana;
    }

    public List<Day> InitWeek()
    {
        try{
            week = SaveManager.days;
        } catch(Exception e) {
            week.Clear();
        }
        
        if(week.Count == 0) {
            for(int i = 0; i < 7; i++) {
                Day d = new Day(GetWorldDay(System.DateTime.Now)+i);

                if(i == 2)
                    d.weather = (Day.Tiempo)2;
                else if(i == 4)
                    d.weather = (Day.Tiempo)3;
                else
                    d.weather = (Day.Tiempo)1;

                week.Add(d);             
                SaveManager.addDay(d);
            }
        }

        for(int i = 0; i < 7; i++) {
            if(SaveManager.getDay((GetWorldDay(System.DateTime.Now)+i).ToString()).weather == Day.Tiempo.vacio)
            {
                int r = UnityEngine.Random.Range(1, 4);
                Day d = new Day(GetWorldDay(System.DateTime.Now)+i);
                d.weather = (Day.Tiempo)r; //Rellena con sol o lluvia los dias de la semana restantes
                week.Add(d);             
                SaveManager.addDay(d);
            }
        }
        List<Day> weekTemp = new List<Day>();
        foreach(Day d in week) {
            if(d.id < GetWorldDay(System.DateTime.Now))
            {
                weekTemp.Add(d);
            }
        }

            /*foreach(Day d in weekTemp) {
                week.Remove(d);
                SaveManager.removeDay(d.id.ToString());
        }*/
        return week;
    }

    public float RandomClimate()
    {
        return (float)UnityEngine.Random.Range(0.5f, 1.5f);  //Si es estrictamente mayor al climateChangeProbability se machaca el clima de ese d�a
    }

    public static int GetWorldDay(System.DateTime dateTime) {
        System.DateTime epochStart = new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
        return (int)(dateTime - epochStart).Days;
    }

    public static int GetDaysSinceStart(System.DateTime dateTime, System.DateTime startingDay) {
        return (int)(dateTime - startingDay).TotalSeconds/(3600*24);
    }
}


