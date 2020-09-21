using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCG_Player : MonoBehaviour {

    // Auton tilat.
    private enum VehicleState { accelerating, idle }

    // Pelaajan auton tila.
    private VehicleState state = VehicleState.accelerating;

    // Viittaus pelimanageriin.
    private RCG_GameManager gameManager;
    // Viittaus käyttöliittymämanageriin.
    private RCG_UIManager uiManager;
    // Viittaus autoluokkaan.
    private RCG_CarController carController;
    // Viittaus pelaajan AudioSource-komponenttiin.
    private AudioSource audioSource;

    // Viittaus savuefektien juuriobjektiin.
    [SerializeField] private Transform smokeEffectsRoot;
    // Lista savuefekteistä.
    private List<ParticleSystem> smokeEffects = new List<ParticleSystem>();

    // Apumuuttuja, joka ilmaisee kaasutetaanko.
    private bool accelerating;
    // Apumuuttuja, joka määrittää luetaanko käyttäjän syötteitä.
    private bool allowInput;

    // Use this for initialization
    private void Start ()
    {
        // Haetaan viittaus peli- ja käyttöliittymämanageriin, autoluokkaan sekä pelaajan AudioSource-komponenttiin.
        GameObject obj = GameObject.FindGameObjectWithTag("GameManager");
        gameManager = obj.GetComponent<RCG_GameManager>();
        uiManager = obj.GetComponent<RCG_UIManager>();
        carController = GetComponent<RCG_CarController>();
        audioSource = GetComponent<AudioSource>();

        // Lisätään kaikki löydetyt savuefektit listaan.
        for (int i = 0; i < smokeEffectsRoot.childCount; i++)
        {
            smokeEffects.Add(smokeEffectsRoot.GetChild(i).gameObject.GetComponent<ParticleSystem>());
        }

        // Alustetaan apumuuttujat.
        accelerating = false;
        allowInput = true;
    }
	
	// Update is called once per frame
	private void Update ()
    {
        // Jos painetaan R-näppäintä ja autoa ei olla jo palauttamassa ja käyttäjän syötteitä luetaan, aloitetaan auton palauttaminen aloituspisteeseen.
        if (Input.GetKeyDown(KeyCode.R) && !carController.GetResetPosition() && allowInput)
        {
            carController.Respawn(gameManager.GetResetPosition());
        }

        // Määritetään kaasutetaanko, vai ei käyttäjän syötteen, sekä sen, luetaanko syötteitä, mukaan.
        if (Input.GetAxis("Vertical") == 0.0f || !allowInput)
        {
            accelerating = false;
        }
        else
        {
            accelerating = true;
        }

        // Muutetaan savuefektejä kaasuttamisen mukaan.
        switch(state)
        {
            case VehicleState.idle:
                if(accelerating)
                {
                    state = VehicleState.accelerating;
                    ModifySmokeEffects(1.0f, 1.0f);
                }
                break;
            case VehicleState.accelerating:
                if (!accelerating)
                {
                    state = VehicleState.idle;
                    ModifySmokeEffects(4.0f, 0.5f);
                }
                break;
        }

        // Päivitetään käyttöliittymän nopeusmittarin lukema.
        uiManager.UpdateSpeedometer(carController.GetCurrentSpeedNormalized());
        // Muutetaan moottorin ääntä ajonopeuden mukaan.
        audioSource.pitch = 0.5f + (carController.GetCurrentSpeedNormalized() / 2.0f);
	}

    private void OnTriggerEnter(Collider other)
    {
        // Jos kappale johon törmätään on väliaikapiste ja peli on päättynyt, lopetetaan käyttäjän syötteiden lukeminen.
        if(other.GetComponent<RCG_Checkpoint>() != null)
        {
            if(gameManager.isGameOver())
            {
                allowInput = false;
                carController.SetAllowInput(false);
            }
        }
    }

    private void ModifySmokeEffects(float multiplier, float colorAlpha)
    {
        foreach (ParticleSystem effect in smokeEffects)
        {
            ParticleSystem.MainModule mainModule = effect.main;
            ParticleSystem.EmissionModule emissionModule = effect.emission;

            mainModule.startLifetime = 0.5f * multiplier;
            mainModule.startSpeed = 88.0f / multiplier;
            mainModule.startSize = 28.8f / multiplier;
            mainModule.startColor = new Color(0.16f, 0.16f, 0.16f, colorAlpha);
            emissionModule.rateOverTime = 86.08f / multiplier;
        }
    }
}
