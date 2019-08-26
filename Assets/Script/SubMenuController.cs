using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenuController : MonoBehaviour
{
    public Text yourName;

    // Start is called before the first frame update
    void Start()
    {
        if  (PlayerPrefs.GetString("nama") != "")
            yourName.text = "HI, "+PlayerPrefs.GetString("nama");
        else
            yourName.text = "HI, Your Name";

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
