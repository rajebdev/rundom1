using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    public Transform[] buttons;
    public Image quitImg;
    public Image settImg;

    //public Image mouseImg;

    private bool isQuitShowned = false;
    private bool isSettShowned = false;

    private float transition;

    //private int i;

    // Start is called before the first frame update
    void Start()
    {
        quitImg.gameObject.SetActive(false);
        settImg.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //// Controller cursor with mouse
        //mouseImg.gameObject.transform.position = Input.mousePosition;
        //// Detection button selected
        //while (i < buttons.Length)
        //{
        //    if (mouseImg.gameObject.transform.position.x == buttons[i].position.x)
        //    {
        //        Debug.Log("Collide Mouse With "+buttons[i]);
        //    }
        //    i++;
        //}
        //i = 0;
        if (isQuitShowned)
        {
            transition += Time.deltaTime * 2;
            quitImg.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 0.5f), transition);
        }
        if (isSettShowned)
        {
            transition += Time.deltaTime * 2;
            settImg.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 0.5f), transition);
        }
    }

    public void ToGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void OnClickQuit()
    {
        quitImg.gameObject.SetActive(true);
        isQuitShowned = true;
    }

    public void OnClickQuitNo()
    {
        quitImg.gameObject.SetActive(false);
        isQuitShowned = false;
        transition = 0.0f;
    }

    public void OnClickQuitYes()
    {
        Application.Quit();
    }

    public void OnClickSett()
    {
        settImg.gameObject.SetActive(true);
        isSettShowned = true;
    }

    public void OnClickSettClose()
    {
        settImg.gameObject.SetActive(false);
        isSettShowned = false;
        transition = 0.0f;
    }
}
