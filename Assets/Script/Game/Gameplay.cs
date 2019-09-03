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

    public Image questionImg;
    public GameObject[] choicesBox;
    public GameObject[] choicesImg;

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

    // Start is called before the first frame update
    void Start()
    {
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
            rend.material.color = Color.green;
        else
            rend.material.color = Color.red;
        if (PlayerPrefs.GetString("gametype") == "EASY")
        {
            for (int i = 0; i < banyakSoal; i++)
            {
                int shapeQ = GetShapeNum();
                while (quesList.Contains(shapeQ))
                {
                    shapeQ = GetShapeNum();
                }
                quesList[i] = shapeQ;

                int answer = Random.Range(0, choicesImg.Length);
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

                int answer = Random.Range(0, choicesImg.Length);
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
                    choicesImg[i].gameObject.SetActive(true);
                    choicesBox[i].gameObject.SetActive(true);
                }
            }
            else
            {
                for (int i = 0; i < 3; i++)
                {
                    choicesImg[i].gameObject.SetActive(false);
                    choicesBox[i].gameObject.SetActive(false);
                }

                questionImg.gameObject.SetActive(false);
                return;
            }
        }
        
        if (questionId == banyakSoal)
        {
            for (int i = 0; i < 3; i++)
            {
                choicesImg[i].gameObject.SetActive(false);
                choicesBox[i].gameObject.SetActive(false);
            }
            player.GetComponent<ScoreController>().OnDeath();
            return;
        }


        if (questionId < banyakSoal && player.GetComponent<ScoreController>().GetQuestionSolved() != banyakSoal)
        {
            questionImg.gameObject.SetActive(true);
            questionImg.GetComponent<Image>().sprite = shapes[quesList[questionId]];
            questionImg.GetComponent<Image>().color = colorValues[quesColor[questionId]];
            choicesImg[answerList[questionId]].GetComponent<SpriteRenderer>().sprite = shapes[quesList[questionId]];
            choicesImg[answerList[questionId]].GetComponent<SpriteRenderer>().color = colorValues[quesColor[questionId]];
            if (PlayerPrefs.GetString("gametype") == "MEDIUM")
                namaBangunText.text = colors[quesColor[questionId]] + " " + namaBangun[quesList[questionId]];
            else
                namaBangunText.text = "";

            if (answerList[questionId] == 0)
            {
                choicesImg[1].GetComponent<SpriteRenderer>().sprite = shapes[shapeChoice1List[questionId]];
                choicesImg[2].GetComponent<SpriteRenderer>().sprite = shapes[shapeChoice2List[questionId]];
                choicesImg[1].GetComponent<SpriteRenderer>().color = colorValues[shape1ColorList[questionId]];
                choicesImg[2].GetComponent<SpriteRenderer>().color = colorValues[shape2ColorList[questionId]];

            }
            else if (answerList[questionId] == 1)
            {
                choicesImg[0].GetComponent<SpriteRenderer>().sprite = shapes[shapeChoice1List[questionId]];
                choicesImg[2].GetComponent<SpriteRenderer>().sprite = shapes[shapeChoice2List[questionId]];
                choicesImg[0].GetComponent<SpriteRenderer>().color = colorValues[shape1ColorList[questionId]];
                choicesImg[2].GetComponent<SpriteRenderer>().color = colorValues[shape2ColorList[questionId]];
            }
            else if (answerList[questionId] == 2)
            {
                choicesImg[0].GetComponent<SpriteRenderer>().sprite = shapes[shapeChoice1List[questionId]];
                choicesImg[1].GetComponent<SpriteRenderer>().sprite = shapes[shapeChoice2List[questionId]];
                choicesImg[0].GetComponent<SpriteRenderer>().color = colorValues[shape1ColorList[questionId]];
                choicesImg[1].GetComponent<SpriteRenderer>().color = colorValues[shape2ColorList[questionId]];
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
        string idRecord = dateNow.Year.ToString() + dateNow.Month.ToString() + dateNow.Day.ToString() + dateNow.Hour.ToString() + dateNow.Minute.ToString() + dateNow.Second.ToString();
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
