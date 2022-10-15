using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime;

public class WeeklyCalendar : MonoBehaviour
{
    enum Tiempo { vacio = -1, sol = 0, lluvia = 1 } //Los estados posibles en los que puede estar el clima.

    public Day[7] week; //Array fijo que representa la semana
    public int day; //El día exacto de la semana en el que estoy ubicado (Indice del Array Week)

    public WeeklyCalendar() {
        for(i in week){week[i] = Tiempo.vacio; }    //Por defecto está vacio todo.
    }

    public getDay() //Obtiene el día de la semana de Domingo a Lunes de la proxima semana (0-6) 
    {
        day = DayOfWeek;    //Nota: En primer lugar necesito saber que semana estoy del mes.
    }

    public RandomWheater(Day[] w){
        for(day in w){
            Random r = new Random();    //He importado System.Runtime para el Random
            w.Add(Tiempo.r.Next(0, 1)); //Rellena con sol o lluvia los días de la semana restantes
        }
    }
}

