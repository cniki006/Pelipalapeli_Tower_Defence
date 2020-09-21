using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lifetime : MonoBehaviour
{
    // Elinaika sekunteina.
    [SerializeField] private float lifetime;
    // Tuhotaanko peliobjekti elinajan päätytyyä.
    [Tooltip("Tuhotaanko peliobjekti elinajan päätyttyä, vai disabloidaanko se.")]
    [SerializeField] private bool destroyOnDeath;

    private void Start ()
    {
        // Kutsuu metodia KillSelf, kun lifetime-muuttujan määrittämä ajanjakso on kulunut.
        Invoke("KillSelf", lifetime);
	}

    // Tuhoaa tai disabloi peliobjektin, jossa tämä scripti on.
    private void KillSelf()
    {
        if (destroyOnDeath)
        {
            Destroy(this.gameObject);
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }
}
