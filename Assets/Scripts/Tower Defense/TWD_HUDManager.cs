using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class TWD_HUDManager : MonoBehaviour
{
    // Viittaus elämäpisteiden tekstikenttään.
    [SerializeField] private Text healthText;
    // Viittaus kellon tekstikenttään.
    [SerializeField] private Text timeText;
    // Viittaus vihollisaallon tekstikenttään.
    [SerializeField] private Text waveText;
    // Viittaus pelin pisteiden tekstikenttään.
    [SerializeField] private Text scoreText;

    // Tornien infotekstit.
    // Viittaus A-tornin infotekstikenttään.
    [SerializeField] private Text towerAText;
    // Viittaus pelin lopetusnäkymään.
    [SerializeField] private GameObject gameOverOverlay;
    // Viittaus voittotekstikenttään.
    [SerializeField] private Text victoryText;
    // Viittaus pelin häviötekstikenttään.
    [SerializeField] private Text defeatText;

    public Text GetTowerAText() { return towerAText; }

    public void SetHealth(int value) { healthText.text = System.String.Format("{0}: {1}", "Health", value); }
    public void SetClock (int min, int sec) { timeText.text = System.String.Format("{0:00}:{1:00}", min, sec); }
    public void SetWave (int value) { waveText.text = System.String.Format("{0}: {1}", "Wave", value); }
    public void SetWave (string text) { waveText.text = text; }
    public void SetScore (int value) { scoreText.text = System.String.Format("{0}: {1}", "Score", value); }

    public void SetTowerText (Text towerText, TWD_Tower tower)
    {
            towerText.text = System.String.Format("{0}: \t\t\t{1}\n{2}: \t\t{3}\n{4}: \t{5}\n{6}: \t\t{7}", "Cost", tower.
            GetCost(), "Damage", tower.GetDamage(), "Fire Rate", tower.GetFireRate(), "Range", tower.GetRange());
    }

    // Pelin lopetusnäkymään aktivointi.
    public void ActivateGameOverOverlay (bool gameWon)
    {
        // Enabloidaan lopetus-näkymä.
        gameOverOverlay.SetActive(true);

        // Jos peli voitettiin.
        if (gameWon)
        {
            // Disabloidaan Defeat-lopetusteksti.
            defeatText.gameObject.SetActive(false);
        }
        // Kun peli hävittiin.
        else
        {
            // Disabloidaan Victory-lopetusteksti.
            victoryText.gameObject.SetActive(false);
        }
    }
}
