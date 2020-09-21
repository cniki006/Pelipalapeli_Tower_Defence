using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCG_CameraController : MonoBehaviour {

    // Viittaus pelimanageriin.
    private RCG_GameManager gameManager;
    // Viittaus kohteeseen, jota kamera seuraa.
    [SerializeField] Transform target;
    // Viittaus varsinaiseen kamerapeliobjektiin (Main Camera).
    [SerializeField] GameObject cameraObject;
    // Viittaus kameran kääntöpisteeseen.
    [SerializeField] GameObject cameraPivot;
    // Kameran kiertonopeus.
    [SerializeField] private float rotationSpeed;
    // Kameran zoomaamisnopeus.
    [SerializeField] private float zoomSpeed;
    // Kameran zoomin raja-arvot (min & max).
    [SerializeField] private Vector2 cameraZoomLimit;
    // Kameran alustava sijainti juuriobjektiin nähden.
    [SerializeField] private float viewOffset;
    // Kameran juuriobjektin sijainnin poikkeama kohteesta.
    [SerializeField] private Vector3 cameraCenterOffset;
    // Kameran kulman minimi ja maksimi arvot.
    [SerializeField] private Vector2 cameraAngleLimit;
    // Kameran kulma suhteessa juuriobjektiin.
    private float cameraAngle;
    // Kameran juuriobjektin kääntämisessä käytettävä kierto.
    private Quaternion rotation;
    // Viittaus kolmannen persoonan kameran kamera-komponenttiin.
    private Camera orbitCamera;
    // Viittaus konepeltikameran kamera-komponenttiin.
    private Camera hoodCamera;
    // Viittaus kolmannen persoonan kameran AudioListener-komponenttiin.
    private AudioListener orbitListener;
    // Viittaus konepeltikameran AudioListener-komponenttiin.
    private AudioListener hoodListener;

    // Use this for initialization
    private void Start ()
    {
        // Haetaan viittaus pelimanageriin.
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<RCG_GameManager>();

        // Haetaan viittaukset kamera-komponentteihin.
        orbitCamera = cameraObject.GetComponent<Camera>();
        hoodCamera = target.GetComponentInChildren<Camera>();
        orbitListener = cameraObject.GetComponent<AudioListener>();
        hoodListener = target.GetComponentInChildren<AudioListener>();

        // Alustetaan kamera-komponenttien käyttö.
        orbitCamera.enabled = true;
        hoodCamera.enabled = false;
        orbitListener.enabled = true;
        hoodListener.enabled = false;

        // Lasketaan alustava kameran sijainti käyttäen juuriobjektin sijaintia sekä kameralle annettua sijaintia juuriobjektiin nähden.
        cameraObject.transform.localPosition = new Vector3(0, 0, -viewOffset);
        rotation = transform.rotation;
    }

    // Update is called once per frame
    private void Update()
    {
        if (gameManager.isGameOver() || Time.timeScale == 0.0f)
        {
            return;
        }
        SwitchView();
        if(orbitCamera.enabled)
        {
            gameManager.RotateWaypointIndicator(orbitCamera.transform);
        }
        else
        {
            gameManager.RotateWaypointIndicator(hoodCamera.transform);
        }
    }

    private void LateUpdate()
    {
        if (gameManager.isGameOver() || Time.timeScale == 0.0f)
        {
            return;
        }

        // Alustetaan pelaajan syötteen arvot.
        float xInput = 0.0f;
        float yInput = 0.0f;

        // Asetetaan juuriobjektin sijainti kohteen sijainnin ja sijainnin poikkeaman kohteesta mukaan.
        transform.position = target.position + cameraCenterOffset;

        // Kameran zoomi, jolle on asetettu raja-arvot.
        if ((Input.GetAxis("Mouse ScrollWheel") > 0 && -cameraObject.transform.localPosition.z > cameraZoomLimit.x) || (Input.GetAxis("Mouse ScrollWheel") < 0 && -cameraObject.transform.localPosition.z < cameraZoomLimit.y))
        {
            cameraObject.transform.localPosition = cameraObject.transform.localPosition + (Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * -cameraObject.transform.localPosition.normalized);
        }

        // Kun kolmannen persoonan näkymä on aktiivinen vastaanotetaan pelaajan syötettä.
        if (orbitCamera.enabled)
        {
            xInput = Input.GetAxis("Mouse X");
            yInput = Input.GetAxis("Mouse Y");
        }
        // Luodaan juuriobjektille kierto globaalin Y-akselin ympärillä pelaajan syötteen mukaan.
        rotation = Quaternion.AngleAxis(xInput * rotationSpeed, Vector3.up) * rotation;
        // Käännetään juuriobjektia globaalin Y-akselin ympärillä huomioiden kohteen kierto.
        transform.rotation = rotation * Quaternion.Euler(0.0f, target.rotation.eulerAngles.y, 0.0f);

        // Lasketaan kameran kulma rajaten se raja-arvojen välille.
        cameraAngle = Mathf.Clamp(cameraAngle + (-yInput * rotationSpeed), cameraAngleLimit.x, cameraAngleLimit.y);
        // Asetetaan kameran kääntöpisteen kierto vastaamaan kameran kulmaa.
        cameraPivot.transform.localRotation = Quaternion.Euler(cameraAngle, 0, 0);
    }

    private void SwitchView()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            orbitCamera.enabled = !orbitCamera.enabled;
            hoodCamera.enabled = !hoodCamera.enabled;
            orbitListener.enabled = !orbitListener.enabled;
            hoodListener.enabled = !hoodListener.enabled;
        }
    }
}
