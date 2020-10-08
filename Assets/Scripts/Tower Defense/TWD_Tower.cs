using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TWD_Tower : MonoBehaviour
{
    private int sTimer;
    public bool sCheck = false;

    //Kerralla ammuttavien projektiilien määrä
    [SerializeField] private int projectileCount;
    //Tornin vahinkoarvo
    [SerializeField] private float damage;
    //Tornin tulinopeus
    [SerializeField] private Timer fireRate;
    //Tornin rakentamishinta
    [SerializeField] private float cost;

    //Havaitsemisaluen säde
    [SerializeField] private float range;
    // Viitaus havaitsemisalueeseen
    [SerializeField] private SphereCollider detectionArea;

    //Kääntykö torni tähtäämän kohdetta vai ei
    [SerializeField] private bool aimAtTarget;
    //Listaus tornin kohteista
    private GameObjectGroup targets;
    //Tornin tämänhetkinen kohde
    private GameObject currentTarget;

    //Viittaus projektiilien peliobjektitietokantaan.
    [SerializeField] private ObjectPooler projectilePooler;
    //Viittaus pelimanageeriin
    private TWD_GameManager gameManager;


    public float GetCost() { return cost; }
    public float GetDamage() { return damage; }
    public float GetFireRate() { return fireRate.GetMax(); }
    public float GetRange() { return range; }
    public void SetProjectilePooler(ObjectPooler pooler) { projectilePooler = pooler; }
    public void SetGameManager(TWD_GameManager newManager) { gameManager = newManager; }



    // Start is called before the first frame update
    void Start()
    {
        //Alustetaan kohdelista
        targets = new GameObjectGroup();

        //Asetetaan havaitsemisalueen koko havaitsemisalueen mukaan
        detectionArea.radius = range;
    }

    // Update is called once per frame
    void Update()
    {
        //shootCheck = true;
        //sTimer--;
        //if (sTimer == 0) shootCheck = false;
        // Jos peli ei ole vielä päättynyt suoritetaan tornin toiminta
        if (!gameManager.GetIsGameOver())
        {

            // Lasketaan tulinopeuden ajastinta eteenpäin.
            fireRate.Count();
            // Päivitetään kohdelista
            targets.CleanUp(true);

            // Kun kohdelista ei ole tyhjä
            if (!targets.IsEmpty())

            {
                currentTarget = targets.GetAtIndex(0);


                // Asetetaan reitillään pisimmälle edennyt vihollinen tämänhetkiseksi kohteeksi
                for (int i = 0; i > targets.GetSize(); i++)
                {
                    if (targets.GetAtIndex(i).GetComponent<TWD_Enemy>().GetProgressPercentage() > currentTarget.
                        GetComponent<TWD_Enemy>().GetProgressPercentage())
                    {
                        currentTarget = targets.GetAtIndex(i);
                    }
                }

                //Kun tornilla on kohde
                if (currentTarget != null)
                {
                    // Jos torni on asetettu kääntymään ja tähtämään kohdettaan
                    if (aimAtTarget)
                    {
                        //Tornin kääntäminen kohti kohdetta:
                        //Suuntavektori tornin ja kohteen välillä
                        Vector3 targetDir = (currentTarget.transform.position - this.transform.position).normalized;
                        //Käännetään tornia kohteen suuntaan z-akselin ympärillä
                        float angle = Mathf.Atan2(targetDir.x, targetDir.z) * Mathf.Rad2Deg;
                        this.transform.rotation = Quaternion.Euler(0, angle, 0);
                    }

                    //Ampuu kun tulinopeuden ajastin on maksimissa
                    if (fireRate.IsFinished())
                    {
                        //shootCheck = true;
                        Shoot();
                        //shootCheck = true;
                        // Nollataan tulinopeuden ajastin
                        fireRate.Reset();
                    }
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //Kun tornin kantaman sisälle saapuu vihollinen, lisätään se kohdelistaan
        if (other.transform.root.gameObject.CompareTag("Hostile"))
        {
            targets.Add(other.transform.root.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Kun tornin kantamaan sisältä poistuu vihollinen poistetaan se kohdelistasta
        if (other.transform.root.gameObject.CompareTag("Hostile"))
        {
            targets.Remove(other.transform.root.gameObject);
        }
    }

    // Tornin ampuminen 
    private void Shoot()
    {
        sTimer = 1000;
        //sCheck = true;
        //shootCheck = true;

        //Luodaan niin monta projektiilia kuin torni ampuu kerralla
        for (int i = 0; i < projectileCount; i++)
        {
            //Lisäkierto joka asetetaan eri projektiileille
            Vector3 additionalRotation = new Vector3(0, (360 / projectileCount * i), 0);
            //Haetaan tietokannasta uusi projektiili
            GameObject newProjectile = projectilePooler.GetPooledObject();

            //Jos projektiili ei ole tyhjä , eli se saatin haettua tietokannasta
            if (newProjectile != null)
            {
                //Asetetaan projektiilille aloitussijainti ja kierto.
                newProjectile.transform.position = this.transform.position;
                newProjectile.transform.rotation = this.transform.rotation * Quaternion.Euler(additionalRotation);
                //Enabloidaan projektiili
                newProjectile.SetActive(true);

                //Asetetaan tornin vahinkoarvo ammuksen vahinkoarvoksi.
                newProjectile.GetComponent<TWD_Projectile>().SetDamage(damage);
                //Lasketaan projektiilin elinaika projektiilin nopeuden ja tornin havaitsemisalueen avulla
                newProjectile.GetComponent<TWD_Projectile>().SetLifetime(range / newProjectile.GetComponent<TWD_Projectile>().GetVelocity());
                //shootCheck = false;
            }
        }

    }
}
