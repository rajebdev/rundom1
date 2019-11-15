using System.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Mono.Data.Sqlite;
using System;
using Random = UnityEngine.Random;

public class Gameplay : MonoBehaviour
{
    public GameObject player;

    public GameObject playerType1, playerType2;

    public Image questionImg;
    public GameObject[] choicesBox;
    
    public Sprite[] shapes;

    public int questionId;

    private int banyakSoal = 3;

    public int[] quesList, answerList, shapeChoice1List, shapeChoice2List, quesColor, shape1ColorList, shape2ColorList;

    // Change Soal

    private bool changeQuestion;

    public int timeHit;

    public int countdownQuestion;

    public int jarakSetiapSoal = 12;

    public Text namaBangunText;

    private string[] namaBangun =
    {
        "CIRCLE",
        "HEKSAGON",
        "KITE",
        "OVAL",
        "PARALLELOGRAM",
        "PENTAGON",
        "RECTANGLE",
        "RHOMBUS",
        "SQUARE",
        "STAR",
        "TRIANGLE"
    };

    private string[] colors =
    {
        "RED",
        "GREEN",
        "BLUE",
        "YELLOW"
    };

    private Color[] colorValues =
    {
        Color.red,
        Color.green,
        Color.blue,
        Color.yellow
    };

    // Countdown
    public Image containerCountdownSoal;
    public Text countdownText;

    public GameObject gameEndText;

    // guide music
    private GuideMusicController guideMusic;

    private bool pilihBentukIsPlay = false;
    private bool bentukIsPlay;

    // Start is called before the first frame update
    void Start()
    {
        guideMusic = GameObject.Find("GuideMusic").GetComponent<GuideMusicController>();
        quesList = new int[banyakSoal];
        answerList = new int[banyakSoal];
        shapeChoice1List = new int[banyakSoal];
        shapeChoice2List = new int[banyakSoal];
        quesColor = new int[banyakSoal];
        shape1ColorList = new int[banyakSoal];
        shape2ColorList = new int[banyakSoal];

        // Get Rendered Object
        Renderer rend = player.GetComponent<Renderer>();

        if (PlayerPrefs.GetString("gender") == "L")
            playerType1.SetActive(false);
        else
            playerType2.SetActive(false);
        if (PlayerPrefs.GetString("gametype") == "MEDIUM")
        {
            for (int i = 0; i < banyakSoal; i++)
            {
                int shapeQ = GetShapeNum();
                while (quesList.Contains(shapeQ))
                {
                    shapeQ = GetShapeNum();
                }
                quesList[i] = shapeQ;

                int answer = Random.Range(0, 3);
                answerList[i] = answer;

                int shape1 = GetShapeNum();
                while (shape1 == shapeQ)
                {
                    shape1 = GetShapeNum();
                }
                shapeChoice1List[i] = shape1;

                int shape2 = GetShapeNum();
                while (shape2 == shapeQ || shape2 == shape1)
                {
                    shape2 = GetShapeNum();
                }
                shapeChoice2List[i] = shape2;
                quesColor[i] = 2;
                shape1ColorList[i] = 2;
                shape2ColorList[i] = 2;
            }
        }
        else
        {
            for (int i = 0; i < banyakSoal; i++)
            {
                int shapeQ = GetShapeNum();
                int colorQ = GetRandColor();
                while (quesList.Contains(shapeQ))
                {
                    shapeQ = GetShapeNum();
                }
                quesList[i] = shapeQ;
                quesColor[i] = colorQ;

                int answer = Random.Range(0, 3);
                answerList[i] = answer;

                int shape1 = GetShapeNum();
                int color1 = GetRandColor();
                while (shape1 == shapeQ && color1 == colorQ)
                {
                    shape1 = GetShapeNum();
                    color1 = GetRandColor();
                }
                shapeChoice1List[i] = shape1;
                shape1ColorList[i] = color1;

                int shape2 = GetShapeNum();
                int color2 = GetRandColor();
                while ((shape2 == shapeQ && color2 == colorQ) || (shape2 == shape1 && color2 == color1))
                {
                    shape2 = GetShapeNum();
                    color2 = GetRandColor();
                }
                shapeChoice2List[i] = shape2;
                shape2ColorList[i] = color2;
            }
        }
        StartGameDatabase();
    }

    // Update is called once per frame
    void Update()
    {

        if (timeHit != 0)
            countdownQuestion = player.GetComponent<PlayerController>().secTime - timeHit;

        containerCountdownSoal.gameObject.SetActive(false);


        if (questionId < banyakSoal - 1)
        {
            if (countdownQuestion > jarakSetiapSoal - 3 && countdownQuestion < jarakSetiapSoal + 1)
            {
                containerCountdownSoal.gameObject.SetActive(true);
                questionImg.gameObject.SetActive(false);
                countdownText.text = (jarakSetiapSoal + 1 - countdownQuestion).ToString();
                return;
            }

        }


        if (changeQuestion)
        {
            if (countdownQuestion > jarakSetiapSoal &&  player.GetComponent<CharacterController>().isGrounded)
            {
                questionId += 1;
                changeQuestion = false;
                for (int i = 0; i < 3; i++)
                {
                    choicesBox[i].gameObject.SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    choicesBox[i].gameObject.SetActive(false);
                }
                questionImg.gameObject.SetActive(false);
                return;
            }
            pilihBentukIsPlay = false;
            bentukIsPlay = false;
        }
        
        if (questionId == banyakSoal)
        {
            for (int i = 0; i < 3; i++)
            {
                choicesBox[i].gameObject.SetActive(false);
            }
            player.GetComponent<ScoreController>().OnDeath();
            return;
        }

        if (!pilihBentukIsPlay)
        {
            guideMusic.playGuideMusic(guideMusic.pilihBentuk);
        }


        if (questionId < banyakSoal && player.GetComponent<ScoreController>().GetQuestionSolved() != banyakSoal)
        {
            questionImg.gameObject.SetActive(true);
            questionImg.GetComponent<Image>().sprite = shapes[quesList[questionId]];
            questionImg.GetComponent<Image>().color = colorValues[quesColor[questionId]];
            for (int i = 0; i < choicesBox[answerList[questionId]].transform.childCount; i++)
            {
                choicesBox[answerList[questionId]].transform.GetChild(i).gameObject.SetActive(false);
            }
            choicesBox[answerList[questionId]].transform.GetChild(quesList[questionId]).gameObject.SetActive(true);
            choicesBox[answerList[questionId]].transform.GetChild(quesList[questionId]).gameObject.GetComponent<Renderer>().material.color = colorValues[quesColor[questionId]];

            if (!pilihBentukIsPlay)
            {
                pilihBentukIsPlay = true;
                guideMusic.playGuideMusic(guideMusic.pilihBentuk);
            }

            if (PlayerPrefs.GetString("gametype") == "MEDIUM")
            {

                if (!bentukIsPlay && pilihBentukIsPlay && !guideMusic.suara.isPlaying)
                {
                    bentukIsPlay = true;
                    if (quesList[questionId] == 0)
                        guideMusic.playGuideMusic(guideMusic.Circle);
                    else if (quesList[questionId] == 1)
                        guideMusic.playGuideMusic(guideMusic.Hexagon);
                    else if (quesList[questionId] == 2)
                        guideMusic.playGuideMusic(guideMusic.Kite);
                    else if (quesList[questionId] == 3)
                        guideMusic.playGuideMusic(guideMusic.Oval);
                    else if (quesList[questionId] == 4)
                        guideMusic.playGuideMusic(guideMusic.Parrallelogram);
                    else if (quesList[questionId] == 5)
                        guideMusic.playGuideMusic(guideMusic.Pentagon);
                    else if (quesList[questionId] == 6)
                        guideMusic.playGuideMusic(guideMusic.Rectangle);
                    else if (quesList[questionId] == 7)
                        guideMusic.playGuideMusic(guideMusic.Rhombus);
                    else if (quesList[questionId] == 8)
                        guideMusic.playGuideMusic(guideMusic.Square);
                    else if (quesList[questionId] == 9)
                        guideMusic.playGuideMusic(guideMusic.Star);
                    else if (quesList[questionId] == 10)
                        guideMusic.playGuideMusic(guideMusic.Triangle);
                }
            }

            else if (PlayerPrefs.GetString("gametype") == "EASY")
            {

                if (!bentukIsPlay && pilihBentukIsPlay && !guideMusic.suara.isPlaying)
                {
                    bentukIsPlay = true;
                    if (quesColor[questionId] == 0)
                    {
                        if (quesList[questionId] == 0)
                            guideMusic.playGuideMusic(guideMusic.redCircle);
                        else if (quesList[questionId] == 1)
                            guideMusic.playGuideMusic(guideMusic.redHexagon);
                        else if (quesList[questionId] == 2)
                            guideMusic.playGuideMusic(guideMusic.redKite);
                        else if (quesList[questionId] == 3)
                            guideMusic.playGuideMusic(guideMusic.redOval);
                        else if (quesList[questionId] == 4)
                            guideMusic.playGuideMusic(guideMusic.redParrallelogram);
                        else if (quesList[questionId] == 5)
                            guideMusic.playGuideMusic(guideMusic.redPentagon);
                        else if (quesList[questionId] == 6)
                            guideMusic.playGuideMusic(guideMusic.redRectangle);
                        else if (quesList[questionId] == 7)
                            guideMusic.playGuideMusic(guideMusic.redRhombus);
                        else if (quesList[questionId] == 8)
                            guideMusic.playGuideMusic(guideMusic.redSquare);
                        else if (quesList[questionId] == 9)
                            guideMusic.playGuideMusic(guideMusic.redStar);
                        else if (quesList[questionId] == 10)
                            guideMusic.playGuideMusic(guideMusic.redTriangle);
                    }
                    else if (quesColor[questionId] == 1)
                    {
                        if (quesList[questionId] == 0)
                            guideMusic.playGuideMusic(guideMusic.greenCircle);
                        else if (quesList[questionId] == 1)
                            guideMusic.playGuideMusic(guideMusic.greenHexagon);
                        else if (quesList[questionId] == 2)
                            guideMusic.playGuideMusic(guideMusic.greenKite);
                        else if (quesList[questionId] == 3)
                            guideMusic.playGuideMusic(guideMusic.greenOval);
                        else if (quesList[questionId] == 4)
                            guideMusic.playGuideMusic(guideMusic.greenParrallelogram);
                        else if (quesList[questionId] == 5)
                            guideMusic.playGuideMusic(guideMusic.greenPentagon);
                        else if (quesList[questionId] == 6)
                            guideMusic.playGuideMusic(guideMusic.greenRectangle);
                        else if (quesList[questionId] == 7)
                            guideMusic.playGuideMusic(guideMusic.greenRhombus);
                        else if (quesList[questionId] == 8)
                            guideMusic.playGuideMusic(guideMusic.greenSquare);
                        else if (quesList[questionId] == 9)
                            guideMusic.playGuideMusic(guideMusic.greenStar);
                        else if (quesList[questionId] == 10)
                            guideMusic.playGuideMusic(guideMusic.greenTriangle);
                    }
                    else if (quesColor[questionId] == 2)
                    {
                        if (quesList[questionId] == 0)
                            guideMusic.playGuideMusic(guideMusic.blueCircle);
                        else if (quesList[questionId] == 1)
                            guideMusic.playGuideMusic(guideMusic.blueHexagon);
                        else if (quesList[questionId] == 2)
                            guideMusic.playGuideMusic(guideMusic.blueKite);
                        else if (quesList[questionId] == 3)
                            guideMusic.playGuideMusic(guideMusic.blueOval);
                        else if (quesList[questionId] == 4)
                            guideMusic.playGuideMusic(guideMusic.blueParrallelogram);
                        else if (quesList[questionId] == 5)
                            guideMusic.playGuideMusic(guideMusic.bluePentagon);
                        else if (quesList[questionId] == 6)
                            guideMusic.playGuideMusic(guideMusic.blueRectangle);
                        else if (quesList[questionId] == 7)
                            guideMusic.playGuideMusic(guideMusic.blueRhombus);
                        else if (quesList[questionId] == 8)
                            guideMusic.playGuideMusic(guideMusic.blueSquare);
                        else if (quesList[questionId] == 9)
                            guideMusic.playGuideMusic(guideMusic.blueStar);
                        else if (quesList[questionId] == 10)
                            guideMusic.playGuideMusic(guideMusic.blueTriangle);
                    }
                    else if (quesColor[questionId] == 3)
                    {
                        if (quesList[questionId] == 0)
                            guideMusic.playGuideMusic(guideMusic.yellowCircle);
                        else if (quesList[questionId] == 1)
                            guideMusic.playGuideMusic(guideMusic.yellowHexagon);
                        else if (quesList[questionId] == 2)
                            guideMusic.playGuideMusic(guideMusic.yellowKite);
                        else if (quesList[questionId] == 3)
                            guideMusic.playGuideMusic(guideMusic.yellowOval);
                        else if (quesList[questionId] == 4)
                            guideMusic.playGuideMusic(guideMusic.yellowParrallelogram);
                        else if (quesList[questionId] == 5)
                            guideMusic.playGuideMusic(guideMusic.yellowPentagon);
                        else if (quesList[questionId] == 6)
                            guideMusic.playGuideMusic(guideMusic.yellowRectangle);
                        else if (quesList[questionId] == 7)
                            guideMusic.playGuideMusic(guideMusic.yellowRhombus);
                        else if (quesList[questionId] == 8)
                            guideMusic.playGuideMusic(guideMusic.yellowSquare);
                        else if (quesList[questionId] == 9)
                            guideMusic.playGuideMusic(guideMusic.yellowStar);
                        else if (quesList[questionId] == 10)
                            guideMusic.playGuideMusic(guideMusic.yellowTriangle);
                    }
                }
            }

            if (PlayerPrefs.GetString("gametype") == "EASY")
            {
                namaBangunText.gameObject.SetActive(true);
                namaBangunText.text = colors[quesColor[questionId]] + " " + namaBangun[quesList[questionId]];
            }
            else
                namaBangunText.text = "";

            if (answerList[questionId] == 0)
            {
                for (int i = 0; i < choicesBox[answerList[questionId]].transform.childCount; i++)
                {
                    choicesBox[1].transform.GetChild(i).gameObject.SetActive(false);
                    choicesBox[2].transform.GetChild(i).gameObject.SetActive(false);
                }
                choicesBox[1].transform.GetChild(shapeChoice1List[questionId]).gameObject.SetActive(true);
                choicesBox[1].transform.GetChild(shapeChoice1List[questionId]).gameObject.GetComponent<Renderer>().material.color = colorValues[shape1ColorList[questionId]];
                choicesBox[2].transform.GetChild(shapeChoice2List[questionId]).gameObject.SetActive(true);
                choicesBox[2].transform.GetChild(shapeChoice2List[questionId]).gameObject.GetComponent<Renderer>().material.color = colorValues[shape2ColorList[questionId]];

            }
            else if (answerList[questionId] == 1)
            {
                for (int i = 0; i < choicesBox[answerList[questionId]].transform.childCount; i++)
                {
                    choicesBox[0].transform.GetChild(i).gameObject.SetActive(false);
                    choicesBox[2].transform.GetChild(i).gameObject.SetActive(false);
                }
                choicesBox[0].transform.GetChild(shapeChoice1List[questionId]).gameObject.SetActive(true);
                choicesBox[0].transform.GetChild(shapeChoice1List[questionId]).gameObject.GetComponent<Renderer>().material.color = colorValues[shape1ColorList[questionId]];
                choicesBox[2].transform.GetChild(shapeChoice2List[questionId]).gameObject.SetActive(true);
                choicesBox[2].transform.GetChild(shapeChoice2List[questionId]).gameObject.GetComponent<Renderer>().material.color = colorValues[shape2ColorList[questionId]];
            }
            else if (answerList[questionId] == 2)
            {
                for (int i = 0; i < choicesBox[answerList[questionId]].transform.childCount; i++)
                {
                    choicesBox[1].transform.GetChild(i).gameObject.SetActive(false);
                    choicesBox[0].transform.GetChild(i).gameObject.SetActive(false);
                }
                choicesBox[1].transform.GetChild(shapeChoice1List[questionId]).gameObject.SetActive(true);
                choicesBox[1].transform.GetChild(shapeChoice1List[questionId]).gameObject.GetComponent<Renderer>().material.color = colorValues[shape1ColorList[questionId]];
                choicesBox[0].transform.GetChild(shapeChoice2List[questionId]).gameObject.SetActive(true);
                choicesBox[0].transform.GetChild(shapeChoice2List[questionId]).gameObject.GetComponent<Renderer>().material.color = colorValues[shape2ColorList[questionId]];
            }
        }
    }

    private void StartGameDatabase()
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/rundomdb.db";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();
        DateTime dateNow = DateTime.Now;
        string idRecord = String.Format("{0:D4}{1:D2}{2:D2}{3:D2}{4:D2}{5:D2}", dateNow.Year, dateNow.Month, dateNow.Day, dateNow.Hour, dateNow.Minute, dateNow.Second);
        PlayerPrefs.SetString("idGameRecord", idRecord);
        string sqlQuery = string.Format("INSERT INTO GameRecord VALUES ({12}, '{0}', '{1}', {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, '{13}', '{14}', '{15}', '{16}', '{17}')", idRecord, PlayerPrefs.GetString("gametype"), quesList[0], quesList[1], quesList[2], 0, 0, answerList[0], answerList[1], answerList[2], 0, 0, PlayerPrefs.GetInt("id"), colors[quesColor[0]], colors[quesColor[1]], colors[quesColor[2]], '0', '0');

        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteNonQuery();

        string sqlQuery2 = string.Format("INSERT INTO GameRecordTrue VALUES ('{0}', {1}, {2}, {3}, {4}, {5})", idRecord,  0, 0, 0, 0, 0);

        dbcmd.CommandText = sqlQuery2;
        dbcmd.ExecuteNonQuery();

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    private int GetShapeNum()
    {
        int shapeNum = 0;
        shapeNum = Random.Range(0, shapes.Length);
        return shapeNum;
    }

    private int GetRandColor()
    {
        int randColor = 0;
        randColor = Random.Range(0, colors.Length);
        return randColor;
    }

    public void GantiSoal()
    {
        changeQuestion = true;
    }

    public int GetBanyakSoal()
    {
        return banyakSoal;
    }

}
