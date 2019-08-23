using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    private int life = 4;

    private CharacterController character;
    private Vector3 moveVector;

    private bool isDead = false;

    private float speed = 7.0f;
    private float verticalVelocity = 0.0f;
    private float gravity = 10f;
    private float jumpHeight = 15.0f;
    private float startTime = 0;
    private float animationDuration = 3.0f;

    private int userPos;
   
    // Kinect Controller
    KinectManager kinect = KinectManager.Instance;

    public bool kinectPower = false;

    public GameObject warningImg;

    public float offSetLoncat = 0.1f;

    private bool detected = false;

    private int Bahu = 2;
    private int Kepala = 3;
    private int BahuKiri = 4;
    private int BahuKanan = 8;

    private float yKepala;
    private float zKepala;
    private float xBahu;
    private float yBahu;
    private float xBahuKanan;
    private float xBahuKiri;
    
    // Time variable
    public Text timeText;
    private float secTime;
    private float tempSecTime;
    private bool warningAktif = false;

    // Count Screen Controller
    public GameObject countScreen;
    public Text countText;
    public Text readyText;

    private int count = 5;

    // Start is called before the first frame update
    void Start()
    {
        character = GetComponent<CharacterController>();
        StartCoroutine(TimeRun());
        Time.timeScale = 1;
        PlayerPrefs.DeleteKey("xBahuKanan");
        PlayerPrefs.DeleteKey("xBahuKiri");
    }

    void Update()
    {
        countScreen.SetActive(false);

        if (Input.GetKeyDown(KeyCode.A))
            detected = true;

        try
        {
            kinect = KinectManager.Instance;
            kinectPower = true;
        }
        catch
        {
            kinectPower = false;
        }

        if (kinectPower)
        {
            if (kinect.IsUserDetected() || detected)
            {
                warningImg.SetActive(false);
                if (PlayerPrefs.GetFloat("xBahuKanan") == 0)
                {
                    PlayerPrefs.SetFloat("xBahuKanan", getPosX(BahuKanan, 1));
                    PlayerPrefs.SetFloat("xBahuKiri", getPosX(BahuKiri, 1));
                }

                if (count > -1)
                {
                    countScreen.SetActive(true);
                    countText.text = count.ToString();
                    return;
                }else if (count == 0)
                {
                    readyText.text = "Go!";
                    countScreen.SetActive(false);
                    secTime = tempSecTime;
                }

            }
            else
            {
                warningImg.SetActive(true);
                if (warningAktif)
                {
                    warningAktif = false;
                }
                else
                {
                    warningAktif = true;
                }

                secTime = 0;
                count = 5;
                return;
            }

            if (kinect.IsUserDetected())
                KinectController();
        }

        // Time Controller
        string tempTime = string.Format("{0:00}:{1:00}", (int)(secTime / 60), (int)(secTime % 60));
        timeText.GetComponent<Text>().text = tempTime;
        
        // Dead Checker
        if (isDead) return;

        // Stoping Game
        if (character.transform.position.y < 0) Death();

        // Disable Control During Animation Camera
        if (Time.time - startTime < animationDuration)
        {
            //character.Move(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        // Reset Move Vector
        moveVector = Vector3.zero;


        // set Gravity
        if (character.isGrounded) verticalVelocity = -0.5f;
        else verticalVelocity -= gravity * Time.deltaTime;

        // character Jump
        if (character.isGrounded)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
                moveVector.y = jumpHeight * speed;
            else
            {
                if (kinectPower)
                {
                    if (kinect.IsUserDetected())
                    {
                        if (IsJump())
                            moveVector.y = jumpHeight * speed;
                    }
                }
                    
            }
        }

        // X - LR
        moveVector.x = 0;

        // Y - UD
        moveVector.y += verticalVelocity;
        
        // Z - FB
        moveVector.z = speed;
        
        // character is Moving by moveVector
        character.Move(moveVector * Time.deltaTime);

        // Controller left right
        if (Input.GetKeyDown(KeyCode.LeftArrow) && userPos > -1)
            userPos -= 1;
        else if (Input.GetKeyDown(KeyCode.RightArrow) && userPos < 1)
            userPos += 1;
        else
        {

            if (kinectPower)
            {
                if (kinect.IsUserDetected())
                {
                    if (IsLeft())
                        userPos = -1;
                    else if (IsRight())
                        userPos = 1;
                    else
                        userPos = 0;
                }
            }
        }

        transform.position = new Vector3(userPos * 2.0f, transform.position.y, transform.position.z);

    }

    public void SetSpeed(float speedModif)
    {
        speed = 7.0f + speedModif;
    }

    public float GetSpeed()
    {
        return speed;
    }

    // Mengecek tabrakan player dengan benda di depannya
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.point.z > transform.position.z + character.radius || Input.GetKeyDown(KeyCode.K))
            Death();
    }

    private void Death()
    {
        life -= 1;
        GameObject tempHearth = GameObject.Find(string.Format("HearthImg ({0})", life));
        Destroy(tempHearth);
        if (life == 0)
        {
            isDead = true;
            GetComponent<ScoreController>().OnDeath();
        }
    }

    private float getPosX(int partBody, int digit)
    {
        return (float)Math.Round(Convert.ToDouble(kinect.GetJointPosition(partBody).x), digit);
    }

    private float getPosY(int partBody, int digit)
    {
        return (float)Math.Round(Convert.ToDouble(kinect.GetJointPosition(partBody).y), digit);
    }

    private float getPosZ(int partBody, int digit)
    {
        return (float)Math.Round(Convert.ToDouble(kinect.GetJointPosition(partBody).z), digit);
    }

    private void KinectController()
    {
        if (getPosZ(Kepala, 1) != zKepala)
        {
            if (kinect.IsJointTracked(Kepala))
                zKepala = getPosZ(Kepala, 1);

            if (kinect.IsJointTracked(Kepala))
                yKepala = getPosY(Kepala, 2);
        }

        if (kinect.IsJointTracked(Bahu))
            yBahu = getPosY(Bahu, 2);

        if (kinect.IsJointTracked(BahuKanan))
            xBahuKanan = getPosX(BahuKanan, 1);

        if (kinect.IsJointTracked(BahuKiri))
            xBahuKiri = getPosX(BahuKiri, 1);
        
    }

    private bool IsJump()
    {
        bool tempJump = false;
        if (yBahu + offSetLoncat > yKepala)
        {
            tempJump = true;
        }

        return tempJump;
    }

    private bool IsLeft()
    {
        bool tempLeft = false;
        if (xBahuKanan < PlayerPrefs.GetFloat("xBahuKiri"))
            tempLeft = true;
        return tempLeft;
    }

    private bool IsRight()
    {
        bool tempRight = false;
        if (xBahuKiri > PlayerPrefs.GetFloat("xBahuKanan"))
            tempRight = true;
        return tempRight;
    }



    private IEnumerator TimeRun()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            secTime++;
            count--;
        }
    }
}
