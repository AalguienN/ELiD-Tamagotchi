using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public static bool isBlue;

    [Header("Loss Factor")]
    public double standardHpLossFactor = 1;
    public double rainingHpLossFactor = 1;
    
    [Header("Testing: remove later \n USAD ESTO COMO BOTONES \n PARA ENCENDER Y APAGAR LA HOGUERA")]

    public bool turnOn;
    public bool turnOff;

    #endregion

    public static int seconds;

    #region enums
    //Estado de la hoguera
    public enum states { 
        nada,
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
        
        //this.transform.GetComponentInChildren<ParticleSystem>().Stop(); 
        StartCoroutine(bonfireTick());

        lit(SaveManager.getFireState()-tiempoFuera);
        if(SaveManager.fuelList.Count != 0) {
            isBlue = true;
            Fuel f = SaveManager.fuelList[SaveManager.fuelList.Count-1];
            f.duration -= tiempoFuera;
            if(f.duration < 0) {
                SaveManager.fuelList.Remove(f);
            }
        }
        else
        {
            isBlue = false;
        }

        Debug.Log("El juego ha estado cerrado " + Mathf.Abs(tiempoFuera) + " segundos");
    }
    private void Update()
    {
        //For testing remove later
        if (turnOn) { lit(); turnOn = false; }
        if (turnOff) { extinguish(); turnOff = false;  }

        if(SaveManager.fuelList.Count != 0) {
            isBlue = true;
            Fuel f = SaveManager.fuelList[SaveManager.fuelList.Count-1];
            f.duration -= Time.deltaTime;
            if(f.duration < 0) {
                SaveManager.fuelList.Remove(f);
            }
        }
        else
        {
            isBlue = false;
        }

        if(hp > 0 && state != states.encendida) 
        {
            //this.transform.GetComponentInChildren<ParticleSystem>().Play(); 
            state = BonfireState.states.encendida; 
            GetComponentInChildren<Light>().GetComponent<Intensity>().encender();
            VisualBonfire.Instance.ChangeBonfire();
        }
        else if(hp <= 0 && state != states.apagada)
        {
            //this.transform.GetComponentInChildren<ParticleSystem>().Stop(); 
            state = BonfireState.states.apagada;
            GetComponentInChildren<Light>().GetComponent<Intensity>().apagar();
            VisualBonfire.Instance.ChangeBonfire();
            //TODO: trigger endgame
            if(SaveManager.hasBurntFirstStick) {
                Destroy(GameObject.FindWithTag("DialogueManager"));
                SceneManager.LoadScene(2);
            }
        }
    }
    #endregion

    #region bonfireTick_IEnumerator
    //Called each tick to update campfire
    private IEnumerator bonfireTick() {
        while (true)
        {
            if (hp > 0 && state == states.encendida)
                hp -= (standardHpLossFactor + rainingHpLossFactor * (int)weather);
                       
            /*else
                extinguish();*/
            //Actualizar en el momento que hp = 0
            if (hp <= 0 && state == states.encendida) 
                extinguish();
            yield return new WaitForSeconds(tickRate);
        }

    }
    #endregion

    #region methods
    //lits up the bonfire
    public void lit() { 
        hp = maxHp;  
    }
    public void lit(int modifier) { 
        hp = modifier;
    }

    //extinguish the bonfire
    private void extinguish() { 
        hp = 0;
    }

    public void heal(double n) {
        if (hp + n < maxHp && hp != 0)
            hp += n;
        else lit();
    }
    public void addFuel(Fuel.types fuel) {
        Fuel f = new Fuel(fuel);
        f.init();
        heal(f.heal);
        if(fuel == Fuel.types.blueStick) 
        {
            SaveManager.fuelList.Add(f);
        }
    }



    #endregion
}