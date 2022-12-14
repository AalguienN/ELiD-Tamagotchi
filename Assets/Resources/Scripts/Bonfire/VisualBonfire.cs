using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualBonfire : MonoBehaviour
{
    public static VisualBonfire Instance { get; private set; }

    static bool _sIsBlue;

    public GameObject fireParticle01Red;
    public GameObject fireParticle02Red;
    public GameObject fireParticle03Red;
    public GameObject fireParticle01Blue;
    public GameObject fireParticle02Blue;
    public GameObject fireParticle03Blue;
    public GameObject fireParticleFinale;
    private bool aux = false;
    #region Awake
    void Awake() {
        Instance = this; 
    }
    #endregion

    // Update is called once per frame
    void Update()
    {

        if(_sIsBlue != BonfireState.isBlue || SaveManager.hasBurntLastBlueStick && !aux) {
            if (SaveManager.hasBurntLastBlueStick) { aux = true; }
            _sIsBlue = BonfireState.isBlue;
            
            ChangeBonfire();
        }
    }

    public void ChangeBonfire() {
        int day = SaveManager.getCurrentDay();
            
        fireParticle01Blue.GetComponent<ParticleSystem>().Stop();
        fireParticle02Blue.GetComponent<ParticleSystem>().Stop();
        fireParticle03Blue.GetComponent<ParticleSystem>().Stop();
        fireParticle01Red.GetComponent<ParticleSystem>().Stop();
        fireParticle02Red.GetComponent<ParticleSystem>().Stop();
        fireParticle03Red.GetComponent<ParticleSystem>().Stop();
        fireParticleFinale.GetComponent<ParticleSystem>().Stop();

        if(BonfireState.Instance.state==BonfireState.states.apagada) return;
        //if(!SaveManager.hasBurntFirstStick) return;


        if(BonfireState.isBlue && day >= 3) {
            fireParticle02Blue.GetComponent<ParticleSystem>().Play();


            if (SaveManager.hasBurntLastBlueStick && day == 6) {
                fireParticle02Blue.GetComponent<ParticleSystem>().Stop();
                fireParticle03Blue.GetComponent<ParticleSystem>().Play();
            }
            else if(SaveManager.hasBurntLastBlueStick && day > 6) { //FINAL DAY ANIMATION
                fireParticleFinale.GetComponent<ParticleSystem>().Play();
                fireParticle02Blue.GetComponent<ParticleSystem>().Stop();
                fireParticle03Blue.GetComponent<ParticleSystem>().Stop();

                GetComponentInParent<GameObject>().GetComponentInChildren<Light>().GetComponent<Intensity>().apagar();
            }
        }
        else {
            if(day > 0 && day < 3) {
                fireParticle01Red.GetComponent<ParticleSystem>().Play();
            }
            else if(day >= 3 && day < 5) {
                fireParticle02Red.GetComponent<ParticleSystem>().Play();
            }
            else {
                fireParticle03Red.GetComponent<ParticleSystem>().Play();
            }
        }
    }
}
