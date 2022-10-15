using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeeklyCalendar : MonoBehaviour
{
    public Day[] week = new Day[7]; //Array fijo que representa la semana
    public int day; //El d�a exacto de la semana en el que estoy ubicado (Indice del Array Week)

    public void Start() {
        for(int i = 0; i < 7; i++){
            week[i].weather = Day.Tiempo.vacio; 
            }    //Por defecto est� vacio todo.
    }

    public int getDay() //Obtiene el d�a de la semana de Domingo a Lunes de la proxima semana (0-6) 
    {
        return day = (int)DateTime.Now.DayOfWeek;    //Nota: En primer lugar necesito saber que semana estoy del mes.
    }

    public void RandomWheater(Day[] w){
        for(int i = 0; i < 7; i++){
            int r = UnityEngine.Random.Range(1,2);
            w[i].weather = (Day.Tiempo)r; //Rellena con sol o lluvia los d�as de la semana restantes
        }
    }
}

