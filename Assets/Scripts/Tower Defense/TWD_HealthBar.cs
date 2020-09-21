using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TWD_HealthBar : MonoBehaviour
{
    //Tehdään viittaus liukusäätimeen
    [SerializeField] private Slider slider;

    public void SetValue (float value)
    {
        //Asetetaan liukusäätimeen arvo raja-arvojen välille
        //Jos arvo alittaa liukusäätimen minimiarvon, asetetaan liukusäätin minimiarvoon
        if (value < slider.minValue)
        {
            value = slider.minValue;
        }
        //Jos arvo ylittää liukusäätimen minimiarvon asetetaan liukusäätin maksimiarvoon
        if (value > slider.maxValue)
        {
            value = slider.maxValue;
        }
        else
        {
            slider.value = value;
        }
    }

}
