using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{

    // Sound Effect Object
    public GameObject buttonSoundEffect;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ToMenu()
    {
        ButtonClick();
        SceneManager.LoadScene("Menu");
    }

    public void ToSubMenu()
    {
        ButtonClick();
        SceneManager.LoadScene("SubMenuGame");
    }
    
    public void ToEasyGame()
    {
        ButtonClick();
        PlayerPrefs.SetString("gametype", "EASY");
        SceneManager.LoadScene("Game");
    }

    public void ToMediumGame()
    {
        ButtonClick();
        PlayerPrefs.SetString("gametype", "MEDIUM");
        SceneManager.LoadScene("Game");
    }

    public void ToHardGame()
    {
        ButtonClick();
        PlayerPrefs.SetString("gametype", "HARD");
        SceneManager.LoadScene("Game");
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
