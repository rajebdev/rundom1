using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicMenuController : MonoBehaviour
{
    private static MusicMenuController instance = null;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("BGM") == "ON" || PlayerPrefs.GetString("BGM") == "")
        {
            PlayerPrefs.SetString("BGM", "ON");
            gameObject.GetComponent<AudioSource>().Play();
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    // Music Global Start
    public static MusicMenuController Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }
    // Music Global End
}
