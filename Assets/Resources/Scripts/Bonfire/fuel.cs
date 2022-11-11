using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Fuel
{
    /** Clase del tipo de combustible */
    public const double STICK_HEAL = 10;
    public const bool STICK_WATERSIELD = false;
    public const double BLUESTICK_HEAL = 20;
    public const bool BLUESTICK_WATERSIELD = true;

    //Tipos de combustible
    public enum types { 
        stick, 
        blueStick
    }
    #region variables
    public types type;

    //Valores inicializados como un palo normal
    //A�ade tiempo de vida a la hoguera
    public double heal = 10;

    //Debuf, si el combustible es r�pido, lento...
    public double debuff;

    public bool waterSield = false;

    //tiempo que se mantiene activa la acci�n
    private double duration;
    #endregion

    public Fuel() {
        this.type = types.stick;
        init();
    }

    public Fuel(types type) : this() {
        this.type = type;
        init();
        
    }

    public void init() {
        switch (this.type)
        {
            case types.stick:
                this.heal = STICK_HEAL;
                waterSield = STICK_WATERSIELD;
                break;
            case types.blueStick:
                this.heal = BLUESTICK_HEAL;
                waterSield = BLUESTICK_WATERSIELD;
                break;
        }
    }

    

}
