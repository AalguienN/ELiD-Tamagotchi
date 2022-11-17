using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualBonfire : MonoBehaviour
{
    public float targetLight;
    static float _sTargetLight = -1;
    static bool _sIsBlue;

    public GameObject fireParticle01Red;
    public GameObject fireParticle02Red;
    public GameObject fireParticle03Red;
    public GameObject fireParticle01Blue;
    public GameObject fireParticle02Blue;
    public GameObject fireParticle03Blue;

    // Update is called once per frame
    void Update()
    { 
        if(targetLight != _sTargetLight)
        {
            targetLight = _sTargetLight;

            ChangeBonfire();
        }

        if(_sIsBlue != BonfireState.isBlue) {
            _sIsBlue = BonfireState.isBlue;
            
            ChangeBonfire();
        }
    }

    void ChangeBonfire() {
        int day = SaveManager.getCurrentDay();
            
        fireParticle01Blue.GetComponent<ParticleSystem>().Stop();
        fireParticle02Blue.GetComponent<ParticleSystem>().Stop();
        fireParticle03Blue.GetComponent<ParticleSystem>().Stop();
        fireParticle01Red.GetComponent<ParticleSystem>().Stop();
        fireParticle02Red.GetComponent<ParticleSystem>().Stop();
        fireParticle03Red.GetComponent<ParticleSystem>().Stop();

        if(targetLight==0) return;
        //if(!SaveManager.hasBurntFirstStick) return;

        if(BonfireState.isBlue && day >= 3) {
            if(day > 0 && day < 3) {
                fireParticle01Blue.GetComponent<ParticleSystem>().Play();
            }
            else if(day >= 3 && day < 5) {
                fireParticle02Blue.GetComponent<ParticleSystem>().Play();
            }
            else {
                fireParticle03Blue.GetComponent<ParticleSystem>().Play();
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

    public static void SetTargetLight(float target) {
        _sTargetLight = target;
    }
}
