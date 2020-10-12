using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TWD_Projectile : MonoBehaviour
{
    //Projektiilin nopeus
    [SerializeField] private float velocity;

    //Projektiilin elämäpisteet.
    [SerializeField] private Resource health;
    //Ampuvan tornin on tarkoitus antaa arvo projektiilin vahinkoarvoksi.
    //Projektiilin vahinkoarvo.
    private float damage;

    //Projektiilin elinaika.
    private Timer lifetime;
    // Start is called before the first frame update

    public float GetVelocity() { return velocity; }
    public float GetDamage() { return damage; }
    public void SetDamage(float value) { damage = value; }
    public void SetLifetime(float value) { lifetime = new Timer(value, 0.0f); }

    void Start()
    {
        //Alustetaan elämänpisteet.
        health.SetValue(health.GetMax());    
    }

    // Update is called once per frame
    void Update()
    {
        //Jos projektiilin elinaika on kulunut loppuun, se tuhotaan
        if(lifetime.IsFinished())
        {
            Destruction();
        }

        //Kun elinaika ei ole vielä lopussa
        else
        {
            lifetime.Count();
            //Liikutetaan peliobjektia eteenpäin
            transform.Translate(Vector3.forward * velocity * Time.deltaTime);
            transform.Rotate(90,90,0);
        }
        
    }

    private void LateUpdate()
    {
        //Jos elämäpisteet laskevat minimiin, tuhotaan tämä objekti
        if (health.GetValue() <= health.GetMin())
        {
            Destruction();
        }
    }

    private void OnEnable()
    {
        //Kun projektiili enabloidaan, alustetaan sen elämäpisteet.
        health.SetValue(health.GetMax());
    }

    //Kun peliobjekti törmää toiseen objektiin
    private void OnTriggerEnter(Collider other)
    {
        //Katsotaan onko toisen objektin tägi "Hostile".
        if(other.CompareTag("Hostile"))
        {
            //Projektiili menettää yhden elämäpisteen törmäyksessä
            health.SetValue(health.GetValue() - 1);
        }

    }

    //Peliobjektin tuhoutuminen
    private void Destruction()
    {
        //Disabloidaan tämä objekti kun se halutaan tuhota.
        this.gameObject.SetActive(false);
    }
}

