using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spin : MonoBehaviour
{
    // Pyörimisliikkeen suunta per akseli ja kiertokulma per sekunti.
    [Tooltip("Pyörimisliikkeen suunta per akseli ja kiertokulma per sekunti.")]
    [SerializeField] private Vector3 spinAxis;
    // Pyöritetäänkö peliobjektia sen lokaalien akseleiden ympäri.
    [SerializeField] private bool useLocalSpace;

    private void Update ()
    {
        // Pyöritetään peliobjektia annettujen arvojen mukaan sen lokaalien akseleiden ympäri.
        if (useLocalSpace)
        {
            this.transform.Rotate(spinAxis * Time.deltaTime, Space.Self);
        }
        // Pyöritetään peliobjektia annettujen arvojen mukaan pelimaailman akseleiden ympäri.
        else
        {
            this.transform.Rotate(spinAxis * Time.deltaTime, Space.World);
        }
    }
}
