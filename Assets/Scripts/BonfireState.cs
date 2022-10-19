using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireState : MonoBehaviour
{
    public double maxHp = 100;
    public double standardHpLossFactor = 1;
    public double rainingHpLossFactor = 1;
    public double hp;
    public List<Fuel> fuelList;

    //Estado de la hoguera
    public enum states { 
        encendida,
        apagada
    }
    public states state;

    //El tiempo que hace, modificar más adelante
    public enum globalState { 
        despejado,
        lluvia
    }
    public globalState tiempo = globalState.despejado;
    
    private void Start()
    {
        
        fuelList = new List<Fuel>();
        encender();
    }
    private void Update()
    {
        if (state == states.encendida && hp == 0) encender();
        if (state == states.apagada && hp > 0) apagar();

        if (hp > 0 && state == states.encendida)
            hp -= (standardHpLossFactor + rainingHpLossFactor * (int)tiempo) * Time.deltaTime;
        else
            apagar();
        
    }
    //Encender
    public void encender() { hp = maxHp; this.transform.GetComponentInChildren<ParticleSystem>().Play(); state = BonfireState.states.encendida; }
    //Aoagar : Morirse :D
    private void apagar() { hp = 0; this.transform.GetComponentInChildren<ParticleSystem>().Stop(); state = BonfireState.states.apagada; }
}
