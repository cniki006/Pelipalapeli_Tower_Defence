using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    // projektiilin nopeus.
    [SerializeField] private float velocity;
    // Onko peliobjekti osa peliobjektitietokantaa.
    [SerializeField] private bool isPooled;
    // Onko peliobjekti elossa.
    private bool alive = true;

    private void Update ()
    {
        // Liikutetaan peliobjektia eteenpäin.
        transform.Translate(Vector3.forward * velocity * Time.deltaTime);
	}

    private void LateUpdate()
    {
        // Kun projektiili ei ole elossa, tuhotaan se tai jos projektiili on osa peliobjektitietokantaa, disabloidaan se.
        if (!alive)
        {
            if (isPooled)
            {
                this.gameObject.SetActive(false);
            }
            else
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnEnable()
    {
        // Kun projektiili enabloidaan, asetetaan se elossa olevaksi.
        alive = true;
    }

    // Kun peliobjekti törmää toiseen peliobjektiin.
    private void OnTriggerEnter(Collider other)
    {
        // Katsotaan onko toisen objektin tagi muu kuin "Trigger" eikä se ole projektiili.
        if (!other.CompareTag("Trigger") && !other.CompareTag("Collectable") && !other.GetComponent<Projectile>())
        {
            alive = false;
        }
    }
}
