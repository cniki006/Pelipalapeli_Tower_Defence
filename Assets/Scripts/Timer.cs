using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Timer
{
    // Ajastimen maksimiarvo.
    [SerializeField] private float max;
    // Ajastimen tämänhetkinen arvo.
    private float current;

    public float GetMax() { return max; }
    public void SetMax(float m) { max = m; }
    public float GetCurrent() { return current; }
    public void SetCurrent(float c) { current = c; }


    /// <summary>
    /// <para> Asetetaan maksimi- ja lähtöarvo. </para>
    /// </summary>
    public Timer(float m, float c)
    {
        this.max = m;
        this.current = c;
    }

    /// <summary>
    /// <para> Laskee aikaa. Tarkoitettu kutsuttavaksi per frame. </para>
    /// </summary>
    public void Count()
    {
        if (current <= max)
        {
            current = current + Time.deltaTime;
        }
    }

    /// <summary>
    /// <para> Kertoo, onko ajastin saavuttanut maksimiarvon. </para>
    /// </summary>
    public bool IsFinished()
    {
        if (current >= max)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// <para> Nollaa ajastimen. </para>
    /// </summary>
	public void Reset()
    {
        current = 0;
    }
}
