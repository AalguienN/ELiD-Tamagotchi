using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fuel
{
    /** Clase del tipo de combustible */ 


    //Tipos de combustible
    public enum types { 
        stick, 
        log,
        coal,
        gasoline
    }
    public types type;
    //Añade tiempo de vida a la hoguera
    private double heal;

    //Debuf, si el combustible es rápido, lento...
    private double debuff;

    public Fuel() {
        type = types.stick;

    }


}
