using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class updates the state of the bonfire for the FMOD emitter
/// </summary>

public class BonfireAudio : MonoBehaviour
{
    
    public double hp;
    [HideInInspector]public float effectiveHP;
    public states state;
    [HideInInspector]public float effectiveState;
    public int currentDay;
    [HideInInspector]public float effectiveCurrentDay;
    public bool lastBlueStick;
    [HideInInspector]public float effectiveLastBlueStick;

    
    BonfireState bonfire;


    //Estado de la hoguera
    public enum states { 
        nada,
        encendida,
        apagada
    }

    

    void Awake()
    {
        bonfire = GetComponent<BonfireState>(); //Abre el rigidbody de la hoguera como component
    }

    // Update is called once per frame
    void Update()
    {

            // Obtener la vida y el estado de la hoguera

        hp = bonfire.hp;
        effectiveHP = (float)hp;

        state = (states)bonfire.state;
        effectiveState = state != states.encendida ? 0 : 1;

        currentDay = SaveManager.getCurrentDay();
        effectiveCurrentDay = (float)currentDay;

        lastBlueStick = SaveManager.hasBurntLastBlueStick;
        effectiveLastBlueStick = lastBlueStick != true ? 0 : 1;


        // Llamar al emitter de FMOD

        var emitter = GetComponent<FMODUnity.StudioEventEmitter>();

        emitter.SetParameter("bonfireHP", effectiveHP);
        emitter.SetParameter("Bonfire State", effectiveState); 
        emitter.SetParameter("Current Day", effectiveCurrentDay);
        emitter.SetParameter("LastBlueStickBurn", effectiveLastBlueStick);



    }
}
