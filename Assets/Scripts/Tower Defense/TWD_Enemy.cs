using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TWD_Enemy : MonoBehaviour
{

    // Elämäpisteet
    [SerializeField] private Resource health;
    //Viittaus elämäpistepalkin toimintalogiikkaan
    [SerializeField] private TWD_HealthBar healthBarScript;

    //Liikkumisnopeus
    [SerializeField] private float velocity;

    // Viittaus reittiin
    private TWD_Path path;
    // Viittaus animaation muuttoon
    public int animationDir=0;
    // Tämänhetkinen reittipiste
    private GameObject currentGoal;
    //Tämänhetkisen reittipisten indeksi
    private int currentGoalIndex;
    //Edistyminen reitillä
    private float currentProgress;
    //Prosentuaalinen edistyminen reitillä
    private float currentProgressPersentage;

    //Apumuuttujat reitillä liikkumisen avuksi
    //Edetäänkö reitillä
    private bool isOnPath = false;
    //Reitin lopun saavuttaminen
    private bool finishedPath = false;

    //Viittaus pelimanageriin
    private TWD_GameManager gameManager;

    //Vahinkoarvo 
    [SerializeField] private float damageToPlayer;
    //Tuhoamisesta palkittava pistemäärä
    [SerializeField] private int scoreReward;

    public float GetProgressPercentage() { return currentProgressPersentage; }
    public void SetGameManager(TWD_GameManager newManager) { gameManager = newManager; }
    public void SetPath (TWD_Path newPath)
    {
        //Asetetaan reitin ja kulkua mittavat muuttujat
        path = newPath;
        currentProgress = 0.0f;
        currentGoalIndex = 0;
        currentGoal = path.GetAtIndex(currentGoalIndex);
    }
    void Start()
    {
        //Alustetaan elämäpisteet
        health.SetValue(health.GetMax());    
    }

    // Update is called once per frame
    void Update()
    {
        //Jos peli ei ole vielä päättynyt, vihollinen suorittaa toimintansa
        if (!gameManager.GetIsGameOver())
        {
            //Asetaan vihollisen suunta kohti seuravaa pistettä
            Vector3 direction = (currentGoal.transform.position - this.transform.position).normalized;
            float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
            if (direction.x < 0 && direction.z < 0) animationDir = 1;
            this.transform.rotation = Quaternion.Euler(0, angle, 0);
            //Lasketaan liikkemäärä tällä fraimilla
            Vector3 movement = transform.forward * velocity * Time.deltaTime;

            //Jos vihollisella on reittipiste, jota kohtaan se etenee
            if (currentGoal != null)
            {
                //Jos saavutetaan tai ohitetaan kohde
                if (movement.magnitude >= (currentGoal.transform.position -this.transform.position).magnitude)
                {
                    //Jos reitin ensimmäinen piste saavutetaan, vihollinen on reitillä
                    if (currentGoalIndex == 0 && !isOnPath)
                    {
                        isOnPath = true;
                    }

                    //Kasvatetaan kohde-indeksia
                    currentGoalIndex = currentGoalIndex + 1;

                    //Jos saavutetaan reitin viimeinen reittipiste
                    if (currentGoalIndex >= path.GetSize())
                    {
                        finishedPath = true;
                        Destruction();
                    }
                    /* Kun reitin loppua ei ole vielä saavutettu, valitaan seuraava reittipiste kohdeksi kohde-indeksin mukaan*/
                    else
                    {
                        currentGoal = path.GetAtIndex(currentGoalIndex);
                    }
                }
            }

            //Kun vihollinen on reitillä lasketaan etenemistä
            if(isOnPath)
            {
                //Etenemisen yksikköarvo
                currentProgress = currentProgress + movement.magnitude;
                //Etenemisen %-arvo (kuljettu matka/koko matka)
                currentProgressPersentage = currentProgress / path.GetPathLenght();
            }

            //Liikutetaan peliobjektia eteenpäin
            this.transform.position = this.transform.position + movement;
        }
        
    }

    private void LateUpdate()
    {
        //Päivitetään elämäpistepalkin sijaintia ja asentoa pelimaailmassa
        healthBarScript.gameObject.transform.position = this.transform.position + new Vector3(0.0f, 2.0f, -0.75f);
        healthBarScript.gameObject.transform.rotation = Quaternion.Euler(90, 0, 0);
        //Jos elämäpisteet laskee minimiin, tuhotaan tämä peliobjekti
        if (health.GetValue() <= health.GetMin())
        {
            Destruction();
        }
    }

    // Kun peliobjekti törmää toiseen peliobjektiin
    private void OnTriggerEnter(Collider other)
    {
        //Katsotaan onko toisen objektin tagi "Hazard"
        if (other.CompareTag("Hazard"))
        {
            //Katsotaan onko toisella objektilla TWD_Projectile-scriptia
            if (other.GetComponentInParent<TWD_Projectile>() != null )
            {
                //Elämäpisteistä vähenetään toisen kappalen ilmoittama määrä 
                health.Modify(-other.GetComponentInParent<TWD_Projectile>().GetDamage());
            }
        }
        //Päivitetään elämäpistepalkin arvo
        healthBarScript.SetValue(health.GetValue() / health.GetMax());
    }

    //Peliobjektin tuhoutuminen
    private void Destruction()
    {
        // Jos vihollinen ei kulkenut koko reittiä loppuun ja peli on päättynyt
        if(!finishedPath && !gameManager.GetIsGameOver())
        {
            //Vihollinen antaa pisteitä 
            gameManager.GetScore().Modify(scoreReward);
        }
        //Jos vihollinen kulki koko reitin läpi
        else if (finishedPath)
        {
            //Vähennetään pelaajan elämäpisteitä
            gameManager.ReduceHealth(damageToPlayer);
        }

        Destroy(this.gameObject);
    }

}
