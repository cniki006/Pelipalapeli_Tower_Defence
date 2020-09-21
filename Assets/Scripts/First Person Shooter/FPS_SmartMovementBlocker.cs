using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPS_SmartMovementBlocker : MonoBehaviour {

    // Viittaus oletetusti viimeiseen maalitauluspawneriin.
    [SerializeField] private FPS_Spawner finalSpawner;
    // Viittaus tämän peliobjektin Collider-komponenttiin.
    [SerializeField] private Collider myCollider;
    // Viittaus suuntanuoleen.
    [SerializeField] private GameObject myArrow;
    // Ajastin suuntanuolen tuhoamiselle.
    private Timer arrowTimer = new Timer(2.5f, 0.0f);

    private void Start() {
        // Disabloidaan suuntanuoli.
        myArrow.SetActive(false);
    }

    private void Update () {
        // Jos viittausta viimeiseen maalitauluspawneriin ei ole, päätetään Update-metodin läpikäynti tähän.
        if (finalSpawner == null) {
            Debug.LogError("PPP Error: SmartMovementBlocker spawner not set.");
            return;
        }

        // Kun viimeinen spawneri on tyhjentynyt, lopetetaan liikkumisen estäminen.
        if (finalSpawner.GetSpawnedObjects().Count == 0)
        {
            StopBlocking();
        }
	}

    // Liikkumiseneston päättyminen.
    private void StopBlocking() {
        // Disabloidaan Collider-komponentti.
        myCollider.enabled = false;
        // Aktivoidaan suuntanuoli.
        myArrow.SetActive(true);

        // Jos suuntanuolen tuhoamisajastin on laskenut loppuun, disabloidaan suuntanuoli.
        if(arrowTimer.IsFinished()) {
            myArrow.SetActive(false);
        }
        // Jos suuntanuolen tuhoamisajastin ei ole laskenut loppuun, lasketaan aikaa.
        else {
            arrowTimer.Count();
        }
    }
}
