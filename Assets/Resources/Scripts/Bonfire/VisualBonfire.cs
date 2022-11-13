using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualBonfire : MonoBehaviour
{
    public float targetLight;
    static float _sTargetLight;

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
            
            int day = SaveManager.getCurrentDay();
            fireParticle01Blue.SetActive(false);
            fireParticle02Blue.SetActive(false);
            fireParticle03Blue.SetActive(false);
            fireParticle01Red.SetActive(false);
            fireParticle02Red.SetActive(false);
            fireParticle03Red.SetActive(false);

            if(targetLight==0) return;
            //if(!SaveManager.hasBurntFirstStick) return;

            if(BonfireState.isBlue && day >= 3) {
                if(day > 0 && day < 3) {
                    fireParticle01Blue.SetActive(true);
                    fireParticle01Blue.GetComponent<ParticleSystem>().Play();
                }
                else if(day >= 3 && day < 5) {
                    fireParticle02Blue.SetActive(true);
                    fireParticle02Blue.GetComponent<ParticleSystem>().Play();
                }
                else {
                    fireParticle03Blue.SetActive(true);
                    fireParticle03Blue.GetComponent<ParticleSystem>().Play();
                }
            }
            else {
                if(day > 0 && day < 3) {
                    fireParticle01Red.SetActive(true);
                    fireParticle01Red.GetComponent<ParticleSystem>().Play();
                }
                else if(day >= 3 && day < 5) {
                    fireParticle02Red.SetActive(true);
                    fireParticle02Red.GetComponent<ParticleSystem>().Play();
                }
                else {
                    fireParticle03Red.SetActive(true);
                    fireParticle03Red.GetComponent<ParticleSystem>().Play();
                }
            }
        }
    }

    public static void SetTargetLight(float target) {
        _sTargetLight = target;
    }
}
