using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Borders : MonoBehaviour
{
    // Onko peliobjekti osa peliobjektitietokantaa.
    [SerializeField] private bool isPooled;

    // Kertoo tuhotaanko peliobjekti, kun se ylittää rajoitetun alueen.
    [Tooltip("Kertoo tuhotaanko peliobjekti, kun se ylittää rajoitetun alueen.")]
    [SerializeField] private bool destroyOnDeparture;

    // Muuttujat, jotka määrittävät rajoitetun alueen (X, Y ja Z-akseleiden minimi- ja maksimiarvot).
    // X-akselin alaraja.
    [SerializeField] private float xMin;
    // X-akselin yläraja.
    [SerializeField] private float xMax;
    // Y-akselin alaraja.
    [SerializeField] private float yMin;
    // Y-akselin yläraja.
    [SerializeField] private float yMax;
    // Z-akselin alaraja.
    [SerializeField] private float zMin;
    // Z-akselin yläraja.
    [SerializeField] private float zMax;

    private void LateUpdate ()
    {
		switch(destroyOnDeparture)
        {
            // Tuhoaa peliobjektin, jos se liikkuu rajoitetun alueen ulkopuolelle.
            case true:
                if (transform.position.x < xMin || transform.position.x > xMax || transform.position.y < yMin || transform.position.y > yMax || transform.position.z < zMin || transform.position.z > zMax)
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
                break;
            // Estää peliobjektin pääsyn rajoitetun alueen ulkopuolelle.
            default:
                if (transform.position.x < xMin) transform.position = new Vector3(xMin, transform.position.y, transform.position.z);
                if (transform.position.x > xMax) transform.position = new Vector3(xMax, transform.position.y, transform.position.z);

                if (transform.position.y < yMin) transform.position = new Vector3(transform.position.x, yMin, transform.position.z);
                if (transform.position.y > yMax) transform.position = new Vector3(transform.position.x, yMax, transform.position.z);

                if (transform.position.z < zMin) transform.position = new Vector3(transform.position.x, transform.position.y, zMin);
                if (transform.position.z > zMax) transform.position = new Vector3(transform.position.x, transform.position.y, zMax);
                break;
        }
	}
}
