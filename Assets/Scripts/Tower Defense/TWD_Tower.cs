using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TWD_Tower : MonoBehaviour
{
    private int sTimer;
    private bool Check=false;
    public Sprite sprite1; // Drag your first sprite here
    public Sprite sprite2; // Drag your second sprite here
  
    private SpriteRenderer spriteRenderer;

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
        detectionArea.radius = range/2;

        spriteRenderer = GetComponent<SpriteRenderer>(); // we are accessing the SpriteRenderer that is attached to the Gameobject
        if (spriteRenderer.sprite == null) // if the sprite on spriteRenderer is null then
            spriteRenderer.sprite = sprite1; // set the sprite to sprite1
    }

    // Update is called once per frame
    void Update()
    {
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
                        Shoot();
                        // Nollataan tulinopeuden ajastin
                        fireRate.Reset();
                    }
                }
            }
        }
        transform.localRotation = transform.localRotation * Quaternion.Inverse(transform.rotation);
        transform.localRotation = transform.localRotation * Quaternion.Euler(90, 0, 0);
        transform.localScale = new Vector3(2, 2, 1);
        sTimer--;
        if (Check == true) // If the space bar is pushed down
        {
            spriteRenderer.sprite = sprite2;
        }
        if (sTimer <= 0) spriteRenderer.sprite = sprite1;
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
    void ChangeSprite()
    {
        spriteRenderer.sprite = sprite2;
        spriteRenderer.sprite = sprite1;
    }
    // Tornin ampuminen 
    private void Shoot()
    {
        Check = true;
        sTimer = 3;

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
                //Check = false;
            }
        }

    }
}
