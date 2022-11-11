using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VisualBonfire : MonoBehaviour
{
    public float targetLight;
    static float _sTargetLight;
    public bool isBlue;

    public GameObject fireParticle01Red;
    public GameObject fireParticle02Red;
    public GameObject fireParticle03Red;
    public GameObject fireParticle01Blue;
    public GameObject fireParticle02Blue;
    public GameObject fireParticle03Blue;

    int savedDay = -1;

    // Update is called once per frame
    void Update()
    {
        if(targetLight != _sTargetLight)
        {
            targetLight = _sTargetLight;
        }
        int day = SaveManager.getCurrentDay();
        if(day != savedDay) {
            savedDay = day;
            
            fireParticle01Blue.SetActive(false);
            fireParticle02Blue.SetActive(false);
            fireParticle03Blue.SetActive(false);
            fireParticle01Red.SetActive(false);
            fireParticle02Red.SetActive(false);
            fireParticle03Red.SetActive(false);

            if(isBlue) {
                if(day > 0 && day > 3) {
                    fireParticle01Blue.SetActive(true);
                }
                else if(day >= 3 && day > 5) {
                    fireParticle02Blue.SetActive(true);
                }
                else {
                    fireParticle03Blue.SetActive(true);
                }
            }
            else {
                if(day > 0 && day > 3) {
                    fireParticle01Red.SetActive(true);
                }
                else if(day >= 3 && day > 5) {
                    fireParticle02Red.SetActive(true);
                }
                else {
                    fireParticle03Red.SetActive(true);
                }
            }
        }
    }

    public static void SetTargetLight(float target) {
        _sTargetLight = target;
    }
}
