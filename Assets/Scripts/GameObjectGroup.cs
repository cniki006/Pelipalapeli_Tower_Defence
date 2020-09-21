using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameObjectGroup
{
    // Lista peliobjekteista.
    [SerializeField] private List<GameObject> group;
    // Viimeksi käsitelty peliobjekti.
    private GameObject selected;
    // Mitattu etäisyys.
    // Käytetään objektien välisen etäisyyden määrittämisessä.
    private float distance;

    // Lista ryhmän jäsenten painoarvoista.
    // Vaikuttaa jäsenen valintatodennäköisyyteen.
    [SerializeField] private List<int> weightList;


    /// <summary>
    /// <para> Palauttaa peliobjektiryhmän jäsenen annetun indeksin kohdalta. </para>
    /// </summary>
    public GameObject GetAtIndex(int index)
    {
        if (group.Count > 0)
        {
            if (group[index] != null)
            {
                return group[index];
            }
        }
        return null;
    }

    /// <summary>
    /// <para> Palauttaa ryhmän jäsenten lukumäärän. </para>
    /// </summary>
    public int GetSize()
    {
        if (group.Count > 0)
        {
            return group.Count;
        }
        else
        {
            return 0;
        }
    }

    /// <summary>
    /// <para> Muutetaan painoarvolistasta annetun indeksin kohdalta jäsenen arvoa. </para>
    /// </summary>
    public void SetWeight(int index, int value)
    {
        if (index >= 0 && index < weightList.Count)
        {
            weightList[index] = value;
        }
    }


    // Peliobjektiryhmä-luokan konstruktori.
    public GameObjectGroup()
    {
        group = new List<GameObject>();
    }


    /// <summary>
    /// <para> Palauttaa tiedon, onko peliobjektiryhmä tyhjä. </para>
    /// </summary>
    public bool IsEmpty()
    {
        // Ryhmä ei ole tyhjä, jos listassa on jäseniä.
        if (group.Count > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    /// <summary>
    /// <para> Lisätään peliobjekti ryhmään. </para>
    /// </summary>
    public void Add(GameObject obj)
    {
        group.Add(obj);
    }

    /// <summary>
    /// <para> Poistetaan peliobjekti ryhmästä. </para>
    /// </summary>
    public void Remove(GameObject obj)
    {
        group.Remove(obj);
    }

    /// <summary>
    /// <para> Tyhjennetään ryhmä. </para>
    /// </summary>
    public void Clear()
    {
        group.Clear();
    }


    /// <summary>
    /// <para> Poistaa ryhmästä tyhjät alkiot. </para>
    /// </summary>
    public void CleanUp()
    {
        List<int> indexList = new List<int>();

        // Kun ryhmässä on jäseniä.
        if (group.Count > 0)
        {
            // Päivitetään ryhmää: otetaan ylös listan tyhjien alkioiden indeksit.
            for (int i = 0; i < group.Count; i++)
            {
                if (group[i] == null)
                {
                    indexList.Add(i);
                }
            }
            // Päivitetään ryhmää: poistetaan tyhjät alkiot indeksien osoittamista paikoista.
            if (indexList.Count > 0)
            {
                for (int i = indexList.Count - 1; i >= 0; i--)
                {
                    group.RemoveAt(indexList[i]);
                }
            }
        }
    }

    /// <summary>
    /// <para> Poistaa ryhmästä tyhjät alkiot. </para>
    /// </summary>
    public void CleanUp(bool includeDisabled)
    {
        List<int> indexList = new List<int>();

        // Kun ryhmässä on jäseniä.
        if (group.Count > 0)
        {
            // Päivitetään ryhmää: otetaan ylös listan tyhjien alkioiden indeksit.
            for (int i = 0; i < group.Count; i++)
            {
                if (group[i] == null || (!group[i].activeInHierarchy && includeDisabled))
                {
                    indexList.Add(i);
                }
            }
            // Päivitetään ryhmää: poistetaan tyhjät alkiot indeksien osoittamista paikoista.
            if (indexList.Count > 0)
            {
                for (int i = indexList.Count - 1; i >= 0; i--)
                {
                    group.RemoveAt(indexList[i]);
                }
            }
        }
    }

    /// <summary>
    /// <para> Palauttaa pistettä lähimmän ryhmän jäsenen. </para>
    /// </summary>    
    public GameObject Nearest(Vector3 location)
    {
        if (!IsEmpty())
        {
            CleanUp();

            // Verrataan ryhmän jäsenten etäisyyksiä pisteeseen.
            distance = Mathf.Infinity;
            foreach (GameObject member in group)
            {
                if ((member.transform.position - location).magnitude < distance)
                {
                    distance = (member.transform.position - location).magnitude;
                    // Asetetaan lähin jäsen käsiteltäväksi jäseneksi.
                    selected = member;
                }
            }
            return selected;
        }
        else
        {
            return null;
        }
    }

    /// <summary>
    /// <para> Haetaan ryhmästä satunnainen jäsen. </para>
    /// </summary>    
    public GameObject GetRandom()
    {
        // Jos ryhmässä ei ole jäseniä, satunnaista ryhmän jäsentä ei voida löytää ja täten myöskään palauttaa.
        if (IsEmpty())
        {
            Debug.LogError("PPP Error: Group has no members, can not proceed with method GetRandom.");
            return null;
        }

        CleanUp();

        // Tarkistetaan, onko painoarvolista oikean kokoinen.
        ValidateWeightList();

        // Arvotaan luku yhdestä painoarvolistan kokonaissummaan.
        int max = 0;
        foreach (int weight in weightList)
        {
            max = max + weight;
        }

        int random = Random.Range(1, max + 1);

        // Valitaan satunnaisesti alkio vihollislistasta, 
        // käyttäen satunnaislukua ja vihollisten painoarvolistaa.
        for (int i = 0; i < weightList.Count && random > 0; i++)
        {
            if (random > weightList[i])
            {
                random = random - weightList[i];
            }
            else
            {
                return group[i];
            }
        }

        Debug.LogError("PPP Error: Random selection failed. A member has a weight value of 0. Returning the group's first member.");
        // Palauttaa ryhmän ensimmäisen jäsenen, jos mitään muuta ei onnistuttu palauttamaan.
        return group[0];
    }

    private void ValidateWeightList()
    {
        // Katsotaan, onko painoarvolista suurempi kuin ryhmä.
        if (weightList.Count > group.Count)
        {
            // Poistetaan alkioita painoarvolistasta.
            weightList.RemoveRange(group.Count + 1, weightList.Count - group.Count);
        }
        // Katsotaan, onko painoarvolista pienempi kuin ryhmä.
        else if (weightList.Count < group.Count)
        {
            // Lisätään alkioita painoarvolistaan.
            while (weightList.Count < group.Count)
            {
                weightList.Add(0);
            }
        }
    }
}
