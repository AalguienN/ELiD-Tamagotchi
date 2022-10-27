using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VisualScripting.Dependencies.NCalc;
//using UnityEditor.Animations;
using UnityEngine;

public class Intensity : MonoBehaviour
{
    public float intensity = 5.9f;
    private Animator a;
    public double HP;

    private void Start() { 
    
        a= GetComponent<Animator>();
        HP = GetComponentInParent<BonfireState>().hp;

    }
    ////public void bajar()
    //{
    //    intensity = GetComponent<Light>().intensity - 0.1f;
    //    intensity = intensity*Mathf.SmoothStep(0, 1, ((float)HP)/100);
    //    Debug.Log(a.GetCurrentAnimatorStateInfo(1));
        
    //}
    public void encender()
    {
        a.SetBool("Encendida", true);
    }
    public void apagar()
    {
        a.SetBool("Encendida", false);
    }

}
