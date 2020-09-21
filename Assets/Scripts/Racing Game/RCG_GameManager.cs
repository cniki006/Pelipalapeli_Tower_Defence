using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RCG_GameManager : MonoBehaviour {

    // Pelin tilat.
    private enum GameState { waiting, game, over }

    // Pelin tila.
    private GameState state = GameState.waiting;

    // Viittaus käyttöliittymämanageriin.
    private RCG_UIManager uiManager;

    // Viittaus audiomanageriin.
    private AudioManager audioManager;
    // Viittaus ääniklippiin, joka toistetaan kun pelaajan auto kulkee väliaikapisteen lävitse.
    [SerializeField] private AudioClip checkpointPassSound;

    // Viittaus Väliaikapisteiden juuriobjektin Transform-komponenttiin.
    [SerializeField] private Transform checkpointsRoot;
    // Pelaajan auton aloitussijainti.
    [SerializeField] private Transform startPosition;
    // Lista kaikista radan väliaikapisteistä.
    private List<GameObject> checkpoints = new List<GameObject>();
    // Kohdeväliaikapisteen indeksi.
    private int targetCheckpointIndex;

    // Ajettavien kierrosten lukumäärä.
    [SerializeField] private int lapCount;
    // Kierros, jolla pelaaja on menossa.
    private int currentLap;

    // Pelin aloituskellonaika.
    private float startTime;

    // Pelin päättymisen jälkeisen viiveen ajastin.
    [SerializeField] private Timer raceEndTimer;

    // Apumuuttuja, joka ilmaisee onko pelin päättymisruutu aktivoitu.
    private bool gameOverScreenActivated;

    public Transform GetResetPosition()
    {
        if(targetCheckpointIndex - 1 < 0)
        {
            return startPosition;
        }
        else
        {
            return checkpoints[targetCheckpointIndex - 1].transform;
        }
    }

    // Use this for initialization
    void Start ()
    {
        // Haetaan viittaus käyttöliittymämanageriin sekä audiomanageriin.
        uiManager = GetComponent<RCG_UIManager>();
        audioManager = GameObject.FindGameObjectWithTag("AudioManager").GetComponent<AudioManager>();

        // Lisätään kaikki löydetyt väliaikapisteet listaan.
		for(int i = 0; i < checkpointsRoot.childCount; i++)
        {
            checkpoints.Add(checkpointsRoot.GetChild(i).gameObject);
        }

        // Alustetaan apumuuttujien arvot sekä käyttöliittymän elementit.
        targetCheckpointIndex = 0;
        currentLap = 1;
        uiManager.UpdateLapInfo(currentLap, lapCount);
        uiManager.SetFinishScreenActive(false);
        uiManager.SetGameOverScreenActive(false);
        gameOverScreenActivated = false;

        // Lukitaan kursori keskelle ruutua.
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update ()
    {
        switch (state)
        {
            case GameState.waiting:
                // Pysytään odotustilassa kunnes käyttäjä painaa välilyöntiä (tai vastaavaa näppäintä).
                if(Input.GetButtonDown("Jump"))
                {
                    state = GameState.game;
                    uiManager.SetWaitingScreenActive(false);
                    startTime = Time.time;
                }
                break;
            case GameState.game:
                // Lasketaan peliaikaa.
                uiManager.UpdateTimer(Time.time - startTime);
                break;
            case GameState.over:
                // Kun peli on päättynyt (pelaajan auto on päässyt maaliin asti), lasketaan pelin päättymisen viiveen ajastin loppuun
                // jonka jälkeen aktivoidaan lopetusruutu.
                if(raceEndTimer.IsFinished())
                {
                    if(!gameOverScreenActivated)
                    {
                        uiManager.SetHUDActive(false);
                        uiManager.SetFinishScreenActive(false);
                        uiManager.SetGameOverScreenActive(true);
                        uiManager.SetEndTimeText();
                        gameOverScreenActivated = true;
                    }
                    // Palataan päävalikkoon käyttäjän syötteestä.
                    if(Input.GetButtonDown("Jump"))
                    {
                        Cursor.lockState = CursorLockMode.None;
                        SceneManager.LoadScene("Scene");
                    }
                }
                else
                {
                    raceEndTimer.Count();
                }
                break;
        }
    }

    /// <summary>
    /// <para>Palauttaa tiedon, onko peli päättynyt.</para>
    /// </summary>
    /// <returns></returns>
    public bool isGameOver()
    {
        if(state == GameState.over)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// <para>Käsittelee väliaikapisteen läpiajon.</para>
    /// </summary>
    /// <param name="checkpoint"></param>
    public void PassThroughCheckpoint(GameObject checkpoint)
    {
        // Jos tarkasteltava väliaikapiste on sama kuin kohdeväliaikapiste, kasvatetaan kohdeväliaikapisteen indeksiä.
        if(GameObject.ReferenceEquals(checkpoints[targetCheckpointIndex], checkpoint))
        {
            targetCheckpointIndex = targetCheckpointIndex + 1;
            // Toistetaan ääniefekti.
            audioManager.PlaySound(checkpointPassSound, 0.25f);

            // Jos kohdeväliaikapisteen indeksi vastaa listan jäsenten määrää siirrytään seuraavalle kierrokselle, ellei maalia jo saavutettu.
            if(targetCheckpointIndex == checkpoints.Count)
            {
                if(currentLap == lapCount)
                {
                    // Maalin saavutettua, päätetään peli.
                    uiManager.SetFinishScreenActive(true);
                    state = GameState.over;
                }
                else
                {
                    // Uuden kierroksen alkaessa kasvatetaan kierroslaskuria ja nollataan kohdeväliaikapisteen indeksi.
                    currentLap = currentLap + 1;
                    uiManager.UpdateLapInfo(currentLap, lapCount);
                    targetCheckpointIndex = 0;
                }
            }
        }
    }

    /// <summary>
    /// <para>Välittää käyttöliittymämanagerille komennon kääntää väliaikapistesuuntanuolta osoittamaan kohti seuraavaa väliaikapistettä.</para>
    /// </summary>
    /// <param name="source"></param>
    public void RotateWaypointIndicator(Transform source)
    {
        uiManager.RotateWaypointIndicator(source, checkpoints[targetCheckpointIndex].transform);
    }
}
