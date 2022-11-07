using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class hpBarController : MonoBehaviour
{
    private GameObject bonfire;
    public Gradient gradient;
    public Image fill;

    // Start is called before the first frame update
    void Start()
    {
        bonfire = GameObject.FindWithTag("Bonfire2");
        this.GetComponent<Slider>().maxValue = (float)bonfire.GetComponent<BonfireState>().maxHp;
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Slider>().value = (float) bonfire.GetComponent<BonfireState>().hp;
        fill.color = gradient.Evaluate(this.GetComponent<Slider>().normalizedValue);
    }
}
