using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class TWD_Path
{
    //Lista reitin pisteistä
    [SerializeField] private List<GameObject> pathList;
    //Reitin pituus
    private float pathLenght;

    public float GetPathLenght() { return pathLenght; }
    public void SetPathLenght() { pathLenght = CalculatePathLenght(); }

    //Palauttaa reitin pisteen annetun indeksin kohdalta
    public GameObject GetAtIndex(int index)
    {
        //Palautetaan viittaus listan tiettyyn tarkistuspisteeseen
        if (pathList.Count > 0)
        {
            if (pathList[index] !=null)
            {
                return pathList[index];
            }
        }
        return null;
    }

    //Palauttaa reitin pisteiden lukumäärän 
    public int GetSize()
    {
        //Palautetaan reitin koko eli pisteiden lukumäärän
        if (pathList.Count > 0)
        {
            return pathList.Count;
        }
        else
        {
            return 0;
        }
    }

    //Reitti-dataluokkaan konstruktori
    public TWD_Path ()
    {
        pathList = new List<GameObject>();
    }

    // Reitin pituuden laskeminen
    // Palauttaa reitin pituudeksi 0, jos reitin pituutta ei voidaan laskea
    private float CalculatePathLenght()
    {
        //Lasketaan reitin pituus kaikkien reittipisteiden välisten etäisyyksien avulla
        if(pathList.Count > 1)
        {
            if (IsPathValid())
            {
                float lenght = 0.0f;
                //Käydään läpi kaikki reitin pisteet
                for (int i = 1; i < pathList.Count; i++)
                {
                    //Kasvatetaan reitin pituutta tämänhetkisen ja edellisen pisteen välisen etäisyyden verran
                    lenght = lenght + (pathList[i].transform.position - pathList[i - 1].transform.position).magnitude;

                }
                return lenght;
            }
        }
        return 0.0f;
    }

    //Palauttaa tiedon, onko reitillä tyhjiä pisteitä
    // true, kun reitillä ei ole tyhjiä pisteitä
    // false, kun reitillä on tyhjiä pisteitä
    private bool IsPathValid()
    {
        //Käydään läpi kaikki reitin pisteet
        foreach (GameObject pathNote in pathList)
        {
            //Tarkastetaan onko reitin piste tyhjä
            if(pathNote == null)
            {
                return false;
            }
        }
        return true;
    }
}
