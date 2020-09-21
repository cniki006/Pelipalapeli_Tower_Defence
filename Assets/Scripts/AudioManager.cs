using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

    // Äänilähde.
    private AudioSource audioSource;

	// Use this for initialization
	void Start () {
        audioSource = GetComponent<AudioSource>();
	}

    /// <summary>
    /// <para> Toistaa annetun äänen. </para>
    /// </summary>
    public void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    /// <summary>
    /// <para> Toistaa annetun äänen annetulla äänenvoimakkuudella. </para>
    /// </summary>
    public void PlaySound(AudioClip clip, float volume)
    {
        audioSource.PlayOneShot(clip, volume);
    }
}
