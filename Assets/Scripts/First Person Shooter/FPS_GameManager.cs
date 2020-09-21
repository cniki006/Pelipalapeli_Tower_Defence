using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FPS_GameManager : MonoBehaviour {

    // Pelin tilat.
    private enum GameState { running, finished }
    // Pelin tila.
    private GameState state;

    // Viittaus audiomanageriin.
    [SerializeField] private AudioManager audioManager;
    // Viittaus UI manageriin.
    [SerializeField] private FPS_UIManager UIManager;

    // Pelin pistemäärä.
    private float gameScore;
    // Viitaus pelaajan ammusten määrään.
    private Resource playerAmmo = new Resource(0, 0);

    public void SetGameScore(float s) { gameScore = s; }
    public float GetGameScore() { return gameScore; }
    public void IncreaseGameScore(float s) { gameScore += s; }

    /// <summary>
    /// <para> Asetetaan pelin tila annetun kokonaisluvun mukaan. </para>
    /// <para> 0 käynnistää pelin, 1 päättää pelin. Muut annetut arvot oletusarvoisesti valitsevat käynnissä olemisen jatkamisen. </para>
    /// </summary>
    public void SetState(int i) {
        switch(i) {
            case 0:
              state = GameState.running;
              break;
            case 1:
                state = GameState.finished;
                break;
            default:
                state = GameState.running;
                break;
        }
    }

    public void SetPlayerAmmo(float min, float max, float value) {
        playerAmmo = new Resource(min, max);
        playerAmmo.SetValue(value);
    }
    public Resource GetPlayerAmmo() { return playerAmmo; }

    public AudioManager GetAudioManager() { return audioManager; }

    public FPS_UIManager GetUIManager() { return UIManager; }

    public bool IsGameRunning() {
        if (state == GameState.running)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Start() {
        // Alustetaan peli käynnissä olevaksi.
        state = GameState.running;
    }

    private void Update() {
        // Kun peli on päättynyt ja pelaaja painaa Space-näppäintä, uudelleenladataan pelikenttä.
        if (state == GameState.finished && Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("First Person Shooter");
        }
    }
}
