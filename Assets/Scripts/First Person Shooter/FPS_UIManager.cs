using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FPS_UIManager : MonoBehaviour {

    // Viittaus pelaajan pistetekstikenttään.
    [SerializeField] private Text scoreText;
    // Viittaus pelaajan ammustekstikenttään.
    [SerializeField] private Text ammoText;
    // Viittaus pelimanageriin.
    private FPS_GameManager gameManager;

    // Viittaus pelin lopetusruutuun.
    [SerializeField] private GameObject endScreen;
    // Viittaus lopetusruudun tekstikenttään.
    [SerializeField] private Text endScreenText;

    private void Start() {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<FPS_GameManager>();
    }

    private void Update () {
        // Päivitetään piste- ja ammustekstikenttiä.
        scoreText.text = string.Format("Score: {0:0000}", gameManager.GetGameScore());
        ammoText.text = string.Format("Ammo: {0:0}/{1:0}", gameManager.GetPlayerAmmo().GetValue(), gameManager.GetPlayerAmmo().GetMax());
    }

    // Lopetusruudun aktivointi.
    public void InitEndScreen() {
        // Enabloidaan lopetusruutu ja näytetään viimeisin pistemäärä.
        endScreen.SetActive(true);
        endScreenText.text = string.Format("You scored: {0:0000}", gameManager.GetGameScore());
    }
}
