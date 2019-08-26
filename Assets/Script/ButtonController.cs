using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonController : MonoBehaviour
{

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
        SceneManager.LoadScene("Menu");
    }

    public void ToSubMenu()
    {
        SceneManager.LoadScene("SubMenuGame");
    }
    
    public void ToEasyGame()
    {
        SceneManager.LoadScene("EasyGame");
    }

    public void ToMiddleGame()
    {
        SceneManager.LoadScene("MiddleGame");
    }

    public void ToHardGame()
    {
        SceneManager.LoadScene("HardGame");
    }


}
