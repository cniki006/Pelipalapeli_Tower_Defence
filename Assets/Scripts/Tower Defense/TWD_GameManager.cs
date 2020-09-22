using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class TWD_GameManager : MonoBehaviour
{
    // Pelin tilat.
    // Aloitustila, pelitila (vihollisaallot), lopetustila.
    private enum GameState { start, waves, end }
    // Pelin tila.
    private GameState state;

    //Viittaus reitteihiin
    //Viittaus A-reittiin
    [SerializeField] private TWD_Path pathA;

    // Viittaukset vihollisten prefabeihiin
    // Viittaus A-vihollisen prefabiin
    [SerializeField] private GameObject enemyAPrefab;

    //Aikaväli, jonka välein vihollisia luodaan
    [SerializeField] private Timer spawnRate;
    //Lista elossa olevista vihollisista
    private GameObjectGroup liveEnemies;

    // Vihollisaalto-dataluokka
    [System.Serializable]
    private class TWD_Wave
    {
        // Vihollisaalto on merkkijono, josta voidaan lukua vihollismäärän ja tyyppit
        // Vihollisaallon merkkijono A-reitille
        [SerializeField] private string PathAWave;
        public string GetPathAWave() { return PathAWave; }
    }

    // Lista vihollisaalloista 
    [SerializeField] private List<TWD_Wave> waves;

    // Pelaajan elämäpisteet.
    [SerializeField] private Resource health;
    // Pelin pisteet.
    private Resource score;
    // Pelin aloituspisteiden määrä.
    [SerializeField] private float startScore;

    // Aika.
    // Kuluneen peliajan sekunnit.
    private float seconds;
    // Kuluneen peliajan minuutit.
    private float minutes;

    // Tämänhetkinen vihollisaalto.
    private int currentWaveIndex;
    // Tämänhetkisen vihollisen indeksi.
    private int currentEnemyIndex;

    // Apumuuttujat vihollisaaltojen käsittelyyn.
    // Apumuuttuja, joka ilmaisee onko vihollisaalto alkanut.
    private bool haveWavesStarted;
    // Apumuuttuja, joka ilmaisee onko vihollisaallon luominen päättynyt.
    private bool isSpawningFinished = false;
    // Apumuuttuja, joka ilmaisee onko vihollisaallon luominen A-reitilla suoritettu loppuun.
    private bool isPathAFinished = false;
    // Apumuuttuja, joka ilmaisee onko peli päättynyt.
    private bool isGameOver;

    // Viittaus HUD-manageriin.
    [SerializeField] private TWD_HUDManager hudManager;


    public Resource GetScore() { return score; }
    public bool GetIsGameOver() { return isGameOver; }
    /// <summary>
    /// <para> Peliajan asettaminen tiettyyn aikaan. </para>
    /// </summary>
    /// <param name="min">Asetettava minuuttimäärä.</param>
    /// <param name="sec">Asetettava sekuntimäärä.</param>
    private void SetGameTime(float min, float sec)
    {
        minutes = min;
        seconds = sec;
    }

    // Use this for initialization
    void Start()
    {
        // Alustetaan reittien pituudet
        pathA.SetPathLenght();

        // Alustetaan pelaajan elämäpisteet.
        health.SetValue(health.GetMax());
        // Asetetaan pisteille minimiarvo 0 ja maksimiarvo 9 999 999.
        score = new Resource(0, 9999999);
        // Asetetaan aloituspistemäärä.
        score.SetValue(startScore);

        // Alustetaan vihollislista
        liveEnemies = new GameObjectGroup();

        // Alustetaan pelin päättymisen apumuuttuja.
        isGameOver = false;

        // Alustetaan ensimmäisen vihollisaallon viive.
        SetGameTime(0.0f, 10.0f);
    }

    // Update is called once per frame
    void Update()
    {
        // Toimitaan pelin tilan mukaan.
        switch (state)
        {
            // Aloitustila.
            case GameState.start:
                // Lasketaan ajastinta kohti nollaa.
                CountGameTime(-Time.deltaTime);

                // Kun pelin aloitusajan laskenta saavuttaa arvon "0:00", aloitetaan vihollisaaltojen luomisprosessi.
                if (minutes <= 0.0f && seconds <= 0.0f)
                {
                    SetGameTime(0.0f, 0.0f);
                    NewWave();
                }
                break;

            // Pelitila.
            case GameState.waves:
                // Lasketaan peliaikaa.
                CountGameTime(Time.deltaTime);
                // Poistetaan vihollislistasta tyhjät jäsenet
                liveEnemies.CleanUp();

                // Jos elämäpisteet laskevat nollaan, lopetetaan peli.
                if (health.GetValue() <= health.GetMin())
                {
                    // Lopetetaan peli pelaajan häviöön.
                    EndGame(false);
                }
                // Jos elämäpisteitä on jäljellä.
                else
                {
                    // Jos tämänhetkinen aalto on käsitelty loppuun ja vihollisia ei ole elossa
                    if (isSpawningFinished && liveEnemies.IsEmpty())
                    {
                        // Jos seuravaa vihollisaaltoa ei ole eli tämä aalto on viimeinen
                        if (currentWaveIndex + 1 >= waves.Count)
                        {
                            // Lopetetaan peli pelajaan voittoon
                            EndGame(true);
                        }
                        // Kun seurava vihollisaalto löytyy
                        else
                        {
                            // Aloitetaan uusi vihollisaalto
                            NewWave();
                        }
                    }
                    // Jos tämänhetkisen aallon luomista ei ole käsitelty loppuun 
                    else if (!isSpawningFinished)
                    {
                        // Kun vihollisen luomisajastin on maksimissa, luodaan vihollinen ja nollataan ajastin
                        if (spawnRate.IsFinished())
                        {
                            ReadWave();
                            spawnRate.Reset();
                        }
                        // Kun vihollisen luomisajastin ei ole maksimissa, lasketaan se eteenpäin
                        else
                        {
                            spawnRate.Count();
                        }
                    }
                }
                break;

            // Lopetustila.
            case GameState.end:
                // Ladataan pelikenttä uudelleen pelaajan inputista.
                if (Input.anyKeyDown)
                {
                    SceneManager.LoadScene("Scene");
                }
                break;

            default:
                Debug.LogError("PPP Error: GameState not set!");
                break;
        }
    }

    private void LateUpdate()
    {
        // Päivitetään HUD:n elämäpiste-, kello- ja pistetekstikinttiä
        hudManager.SetHealth((int)health.GetValue());
        hudManager.SetClock((int)minutes, (int)seconds);
        hudManager.SetScore((int)score.GetValue());

        // Kun peli on aloitustilassa, muutetaan vihollisaaltotekstikentän tekstiä.
        if (state == GameState.start)
        {
            hudManager.SetWave("Prepare yourself!");
        }
        // Kun peli on peli- tai lopetustilassa, näytetään aallon numero vihollisaaltotekstikentässä.
        else
        {
            hudManager.SetWave(currentWaveIndex + 1);
        }
    }
    // Uuden vihollisaallon aloittaminen.
    private void NewWave()
    {
        // Jos peli on aloitustilassa.
        if (state == GameState.start)
        {
            // Asetetaan peli pelitilaan.
            state = GameState.waves;
            // Aloitetaan vihollisaaltojen käsittely.
            currentWaveIndex = 0;
        }
        // Kun pelin tila ei ole aloitustila.
        else
        {
            // Kasvatetaan käsittelyssä olevan vihollisaallon indeksiä.
            currentWaveIndex = currentWaveIndex + 1;
        }

        // Valmistellaan apumuuttujat seuraavaan vihollisaaltoon siirtymistä varten:
        // Nollataan vihollisten luontijonon indeksi.
        currentEnemyIndex = 0;
        // Julistetaan vihollisten luominen alkaneeksi.
        isSpawningFinished = false;
        // Asetetaan luomisajastin maksimiin
        spawnRate.SetCurrent(spawnRate.GetMax());
    }

    // Pelin päättyminen.
    private void EndGame(bool gameWon)
    {
        // Lopetetaan peli:
        // Muutetaan pelin tila lopetustilaan.
        state = GameState.end;
        isGameOver = true;

        // Aktivoidaan pelin lopetusnäkymä voiton tai häviön mukaan.
        hudManager.ActivateGameOverOverlay(gameWon);
    }

    // Vihollisaallon käsittely.
    private void ReadWave()
    {
        TWD_Wave currentWave = waves[currentWaveIndex];
        char enemyID;

        //A-reitin vihollisen luominen 
        //Jos a-reitin vihollisen luomista ei ole käsitelty loppuun
        if (!isPathAFinished)
        {
            // Käsitellään A-reitin vihollismerkkijono ellei sitä ole käsitelty loppuun
            if (currentEnemyIndex < currentWave.GetPathAWave().Length)
            {
                // Otetaan talteen merkkijonon merkki indeksin kohdalta ja luodaan vihollinen sen pohjalta
                enemyID = currentWave.GetPathAWave()[currentEnemyIndex];
                SpawnEnemy(pathA, enemyID);
            }
            // Kun A-reitin merkkijonossa ei ole enään vihollisia käsiteltäväksi, reitti A on käsitetly loppuun tällä aallolla.
            else
            {
                isPathAFinished = true;
            }
        
        }
        // Kasvatetaan merkkijonon käsiteltävä indeksia vastamaan seuravaa indeksia
        currentEnemyIndex = currentEnemyIndex + 1;

        // Jos kaikki merkkijonot on käyty loppuun voidaan siirtyä seuravaan aaltoon
        if (isPathAFinished)
        {
            //Vihollisen luominen tällä aallolla on suoritettu
            isSpawningFinished = true;

            //Resetoidaan apumuuttajat 
            isPathAFinished = false;
        }
    }

    // Vihollisen luominen.
    private void SpawnEnemy(TWD_Path path, char enemyID)
    {
        GameObject enemyPrefab = null;
        Vector3 pathBeginning = new Vector3();

        //Asetetaan reitin ensimmäinen piste aloituspisteeksi
        pathBeginning = path.GetAtIndex(0).transform.position;

        //Valitaan vihollisen malli merkin perusteella
        switch (enemyID)
        {
            case 'A':
                // Asetaan vihollismalliksi A-merkkiä vastava vihollinen
                enemyPrefab = enemyAPrefab;
                break;

            default:
                break;
        }

        // Jos vihollisen malli ei ole tyhjä, eli vihollinen pystyttiin määrittelemään merkin avulla.
        if (enemyPrefab != null)
        {
            GameObject newEnemy;
            //Luodaan vihollinen vihollismallin pohjalta ja asetetaan se reitin alkuun
            newEnemy = Instantiate(enemyPrefab, pathBeginning, Quaternion.identity);

            //Asetetaan viholliselle reittitieto
            newEnemy.GetComponent<TWD_Enemy>().SetPath(path);
            //Asetetaan viholliselle viittaus pelimanageriin
            newEnemy.GetComponent<TWD_Enemy>().SetGameManager(this.GetComponent<TWD_GameManager>());

            //Lisätään luotu vihollinen elävien vihollisten listalle
            liveEnemies.Add(newEnemy);
        }
    }

    // Vähennetään elämäpisteitä.
    public void ReduceHealth(float value)
    {
        // Vähennetään elämäpisteitä annetun arvon verran.
        health.Modify(-value);

    }

    // Ajan laskeminen.
    private void CountGameTime(float value)
    {
        // Ajan muutos.
        seconds = seconds + value;

        // Jos ajan muutosarvo on positiivista, tarkkaillaan sekuntien ja minuuttien kasvua.
        if (value > 0.0f)
        {
            if (seconds >= 60.0f)
            {
                minutes = minutes + 1;
                seconds = seconds - 60.0f;
                if (minutes >= 60.0f)
                {
                    minutes = 0.0f;
                }
            }
        }
        // Jos ajan muutosarvo on negatiivista, tarkkaillaan sekuntien ja minuuttien vähenemistä.
        else if (value < 0.0f)
        {
            if (seconds < 0.0f && minutes > 0.0f)
            {
                minutes = minutes - 1;
                seconds = 59.0f;
            }
        }
    }
}
