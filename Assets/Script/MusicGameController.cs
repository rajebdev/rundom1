using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicGameController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        // Pausing background menu music
        GameObject soundObject = GameObject.Find("BackgroundSoundMenu");
        if (soundObject != null)
        {
            AudioSource audioSource = soundObject.GetComponent<AudioSource>();
            audioSource.Pause();
        }

        // Playing background game music
        if (PlayerPrefs.GetString("BGM") == "ON" || PlayerPrefs.GetString("BGM") == "")
        {
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
