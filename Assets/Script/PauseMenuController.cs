using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuController : MonoBehaviour
{
    public GameObject player;

    public GameObject musicTick;
    public GameObject bgmTick;
    public Text musicOnText;
    public Text bgmOnText;

    private bool isShowned = false;
    private float transition = 0.0f;

    private float tempSpeed;

    // Sound Effect Object
    public GameObject buttonSoundEffect;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
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
        if (!isShowned) return;

        transition += Time.deltaTime;
        gameObject.GetComponent<Image>().color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 0.5f), transition);
        if (transition > 1.0f)
            Time.timeScale = 0;
    }

    public void TogglePauseMenu()
    {
        ButtonClick();
        tempSpeed = player.GetComponent<PlayerController>().GetSpeed();
        player.GetComponent<PlayerController>().SetSpeed(-7.0f);
        gameObject.SetActive(true);
        isShowned = true;
    }

    // Play Buttons
    public void ToClosePausemenu()
    {
        ButtonClick();
        player.GetComponent<PlayerController>().SetSpeed(-7.0f+tempSpeed);
        gameObject.SetActive(false);
        Time.timeScale = 1;
    }

    public void ToMenu()
    {
        ButtonClick();
        SceneManager.LoadScene("Menu");
        // Playing background menu music
        GameObject soundObject = GameObject.Find("BackgroundSoundMenu");
        if (soundObject != null && PlayerPrefs.GetString("BGM") == "ON")
        {
            AudioSource audioSource = soundObject.GetComponent<AudioSource>();
            audioSource.Play();
        }
    }

    public void OnMusicClick()
    {
        ButtonClick();
        if (musicOnText.text == "ON")
        {
            PlayerPrefs.SetString("SOUND", "OFF");
            musicTick.gameObject.SetActive(false);
            musicOnText.text = "OFF";
        }
        else
        {
            PlayerPrefs.SetString("SOUND", "ON");
            musicTick.gameObject.SetActive(true);
            musicOnText.text = "ON";
        }
    }

    public void OnBGMClick()
    {
        ButtonClick();
        // Playing background menu music
        GameObject soundObject = GameObject.Find("BackgroundMusicPlay");
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

    private void ButtonClick()
    {
        if (PlayerPrefs.GetString("SOUND") == "ON" || PlayerPrefs.GetString("SOUND") == "")
        {
            PlayerPrefs.SetString("SOUND", "ON");
            buttonSoundEffect.GetComponent<AudioSource>().Play();
        }
    }
}
