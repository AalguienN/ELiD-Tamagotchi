using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class updates the state of the bonfire for the FMOD emitter
/// </summary>

public class BonfireAudio : MonoBehaviour
{
    
    public double hp;
    public float effectiveHP;
    public states state;
    public float effectiveState;
    
    //Estado de la hoguera
    public enum states { 
        nada,
        encendida,
        apagada
    }

    void Awake()
    {

    }

    // Update is called once per frame
    void Update()
    {

            // Obtener la vida y el estado de la hoguera

        hp = BonfireState.getBonfireHP();

        effectiveHP = (float)hp;

        state = (states)BonfireState.getBonfireState();

        effectiveState = state != states.encendida ? 0 : 1;

        // Llamar al emitter de FMOD

        var emitter = GetComponent<FMODUnity.StudioEventEmitter>();

        emitter.SetParameter("bonfireHP", effectiveHP);
        emitter.SetParameter("Bonfire State", effectiveState);                                                     

    }
}
