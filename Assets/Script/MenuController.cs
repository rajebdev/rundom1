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

    private bool isQuitShowned = false;
    private bool isSettShowned = false;
    private bool isInputShowned = false;

    private float transition;

    public GameObject inputContainer;
    public InputField nameText;
    public GameObject maleCek;
    public GameObject femaleCek;
    

    // Start is called before the first frame update
    void Start()
    {
        quitImg.gameObject.SetActive(false);
        settImg.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
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
        if (isInputShowned)
        {
            transition += Time.deltaTime * 2;
            inputContainer.GetComponent<Image>().color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 0.5f), transition);
        }
    }

    public void ToSubmit()
    {
        SceneManager.LoadScene("SubMenuGame");
    }

    public void ToPlay()
    {
        inputContainer.SetActive(true);
        isInputShowned = true;
        transition = 0.0f;
    }

    public void OnCancelInput()
    {
        inputContainer.SetActive(false);
        isInputShowned = false;
        transition = 0.0f;
        maleCek.SetActive(false);
        femaleCek.SetActive(false);
    }

    public void ToClearText()
    {
        nameText.Select();
        nameText.text = "";
    }

    public void ToMale()
    {
        maleCek.SetActive(true);
        femaleCek.SetActive(false);
    }

    public void ToFemale()
    {
        femaleCek.SetActive(true);
        maleCek.SetActive(false);
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
