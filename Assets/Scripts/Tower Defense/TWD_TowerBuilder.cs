using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TWD_TowerBuilder : MonoBehaviour
{

    // Tornien rakentamistyökalun tilat.
    // Rakentamistila ja normaalitila.
    private enum BuilderState { build, normal }
    // Rakentamistyökalun tila.
    private BuilderState state;

    // Layermask, jolla määritellään ne objektit, joihin raycast-säde voi osua.
    private int rayCatcherMask;
    // Kamerasta lähtevän säteen pituus.
    private float camRayLength = 1000.0f;

    // Tornin rakentamisindikaattori.
    [SerializeField] private GameObject buildIndicator;
    // Materiaali indikaattoorille: rakentamisen estävä väri.
    [SerializeField] private Material buildObstructedMaterial;
    // Materiaali indikaattorille: rakentamisen salliva väri.
    [SerializeField] private Material buildUnobstructedMaterial;

    // Lista peliobjekteista, jotka ovat indikaattorin kanssa päällekkäisesti samassa tilassa.
    private GameObjectGroup restrictingObjects;
    // Rakennettava torni.
    private GameObject selectedTower;
    // Tornien prefabit.
    // Viittaus tornin A malliin.
    [SerializeField] private GameObject towerAPrefab;

    // Projektiilitietokanta.
    private ObjectPooler selectedPooler;
    // Viittaukset projektiilitietokantoihin.
    // Viittaus A-projektiilitietokantaan.
    [SerializeField] private ObjectPooler projectileAPooler;

    // Apumuuttuja rakentamisen sallimiseen ja estämiseen.
    private bool canBuild;
    // Apumuuttuja rakentamisen onnistumisen ja epäonnistumisen tarkasteluun.
    private bool failedToBuild;

    // Viittaus pelimanageriin.
    [SerializeField] private TWD_GameManager gameManager;
    // Viittaus HUD-manageriin.
    [SerializeField] private TWD_HUDManager hudManager;
    // Viittaukset tornien rakentamispainikkeisiin HUDissa.
    // Viittaus A-tornin rakentamispanikkeeseen.
    [SerializeField] private Button towerAButton;

    public void TowerAButtonClicked() { EnterBuildState(towerAPrefab, projectileAPooler); }

    private void SetIndicatorMaterial(Material newMaterial)
    {
        // Jos rakentamisindikaattorilla on Renderer-komponentti.
        if (buildIndicator.GetComponent<Renderer>().material != newMaterial)
        {
            // Jos materiaali ei jo ole asetettava meteriaali.
            if (buildIndicator.GetComponent<Renderer>().material != newMaterial)
            {
                // Asetetaan indikaattorin meriaaliksi annettu materiaali
                buildIndicator.GetComponent<Renderer>().material = newMaterial;
            }
        }
    }

    // Use this for initialization
    void Start()
    {
        restrictingObjects = new GameObjectGroup();
        selectedTower = null;

        // Asetetaan aluksi normaalitila.
        state = BuilderState.normal;
        // Disabloidaan rakentamisindikaattori.
        buildIndicator.SetActive(false);

        // Alustetaan apumuuttujat
        canBuild = true;
        failedToBuild = false;

        // Haetaan viittaus layermaskiin.
        rayCatcherMask = LayerMask.GetMask("RaycastTarget");

        // Alustetaan tornien infotekstit.
        InitializeTowerTexts();
    }

    // Update is called once per frame
    void Update()
    {
        // Tarkastetaan pelimanagerista, onko peli vielä päättynyt:
        // Jos rakentaminen on sallittu ja peli on päättynyt.
        if (canBuild && gameManager.GetIsGameOver())
        {
            // Estetään rakentaminen.
            canBuild = false;
            // Jos ollaan rakennustilassa, päätetään rakentaminen.
            if (state == BuilderState.build)
            {
                ExitBuildState();
            }
        }
        // Jos rakentaminen sallitaan.
        else if (canBuild)
        {
            // Päivitetään listaa niistä peliobjekteista, joihin indikaattori on törmänneenä.
            restrictingObjects.CleanUp();

            // Jos ollaan rakennustilassa.
            if (state == BuilderState.build)
            {
                // Jos hiiren oikeaa painiketta painetaan.
                if (Input.GetMouseButtonDown(1))
                {
                    // Perutaan rakentaminen ja poistutaan rakentamistilasta.
                    ExitBuildState();
                }
                // Kun hiiren oikeaa painiketta ei paineta, eli pysytään rakennustilassa.
                else
                {

                    // Paikkavektori.
                    Vector3 location = new Vector3();

                    // Ammutaan säde kamerasta hiiren kursorin sijaintiin.
                    Ray camRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit hitEvent;
                    // Jos säde osuu, muodostetaan paikkavektori rakentamiselle osumakohdan perusteella.
                    if (Physics.Raycast(camRay, out hitEvent, camRayLength, rayCatcherMask))
                    {
                        // Paikkavektorin muodostaminen.
                        location = hitEvent.point;
                        location.y = 0.0f;

                        // Siirretään rakentamisindikaattori paikkavektorin sijaintiin.
                        this.transform.position = location;
                    }

                    // Jos rakentamista estäviä törmäyksiä ei ole.
                    if (restrictingObjects.IsEmpty())
                    {
                        // kUn hiiren vasenta painiketta painetaan, eli aiotaan rakentaa torni.
                        if (Input.GetMouseButtonDown(0))
                        {
                            // Jos kursori ei ole minkään painikkeen päällä.
                            if (!IsMouseOverButtons())
                            {
                                if (selectedTower != null)
                                {
                                    // Tarkistetaan, maksaako torni vähemmän tai yhtä paljon kuin pelaajalla on pisteitä.
                                    if (selectedTower.GetComponent<TWD_Tower>().GetCost() <= gameManager.GetScore().GetValue())
                                    {
                                        //Rakennetaan torni.
                                        BuildTower(location);

                                    }
                                    // Jos pisteitä ei ole riittävästi tornin rakentamista varten.
                                    else
                                    {
                                        // Asetaan indikaattorin materiaaliksi rakentamisen estävä materiaali.
                                        SetIndicatorMaterial(buildObstructedMaterial);
                                        // Rakentamisyritys epäonnistui.
                                        failedToBuild = true;
                                    }
                                }
                            }
                        }
                        // Kun hiiren vasen painike nostetaan ja aikaisempi rankentamisyritys epäonnistui
                        else if (Input.GetMouseButtonUp(0) && failedToBuild)
                        {
                            // Asetetaan indikaattorin materiaaliksi rakentamisen salliva materiaali.
                            SetIndicatorMaterial(buildUnobstructedMaterial);
                            failedToBuild = false;
                        }
                        // Kun hiiren vasenta painiketta ei paineta tai nosteta, ja aikaisempi rakentamisyritys ei ole epäonnistunut
                        else if (!failedToBuild)
                        {
                            // Asetetaan indikaattorin materiaaliksi rakentamisen salliva materiaali
                            SetIndicatorMaterial(buildUnobstructedMaterial);
                        }
                    }
                    // Kun rakentaminen on estetty.
                    else
                    {
                        // Asetetaan indikaattorin mataeriaaliksi rakentamisen estävä materiaali.
                        SetIndicatorMaterial(buildObstructedMaterial);
                    }
                }
            }
        }
    }

    // Kun rakentamisindikaattori törmää toiseen peliobjektiin.
    private void OnTriggerEnter(Collider other)
    {
        // Katsotaan onko toisen peliobjektin tagi "Trigger".
        if (other.CompareTag("Trigger"))
        {
            // Jos ollaan rakennustilassa, lisätään peliobjekti listaan törmäämisen alussa.
            if (state == BuilderState.build)
            {
                restrictingObjects.Add(other.transform.root.gameObject);
            }
        }
    }

    // Kun törmäys rakentamisindikaattorin ja toisen peliobjektin välillä päättyy.
    private void OnTriggerExit(Collider other)
    {
        // Katsotaan onko toisen peliobjektin tagi "Trigger".
        if (other.CompareTag("Trigger"))
        {
            // Poistetaan  peliobjekti listasta, kun törmääminen sen kanssa päättyy.
            restrictingObjects.Remove(other.transform.root.gameObject);
        }
    }

    // Rakentamistilaan siirtyminen.
    private void EnterBuildState(GameObject tower, ObjectPooler pooler)
    {
        // Rakennustilan alussa asetetaan pelitila rakennustilaksi ja enabloidaan rakentamisindikaattori.
        state = BuilderState.build;
        buildIndicator.SetActive(true);
        // Rakennettava torni.
        selectedTower = tower;
        // Tornin ammustietokanta.
        selectedPooler = pooler;
    }

    // Rakentamistilasta poistuminen.
    private void ExitBuildState()
    {
        // Rakennustilan lopussa asetetaan pelitila normaaliksi ja nollataan rakentamisindikaattorin sijainti.
        state = BuilderState.normal;
        this.transform.position = Vector3.zero;

        // Tyhjennetään rakentamisen estävien peliobjektien lista, jotta ne eivät jää estämään rakentamista seuraavalla kerralla.
        restrictingObjects.Clear();
        // Disabloidaan rakentamisindikaattori.
        buildIndicator.SetActive(false);
    }
    //Tornin rakentaminen
    private void BuildTower(Vector3 pos)
    {
        // Muutetaan pelin pisteitä rakennettavan tornin hinnan verran.
        gameManager.GetScore().Modify(-selectedTower.GetComponent<TWD_Tower>().GetCost());

        // Luodaan uusi torni.
        GameObject newTower;
        // Asetetaan uudelle tornille prefab, aloitussijainti ja kierto.
        newTower = Instantiate(selectedTower, pos, Quaternion.identity);

        // Asetetaan tornille viittaus projektiilitietokantaan.
        newTower.GetComponent<TWD_Tower>().SetProjectilePooler(selectedPooler);
        // Asetetaan tornille viittaus pelimanageriin
        newTower.GetComponent<TWD_Tower>().SetGameManager(gameManager);
    }

    // Tornien ingotekstien alustus.
    private void InitializeTowerTexts()
    {
        // Asetetaan A-tornin infoteksti.
        hudManager.SetTowerText(hudManager.GetTowerAText(), towerAPrefab.GetComponent<TWD_Tower>());
    }

    // Tarkastetaan, onko hiiren kursori tornien rakentamispainikkeiden päällä.
    private bool IsMouseOverButtons()
    {
        // Tarkistetaan, onko hiiren kursori A-tornin painikkeen päällä.
        if (towerAButton.GetComponent<TWD_Button>() != null)
        {
            if (towerAButton.GetComponent<TWD_Button>().GetIsHovered())
            {
                return true;
            }
        }

        // Jos hiiren kursori ei ole minkään tornin painikkeen päällä.
        return false;
    }
}
