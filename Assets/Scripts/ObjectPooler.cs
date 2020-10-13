using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    // Peliobjektimalli.
    [SerializeField] private GameObject pooledObject;
    // Kantaan varattavien peliobjektien lukumäärä.
    [SerializeField] private int pooledAmount;
    // Voidaanko kantaa kasvattaa ajon aikana.
    [SerializeField] private bool canExpand;

    // Lista kaikista kantaan kuuluvista peliobjekteista.
    private List<GameObject> pooledObjects;

    private void Start ()
    {
        // Luodaan ja alustetaan lista. Täytetään se peliobjektimallin mukaisilla peliobjekteilla.
        pooledObjects = new List<GameObject>();
        for (int i = 0; i < pooledAmount; i++)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);
            obj.SetActive(false);
            pooledObjects.Add(obj);
        }
	}

    /// <summary>
    /// <para> Palauttaa ensimmäisen käytettävissä olevan peliobjektin tietokannsta. </para>
    /// </summary>
    public GameObject GetPooledObject()
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy)
            {
                return pooledObjects[i];
            }
        }
        // Luodaan kantaan lisää peliobjekteja, jos kantaa voidaan kasvattaa.
        if (canExpand)
        {
            GameObject obj = (GameObject)Instantiate(pooledObject);

            pooledObjects.Add(obj);

            Debug.Log(this.gameObject.name + " pool size: " + pooledObjects.Count);
            return obj;
        }

        Debug.LogError("PPP Error: All objects in this pool are already in use!");
        return null;
    }

    /// <summary>
    /// <para> Palauttaa peliobjektitietokannan jäsenlistan. </para>
    /// </summary>
    public List<GameObject> GetPooledObjects()
    {
        return pooledObjects;
    }

    /// <summary>
    /// <para> Aktivoi tietokantaan kuuluvan peliobjektin, asettaen sille annetun sijainnin (pos) ja kierron (rot). </para>
    /// </summary>
    public void Activate(Vector3 pos, Quaternion rot)
    {
        GameObject obj = GetPooledObject();
        obj.transform.position = pos;
        obj.transform.rotation = rot;
        obj.SetActive(true);
    }

    /// <summary>
    /// <para> Aktivoi tietokantaan kuuluvan peliobjektin, asettaen sille toisen peliobjektin (spawner) sijainnin ja kierron. </para>
    /// </summary>
    public void Activate(GameObject spawner)
    {
        GameObject obj = GetPooledObject();
        obj.transform.position = spawner.transform.position;
        obj.transform.rotation = spawner.transform.rotation;
        obj.SetActive(true);
    }
}
