using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    // Janan alkupiste.
    private Vector3 pointA;
    // Janan loppupiste.
    private Vector3 pointB;

    public Vector3 GetPointA() { return pointA; }
    public Vector3 GetPointB() { return pointB; }


    /// <summary>
    /// <para> Asetetaan alku- ja loppupiste vektoreina, jotka määritellään pisteinä 3D-avaruudessa. </para>
    /// </summary>
    public Line(Vector3 a, Vector3 b)
    {
        this.pointA = a;
        this.pointB = b;
    }

    /// <summary>
    /// <para> Katsoo, onko jana pysty- tai vaaka-akselin suuntainen. </para>
    /// </summary>
    public string CheckDirection()
    {
        if (pointA.y == pointB.y)
        {
            return "horizontal";
        }
        else if (pointA.x == pointB.x)
        {
            return "vertical";
        }
        else
        {
            return "diagonal";
        }
    }

    /// <summary>
    /// <para> Palauttaa janan pituuden. </para>
    /// </summary>
    public float GetMagnitude()
    {
        return Vector3.Magnitude(pointA + pointB);
    }

    // Vertailuoperaattorin kuormitus, jotta janoja voidaan verrata toisiinsa (yhtäsuuruus).
    public static bool operator ==(Line a, Line b)
    {
        if (a.GetPointA() == b.GetPointA() && a.GetPointB() == b.GetPointB())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    // Vertailuoperaattorin kuormitus, jotta janoja voidaan verrata toisiinsa (erisuuruus).
    public static bool operator !=(Line a, Line b)
    {
        if (a.GetPointA() == b.GetPointA() && a.GetPointB() == b.GetPointB())
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    // ÄLÄ KÄYTÄ
    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }
    // ÄLÄ KÄYTÄ
    public override int GetHashCode()
    {
        return base.GetHashCode();
    }
}
