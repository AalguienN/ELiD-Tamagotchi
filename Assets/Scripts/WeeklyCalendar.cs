using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//EJEMPLO
/*Estamos a 16 de octubre, entramos al juego por primera vez (hasta ahi todo correcto)
    Al entrar por primera vez, generamos los 7 dias siguientes a el actual

Pasan 3 dias, estamos a 19 de octubre
    El dia 16, 17 y 18 se tienen que eliminar del calendario pq como ya han pasado, no nos interesa guardarlos
    Ademas de eso, hay que generar 3 dias más en el futuro, el dia 24,25 y 26 (creo)

Luego la opcion extra que dices de machacar los dias futuros es de que haya una probabilidad de x% de que un dia que ya tenga un valor establecido cambie
    Pero esto ya se hará, es un if con un (random > 0.2) p.ej.

La parte de eliminar los dias, lo mejor sería obtener el dia desde el 1 de Enero de 1970 y compararlo por si acaso el jugador juega desde el 25 de diciembre al 4 de enero por ejemplo
*/

//Dejo el ejemplo para que no se pierda la idea de lo que hay que hacer >:^)

public class WeeklyCalendar : MonoBehaviour
{
    public Day[] week = new Day[7]; //Array fijo que representa la semana
    private const float climateChangeProbability = 0.2f; //Porcentaje de probabilidad de que el clima cambie de un dia para otro 
    private float random;

    public void Start() {
        for(int i = 0; i < 7; i++){
            week[i].weather = Day.Tiempo.vacio; 
        }    //Por defecto esta vacio todo.
    }

    //Funciones para randomizar

    public int getDay() //Obtiene el dia de la semana de Domingo a Lunes de la proxima semana (0-6) 
    {
        return (int)DateTime.Now.DayOfWeek;

    }

    public float random_climate()
    {
        return (float)UnityEngine.Random.Range(0.1f, 1.5f);  //Si es estrictamente mayor al climateChangeProbability se machaca el clima de ese día
    }


    //Función para establecer el clima de la semana.
    public void RandomWeather(Day[] w){
        for(int day = 0; day < 7; day++){    //Obtienes el día de hoy (0..6) como punto de inicio
            if(w[day].weather == Day.Tiempo.vacio) {
                int r = UnityEngine.Random.Range(1,2);
                w[day].weather = (Day.Tiempo)r; //Rellena con sol o lluvia los dias de la semana restantes
            }
        }
    }
}