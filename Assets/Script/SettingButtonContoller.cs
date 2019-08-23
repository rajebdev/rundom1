using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingButtonContoller : MonoBehaviour
{
    public GameObject musicTick;
    public GameObject bgmTick;
    public Text musicOnText;
    public Text bgmOnText;

    // Start is called before the first frame update
    void Start()
    {
        musicTick.gameObject.SetActive(true);
        if (PlayerPrefs.GetString("BGM") == "ON" || PlayerPrefs.GetString("BGM") == "")
        {
            bgmTick.gameObject.SetActive(true);
            bgmOnText.text = "ON";
        }
        else
        {
            bgmTick.gameObject.SetActive(false);
            bgmOnText.text = "OFF";
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnMusicClick()
    {
        if (musicOnText.text == "ON")
        {
            musicTick.gameObject.SetActive(false);
            musicOnText.text = "OFF";
        }
        else
        {
            musicTick.gameObject.SetActive(true);
            musicOnText.text = "ON";
        }
    }

    public void OnBGMClick()
    {
        // Playing background menu music
        GameObject soundObject = GameObject.Find("BackgroundSoundMenu");
        AudioSource audioSource = soundObject.GetComponent<AudioSource>();

        if (bgmOnText.text == "ON")
        {
            bgmTick.gameObject.SetActive(false);
            PlayerPrefs.SetString("BGM", "OFF");
            bgmOnText.text = "OFF";
            audioSource.Pause();
        }
        else
        {
            bgmTick.gameObject.SetActive(true);
            PlayerPrefs.SetString("BGM", "ON");
            bgmOnText.text = "ON";
            audioSource.Play();
        }
    }
}
