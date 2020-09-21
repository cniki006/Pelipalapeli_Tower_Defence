using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RCG_UIManager : MonoBehaviour {

    // Analoogisen nopeusmittarin neulan maksimikulma.
    [SerializeField] private float SpeedometerAngleMax = 0;
    // Analoogisen nopeusmittarin neulan minimikulma.
    [SerializeField] private float SpeedometerAngleMin = 0;
    // Analoogisen nopeusmittarin neulan sektori.
    private float speedometerRange;
    // Viittaus analoogisen nopeusmittarin neulan Transform-komponenttiin.
    [SerializeField] private Transform speedometerNeedleTransform;

    // Viittaus kierrosta ilmaisevaan tekstiin.
    [SerializeField] private Text lapInfoText;
    // Viittaus Kulunutta peliaikaa ilmaisevaan tekstiin.
    [SerializeField] private Text timeInfoText;
    // Viittaus Kokonaispeliajan ilmaisevaan tekstiin.
    [SerializeField] private Text endTimeText;

    // Viittaus pelin aloitusruutuun.
    [SerializeField] private GameObject waitingScreen;
    // Viittaus pelin maaliinajoruutuun.
    [SerializeField] private GameObject finishScreen;
    // Viittaus pelin lopetusruutuun.
    [SerializeField] private GameObject gameOverScreen;
    // Viittaus pelin HUD-elementit käsittävään paneeliin.
    [SerializeField] private GameObject HUD;

    // Viittaus väliaikapistesuuntanuoleen.
    [SerializeField] private GameObject waypointIndicator;

    public void SetWaitingScreenActive(bool b) { waitingScreen.SetActive(b); }
    public void SetFinishScreenActive(bool b) { finishScreen.SetActive(b); }
    public void SetGameOverScreenActive(bool b) { gameOverScreen.SetActive(b); }
    public void SetHUDActive(bool b) { HUD.SetActive(b); }

    // Use this for initialization
    private void Start ()
    {
        // Alustetaan nopeusmittarin neulan sektori sekä kulunutta peliaikaa ilmaiseva teksti.
        speedometerRange = SpeedometerAngleMin + SpeedometerAngleMax;
        timeInfoText.text = "00:00.00";
	}

    /// <summary>
    /// <para>Päivittää nopeusmittarin lukeman.</para>
    /// </summary>
    /// <param name="speed"></param>
    public void UpdateSpeedometer(float speed)
    {
        speedometerNeedleTransform.eulerAngles = new Vector3(0, 0, SpeedometerAngleMin - speed * speedometerRange);
    }

    /// <summary>
    /// <para>Päivittää kierrosta ilmaisevan tekstin.</para>
    /// </summary>
    /// <param name="currentLap"></param>
    /// <param name="lapCount"></param>
    public void UpdateLapInfo(int currentLap, int lapCount)
    {
        lapInfoText.text = "LAP " + currentLap.ToString() + "/" + lapCount.ToString();
    }

    /// <summary>
    /// <para>Päivittää peliaikalaskurin.</para>
    /// </summary>
    /// <param name="time"></param>
    public void UpdateTimer(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60.0f);
        int seconds = (int)(time % 60);
        int fraction = (int)((time * 100) % 100);

        timeInfoText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, fraction);
    }

    /// <summary>
    /// <para>Asettaa kokonaispeliajan ilmaisevan tekstin arvoksi kulunutta peliaikaa ilmaisevan tekstin arvon.</para>
    /// </summary>
    public void SetEndTimeText()
    {
        endTimeText.text = timeInfoText.text;
    }

    /// <summary>
    /// <para>Kääntää väliaikapistesuuntanuolen osoittamaan kohti kohdetta.</para>
    /// </summary>
    /// <param name="source"></param>
    /// <param name="target"></param>
    public void RotateWaypointIndicator(Transform source, Transform target)
    {
        Vector3 waypointDirection = target.position - source.position;
        waypointIndicator.transform.localEulerAngles = new Vector3(60.0f, 0, Vector2.SignedAngle(new Vector2(source.forward.x, source.forward.z), new Vector2(waypointDirection.x, waypointDirection.z)));
    }
}
