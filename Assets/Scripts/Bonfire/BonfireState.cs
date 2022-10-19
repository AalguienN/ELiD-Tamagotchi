using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireState : MonoBehaviour
{
    [Header("TickRate in seconds")]
    public float tickRate = 1f;
    

    [Header("Stats")]
    public double hp;
    public double maxHp = 100;
    public states state;
    public globalState weather = globalState.despejado; //Provisional

    [Header("Loss Factor")]
    public double standardHpLossFactor = 1;
    public double rainingHpLossFactor = 1;
    
    public List<Fuel> fuelList;

    [Header("Testing remove later")]
    public bool turnOn;
    public bool turnOff;

    //Estado de la hoguera
    public enum states { 
        encendida,
        apagada
    }

    

    //Weather, modify in the future to merge with the rest
    public enum globalState { 
        despejado,
        lluvia
    }
    
    private void Start()
    {
        fuelList = new List<Fuel>();
        lit();
        StartCoroutine(bonfireTick());
    }
    private void Update()
    {
        //For testing remove later
        if (turnOn) { lit(); turnOn = false; }
        if (turnOff) { extinguish(); turnOff = false; }

    }

    //Called each tick to update campfire
    private IEnumerator bonfireTick() {
        while (true)
        {
            
            yield return new WaitForSeconds(tickRate);
            if (hp > 0 && state == states.encendida)
                hp -= (standardHpLossFactor + rainingHpLossFactor * (int)weather);
                       
            /*else
                extinguish();*/
            //Actualizar en el momento que hp = 0
            if (hp <= 0 && state == states.encendida) 
                extinguish();
        }

    }

    //lits up the bonfire
    public void lit() { hp = maxHp; this.transform.GetComponentInChildren<ParticleSystem>().Play(); state = BonfireState.states.encendida; }

    //extinguish the bonfire
    private void extinguish() { hp = 0; this.transform.GetComponentInChildren<ParticleSystem>().Stop(); state = BonfireState.states.apagada; }
}
