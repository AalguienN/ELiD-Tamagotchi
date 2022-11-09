using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BonfireState : MonoBehaviour
{
    public static BonfireState Instance { get; private set; }

    [Header("References")]
    //For now is empty
    
    #region Variables
    [Header("TickRate in seconds")]
    public float tickRate = 1f;
    

    [Header("Stats")]
    public double hp;
    public double maxHp = 1000;
    public states state;
    public globalState weather = globalState.despejado; //Provisional

    [Header("Loss Factor")]
    public double standardHpLossFactor = 1;
    public double rainingHpLossFactor = 1;
    
    public List<Fuel> fuelList;

    [Header("Testing: remove later \n USAD ESTO COMO BOTONES \n PARA ENCENDER Y APAGAR LA HOGUERA")]

    public bool turnOn;
    public bool turnOff;

    #endregion

    public static int seconds;

    #region enums
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
    #endregion

    #region Awake
    void Awake() {
        Instance = this; 
    }
    #endregion

    #region Start_And_Update
    private void Start()
    {
        turnOn = false; turnOff = false;

        int tiempoFuera = SaveManager.getSecondsSinceLastConexion();

        Debug.Log(SaveManager.getFireState());
        fuelList = new List<Fuel>();
        lit(-tiempoFuera, SaveManager.getFireState());
        StartCoroutine(bonfireTick());

        Debug.Log("El juego ha estado cerrado " + Mathf.Abs(tiempoFuera) + " segundos");
    }
    private void Update()
    {
        //For testing remove later
        if (turnOn) { lit(); turnOn = false; }
        if (turnOff) { extinguish(); turnOff = false;  }

    }
    #endregion

    #region bonfireTick_IEnumerator
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
    #endregion

    #region methods
    //lits up the bonfire
    public void lit() { hp = maxHp; this.transform.GetComponentInChildren<ParticleSystem>().Play(); state = BonfireState.states.encendida; GetComponentInChildren<Light>().GetComponent<Intensity>().encender(); }
    public void lit(int modifier) { hp = maxHp + modifier; this.transform.GetComponentInChildren<ParticleSystem>().Play(); state = BonfireState.states.encendida; }
    public void lit(int modifier, int ini) { 
        hp = ini + modifier; 
        this.transform.GetComponentInChildren<ParticleSystem>().Play(); 
        state = BonfireState.states.encendida; 
    }

    //extinguish the bonfire
    private void extinguish() { hp = 0; this.transform.GetComponentInChildren<ParticleSystem>().Stop(); state = BonfireState.states.apagada; GetComponentInChildren<Light>().GetComponent<Intensity>().apagar(); }

    public void heal(double n) {
        if (hp + n < maxHp && hp != 0)
            hp += n;
        else lit();
    }
    public void addFuel(Fuel.types fuel) {
        fuelList.Add(new Fuel(fuel));
        Fuel f = new Fuel(fuel);
        f.init();
        heal(f.heal);
    }


    #endregion
}
