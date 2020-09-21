using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_Spawner : MonoBehaviour {

    // Luotujen peliobjektien käyttäytymistavat.
    private enum Type { Line, Pop }
    // Luotujen peliobjektien käyttäytymistapa.
    [SerializeField] private Type spawnerType = Type.Line;

    // Luomistahti, kuinka monta peliobjektia luodaan per sekunti.
    [SerializeField] private float spawnRate = 1;
    // Viive luomisen aloittamiseen sekunteina.
    [SerializeField] private float spawnDelay = 0;
    // Luomisajastin.
    private Timer spawnTimer;

    // Alkupiste luoduille peliobjekteille.
    [SerializeField] private Transform spawnPoint;
    // Loppupiste luoduille peliobjekteille.
    [SerializeField] private Transform endPoint;
    // Lista luotavista objekteista.
    [SerializeField] private List<GameObject> spawnedObjects;

    // Apumuuttuja, joka kertoo onko luominen aloitettu vai ei.
    private bool started = false;

    public List<GameObject> GetSpawnedObjects() { return spawnedObjects; }

    private void Start() {
        // Alustetaan luomisajastin annettujen arvojen perusteella.
        spawnTimer = new Timer(spawnRate, spawnRate - spawnDelay);
    }

    private void Update() {
        // Kutsutaan luomisalgoritmia per frame.
        Spawn();
    }

    // Luomisalgoritmi.
    private void Spawn() {
        // Jos luotavia peliobjekteja on vielä jäljellä ja luominen on aloitettu.
        if(spawnedObjects.Count > 0 && started) {
            // Jos luomisajastin on laskenut loppuun.
            if (spawnTimer.IsFinished())
            {
                // Resetoidaan luomisajastin.
                spawnTimer.Reset();

                // Luodaan peliobjekti ja annetaan sille käyttäytymistapa, aloitus- ja lopetussijainnit sekä viive.
                GameObject cloneTarget = Instantiate(spawnedObjects[0], spawnPoint.position, spawnPoint.rotation);
                //cloneTarget.GetComponent<FPS_Target>().Initialize((int)spawnerType, spawnPoint.position, endPoint.position, spawnRate);

                // Peliobjektin luomisen jälkeen poistetaan peliobjekti listasta.
                spawnedObjects.RemoveAt(0);
            }
            // Jos luomisajastin ei ole laskenut loppuun, lasketaan aikaa.
            else
            {
                spawnTimer.Count();
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        // Kun pelaaja törmää tähän peliobjektiin, määritetään luominen alkaneeksi.
        if(other.CompareTag("Player")) {
            started = true;
        }
    }
}
