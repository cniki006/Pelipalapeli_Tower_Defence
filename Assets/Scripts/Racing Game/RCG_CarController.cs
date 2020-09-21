using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RCG_CarController : MonoBehaviour {

    [System.Serializable]
    private class AxleInfo
    {
        // Viittaus vasemmanpuoleiseen pyörään.
        public WheelCollider leftWheel;
        // Viittaus oikeanpuoleiseen pyörään.
        public WheelCollider rightWheel;
        // Apumuuttuja, joka kertoo onko akseli vetävä.
        [SerializeField] public bool powered;
        // Apumuuttuja, joka kertoo onko akseli ohjaava.
        [SerializeField] public bool steering;
        // Apumuuttuja, joka kertoo onko akseli jarrullinen.
        [SerializeField] public bool braking;
    }

    // Lista kaikista auton akseleista.
    [SerializeField] private List<AxleInfo> axleInfos;
    // Moottorin vääntömomentin maksimiarvo.
    [SerializeField] private float maxMotorTorque;
    // Ohjauskulman maksimiarvo.
    [SerializeField] private float maxSteeringAngle;
    // Kallistuksenvakainjärjestelmän vakautusvoiman maksimiarvo. 
    [SerializeField] private float maxAntiRollForce;
    // Seisontajarrun jarrutusvoima.
    [SerializeField] private float parkingBrakeForce;
    // Auton huippunopeus (km/h).
    [SerializeField] private float topSpeed;
    // Viittaus auton Rigidbody-komponenttiin.
    private Rigidbody rb;
    // Apumuuttuja, joka ilmaisee onko seisontajarru päällä vai pois päältä.
    private bool parkingBrake;
    // Moottorin vääntömomentin arvo.
    private float wheelTorque;
    // Ohjauskulman arvo.
    private float steeringAngle;
    // Apumuuttuja, joka ilmaisee ollaanko auton sijaintia palauttamassa viimeisimpään väliaika- tai aloituspisteeseen.
    private bool resetPosition;
    // Viittaus palautuspisteeseen, johon auto voidaan palauttaa.
    private Transform spawnPosition;
    // Apumuuttuja, joka ilmaisee luetaanko käyttäjän syötteitä.
    private bool allowInput;

    public float GetCurrentSpeedNormalized() { return rb.velocity.magnitude / topSpeed; }
    public bool GetResetPosition() { return resetPosition; }
    public void SetAllowInput(bool b) { allowInput = b; }

    // Use this for initialization
    private void Start ()
    {
        // Haetaan viittaus Rigidbody-komponenttiin.
        rb = GetComponent<Rigidbody>();

        // Alustetaan apumuuttujien arvot.
        allowInput = true;
        parkingBrake = true;
        resetPosition = false;
        wheelTorque = 0.0f;
        steeringAngle = 0.0f;

        // Muutetaan huippunopeus muotoon m/s.
        topSpeed = topSpeed / 3.6f;

        // Varmistetaan että seisontajarrun voima ei ole negatiivinen.
        if (parkingBrakeForce < 0)
        {
            parkingBrakeForce = parkingBrakeForce * (-1);
        }
    }
	
	// Update is called once per frame
	private void Update ()
    {
        if(Input.GetButtonDown("Jump") && allowInput)
        {
            if(parkingBrake)
            {
                parkingBrake = false;
            }
            else
            {
                parkingBrake = true;
            }
        }
	}

    private void FixedUpdate()
    {
        if (resetPosition)
        {
            // Asetetaan auton sijainniksi palautuspiste sekä nollataan Rigidbody-komponentin nopeusarvot ja moottorin vääntömomentin arvo.
            transform.position = spawnPosition.position + (Vector3.up);
            transform.rotation = spawnPosition.rotation;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            wheelTorque = 0.0f;
            resetPosition = false;
        }

        if(allowInput)
        {
            // Lasketaan moottorin vääntömomentti käyttäen käyttäjän syötteen arvoa sekä moottorin vääntömomentin maksimiarvoa.
            wheelTorque = maxMotorTorque * Input.GetAxis("Vertical");
            // Lasketaan ohjauskulma käyttäen käyttäjän syötteen arvoa sekä ohjauskulman maksimiarvoa.
            steeringAngle = maxSteeringAngle * Input.GetAxis("Horizontal");
        }
        else
        {
            // Asetetaan moottorin vääntömomentti ja ohjauskulma nollaan sekä aktivoidaan seisontajarru.
            wheelTorque = maxMotorTorque * 0.0f;
            steeringAngle = maxSteeringAngle * 0.0f;
            parkingBrake = true;
        }

        // Käydään läpi jokainen auton akseli.
        foreach (AxleInfo axleInfo in axleInfos)
        {
            if(axleInfo.steering)
            {
                // Jos akseli on ohjaava, päivitetään ohjauskulma molemmille pyörille.
                axleInfo.leftWheel.steerAngle = steeringAngle;
                axleInfo.rightWheel.steerAngle = steeringAngle;
            }
            if(axleInfo.powered)
            {
                // Jos akseli on vetävä, päivitetään vääntömomentti molemmille pyörille.
                axleInfo.leftWheel.motorTorque = wheelTorque;
                axleInfo.rightWheel.motorTorque = wheelTorque;
            }
            if(axleInfo.braking && parkingBrake)
            {
                // Jos akseli on jarrullinen ja seisontajarru on aktivoituna, asetetaan molemmille pyörille jarruvoimaksi seisontajarrun jarruvoiman arvo.
                axleInfo.leftWheel.brakeTorque = parkingBrakeForce;
                axleInfo.rightWheel.brakeTorque = parkingBrakeForce;
            }
            else if(axleInfo.braking)
            {
                // Jos akseli on jarrullinen, asetetaan molemmille pyörille jarruvoimaksi seisontajarrun jarruvoiman arvo.
                axleInfo.leftWheel.brakeTorque = 0.0f;
                axleInfo.rightWheel.brakeTorque = 0.0f;
            }
            // Kutsutaan kallistuksenvakainjärjestelmän toiminnallisuutta.
            ApplyStabilizerBarForce(axleInfo);
            // Asetetaan pyörien visuaaliset mallit vastaamaan pyörien ohjauskulmaa.
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

        // Jos auton nopeus ylittää huippunopeuden arvon asetetaan nopeudeksi huippunopeutta vastaava arvo.
        if (rb.velocity.magnitude > topSpeed)
        {
            rb.velocity = rb.velocity.normalized * topSpeed;
        }

    }

    private void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        // Jos pyörällä ei ole visuaalista mallia.
        if(collider.transform.childCount == 0)
        {
            return;
        }

        // Haetaan viittaus visuaaliseen malliin.
        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        // Luetaan pyörän sijainti ja kierto ja talletetaan niiden arvot muuttujiin.
        collider.GetWorldPose(out position, out rotation);

        // Asetetaan visuaalisen mallin sijainti ja kierto.
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    private void ApplyStabilizerBarForce(AxleInfo axleInfo)
    {
        WheelHit hit;
        float travelLeft = 1.0f;
        float travelRright = 1.0f;

        // Tarkastellaan koskettaako akselin vasen pyörä maata ja koskettaessa lasketaan jousituksessa jäljellä olevan jouston suuruus (jousen pituus ajanhetkellä).
        bool groundedLeft = axleInfo.leftWheel.GetGroundHit(out hit);
        if(groundedLeft)
        {
            travelLeft = (-axleInfo.leftWheel.transform.InverseTransformPoint(hit.point).y - axleInfo.leftWheel.radius) / axleInfo.leftWheel.suspensionDistance;
        }

        // Tarkastellaan koskettaako akselin oikea pyörä maata ja koskettaessa lasketaan jousituksessa jäljellä olevan jouston suuruus (jousen pituus ajanhetkellä).
        bool groundedRight = axleInfo.rightWheel.GetGroundHit(out hit);
        if(groundedRight)
        {
            travelRright = (-axleInfo.rightWheel.transform.InverseTransformPoint(hit.point).y - axleInfo.rightWheel.radius) / axleInfo.rightWheel.suspensionDistance;
        }

        // Lasketaan kallistuksenvakaimen ajoneuvon koriin kohdistaman voiman suuruus.
        float antiRollForce = (travelLeft - travelRright) * maxAntiRollForce;

        // Ajoneuvon korin kallistuessa vasemmalle, kohdistetaan kallistuksenvakaimen voima vasempaan puoleen.
        if(groundedLeft)
        {
            rb.AddForceAtPosition(axleInfo.leftWheel.transform.up * -antiRollForce, axleInfo.leftWheel.transform.position);
        }

        // Ajoneuvon korin kallistuessa oikealle, kohdistetaan kallistuksenvakaimen voima oikeaan puoleen.
        if (groundedRight)
        {
            rb.AddForceAtPosition(axleInfo.rightWheel.transform.up * antiRollForce, axleInfo.rightWheel.transform.position);
        }
    }

    /// <summary>
    /// <para>Aktivoi auton palauttamisen palautuspisteeseen.</para>
    /// </summary>
    /// <param name="position"></param>
    public void Respawn(Transform position)
    {
        spawnPosition = position;
        resetPosition = true;
    }
}
