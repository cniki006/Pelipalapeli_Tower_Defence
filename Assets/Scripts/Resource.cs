using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Resource
{
    // Resurssin minimiarvo.
    [SerializeField] private float min;
    // Resurssin maksimiarvo.
    [SerializeField] private float max;
    // Resurssin tämänhetkinen arvo.
    private float value;

    public float GetMax() { return max; }
    public float GetMin() { return min; }
    public float GetValue() { return value; }
    public void SetMax(float v) { max = v; }
    public void SetMin(float v) { min = v; }
    public void SetValue(float v)
    {
        // Ei anneta arvon mennä alle minimin tai yli maksimin.
        if (v < min)
        {
            value = min;
        }
        else if (v > max)
        {
            value = max;
        }
        else
        {
            value = v;
        }
    }

    /// <summary>
    /// <para> Parametreiksi asetetaan minimi- sekä maksimiarvo (min, max). Tämänhetkiseksi arvoksi asetetaan minimiarvo. </para>
    /// </summary>
    public Resource(float min, float max)
    {
        this.min = min;
        this.max = max;
        this.value = min;
    }

    /// <summary>
    /// <para> Muokkaa resurssiarvoa annetun arvon mukaan (+/-). </para>
    /// </summary>
    public void Modify(float a)
    {
        value = value + a;

        SetValue(value);
    }
}
