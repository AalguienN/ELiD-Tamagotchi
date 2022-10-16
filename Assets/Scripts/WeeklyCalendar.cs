using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeeklyCalendar : MonoBehaviour
{
    public Day[] week = new Day[7]; //Array fijo que representa la semana

    public void Start() {
        for(int i = 0; i < 7; i++){
            week[i].weather = Day.Tiempo.vacio; 
        }    //Por defecto esta vacio todo.
    }

    public int getDay() //Obtiene el dia de la semana de Domingo a Lunes de la proxima semana (0-6) 
    {
        return (int)DateTime.Now.DayOfWeek;
    }

    public void RandomWeather(Day[] w){
        for(int day = getDay(); day < 7; day++){    //Obtienes el día de hoy (0..6) como punto de inicio
            int r = UnityEngine.Random.Range(1,2);
            w[day].weather = (Day.Tiempo)r; //Rellena con sol o lluvia los dias de la semana restantes
        }
    }
}

