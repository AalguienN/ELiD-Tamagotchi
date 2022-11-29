using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    
    #region Variables
    public int currentDay;
    public bool isDed;

    public GameObject Forest;
    public GameObject ForestFire;
    public GameObject DarkForest;
    public GameObject Death;
    #endregion
    
    // En teoría, solo hay que llamar a esta función al abrir el juego, o cuando otro momento lo requiera
    void Start()
    {
        //Retrieve isDed from save (Es decir, mira a ver si estás muerto para no cambiar la música sin sentido)
        isDed = false; //Por ahora false hasta que se implemente
        currentDay = SaveManager.getCurrentDay();
        if (!isDed){
            if (currentDay < 3) {
                Forest.SetActive(true);
            }
            else if (currentDay < 7) {
                DarkForest.SetActive(true);
            } 
            else {
                ForestFire.SetActive(true);
            }
        }
    }

    //Función para cuando te mueres (Básicamente se llama desde otro script y cambia la pista de música que está sonando a la de muerte)
}
