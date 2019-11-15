using Mono.Data.Sqlite;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    private float score = 0.0f;

    private int scoreHit;

    private CharacterController user;

    public DeathMenuController deathMenu;
    public Text scoreText;
    public Transform cubeBox;

    private bool isDead = false;
    private bool isColl = false;

    private float animationDuration = 3.0f;
    private float startTime;

    private int difficultyLevel = 1;
    private int maxDifficultyLevel = 3;
    private int scoreToNextLevel = 10;

    // Nilai Contoroller
    public Text benarText;
    public Text salahText;

    private int truePoint;
    private int falsePoint;

    // hit Object Controller
    public GameObject GamePlay;
    private List<GameObject> hitObjects = new List<GameObject>();

    // Notif True False Answer
    public GameObject trueNotif, falseNotif;
    public int timeHitFalse;

    private bool answerCek;

    private int timeClear;

    private int questSolved;

    private string[] colors =
    {
        "RED",
        "GREEN",
        "BLUE",
        "YELLOW"
    };

    // guide music
    private GuideMusicController guideMusic;

    // Start is called before the first frame update
    void Start()
    {
        guideMusic = GameObject.Find("GuideMusic").GetComponent<GuideMusicController>();
        user = GetComponent<CharacterController>();
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {

        // untuk mematikan sentuhan
        if (user.isGrounded)
            isColl = false;

        //Dead Checker
        if (isDead) return;

        // Disable Score During Animation Camera
        if (Time.time - startTime < animationDuration)
            return;
        if ((GetComponent<PlayerController>().secTime - timeHitFalse < 4 || GetComponent<PlayerController>().secTime - GamePlay.GetComponent<Gameplay>().timeHit < 4) && (timeHitFalse != 0 || GamePlay.GetComponent<Gameplay>().timeHit != 0))
        {
            GamePlay.GetComponent<Gameplay>().namaBangunText.gameObject.SetActive(false);
            GamePlay.GetComponent<Gameplay>().questionImg.gameObject.SetActive(false);
        }
        //else
        //{
        //    GamePlay.GetComponent<Gameplay>().namaBangunText.gameObject.SetActive(true);
        //    GamePlay.GetComponent<Gameplay>().questionImg.gameObject.SetActive(true);
        //}

        if (GetComponent<PlayerController>().secTime-timeHitFalse > 3)
        {
            falseNotif.SetActive(false);
        }
        if (GetComponent<PlayerController>().secTime - GamePlay.GetComponent<Gameplay>().timeHit > 3)
        {
            trueNotif.SetActive(false);
        }

        if (score >= scoreToNextLevel)
            LevelUp();
        scoreText.text = ((int) score).ToString();
    }

    private void LevelUp()
    {
        if (difficultyLevel == maxDifficultyLevel)
            return;

        scoreToNextLevel *= 2;
        difficultyLevel++;
        GetComponent<PlayerController>().SetSpeed(difficultyLevel);
    }

    public void OnDeath()
    {
        isDead = true;
        deathMenu.ToggleEndMenu(score);
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Mengecek  Sundulan
        if ((hit.point.y > transform.position.y + user.radius || Input.GetKeyDown(KeyCode.K)) && !isColl)
        {
            if (hit.gameObject.name.Substring(0, 6) == "Choice")
            {
                string hitObjectName = hit.gameObject.name;
                int id = GamePlay.GetComponent<Gameplay>().questionId;
                int answer = GamePlay.GetComponent<Gameplay>().answerList[id];
                if (hit.gameObject.name.Substring(0, 10) == string.Format("Choice ({0})",answer+1))
                {
                    if (PlayerPrefs.GetString("gametype") == "EASY")
                    {
                        scoreHit = 10;
                        score += 10.0f;
                    }
                    else
                    {
                        scoreHit = 20;
                        score += 20.0f;
                    }
                    truePoint++;
                    benarText.GetComponent<Text>().text = truePoint.ToString();
                    for (int i=0; i < hitObjects.Count; i++)
                    {
                        hitObjects[i].SetActive(true);
                    }
                    GamePlay.GetComponent<Gameplay>().GantiSoal();
                    GamePlay.GetComponent<Gameplay>().timeHit = GetComponent<PlayerController>().secTime;
                    trueNotif.SetActive(true);
                    guideMusic.playGuideMusic(guideMusic.jawabBenar);
                    answerCek = true;
                    questSolved += 1;
                    SaveTimeSolvedQuestion(id);
                }
                else
                {
                    scoreHit = -2;
                    score -= 2.0f;
                    falsePoint++;
                    salahText.GetComponent<Text>().text = falsePoint.ToString();
                    //GamePlay.GetComponent<Gameplay>().choicesImg[int.Parse(hit.gameObject.name.Substring(8, 1)) - 1].gameObject.SetActive(false);
                    hit.gameObject.SetActive(false);
                    timeHitFalse = GetComponent<PlayerController>().secTime;
                    falseNotif.SetActive(true);
                    guideMusic.playGuideMusic(guideMusic.jawabSalah);
                    answerCek = false;
                }
                
                if (id == GamePlay.GetComponent<Gameplay>().GetBanyakSoal()-1)
                {
                    timeClear = GetComponent<PlayerController>().secTime;
                }

                int shapeAns = GamePlay.GetComponent<Gameplay>().quesList[id];
                string colorAns = colors[GamePlay.GetComponent<Gameplay>().quesColor[id]];
                if (!answerCek)
                {
                    if (answer == 1)
                    {
                        if (int.Parse(hit.gameObject.name.Substring(8, 1)) - 1 == 2)
                        {
                            shapeAns = GamePlay.GetComponent<Gameplay>().shapeChoice1List[id];
                            colorAns = colors[GamePlay.GetComponent<Gameplay>().shape1ColorList[id]];
                        }

                        else
                        {
                            shapeAns = GamePlay.GetComponent<Gameplay>().shapeChoice2List[id];
                            colorAns = colors[GamePlay.GetComponent<Gameplay>().shape2ColorList[id]];
                        }
                    }
                    else
                    {
                        if (int.Parse(hit.gameObject.name.Substring(8, 1)) - 1 == 1)
                        {
                            shapeAns = GamePlay.GetComponent<Gameplay>().shapeChoice1List[id];
                            colorAns = colors[GamePlay.GetComponent<Gameplay>().shape1ColorList[id]];
                        }

                        else
                        {
                            shapeAns = GamePlay.GetComponent<Gameplay>().shapeChoice2List[id];
                            colorAns = colors[GamePlay.GetComponent<Gameplay>().shape2ColorList[id]];
                        }
                    }
                }


                SaveHitRecordToDatabase(id, int.Parse(hit.gameObject.name.Substring(8, 1)) - 1, answerCek, scoreHit, shapeAns, colorAns, GetComponent<PlayerController>().secTime);

                isColl = true;
                hitObjects.Add(hit.gameObject);
            }
        }
    }

    public int GetTruePoint()
    {
        return truePoint;
    }

    public int GetFalsePoint()
    {
        return falsePoint;
    }

    public int GetScore()
    {
        return (int)score;
    }
    
    public int GetTimeClear()
    {
        return timeClear;
    }

    private void SaveHitRecordToDatabase(int qId, int answerHit, bool cond, int score, int shapeAns, string colorAns, int timeH)
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/rundomdb.db";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = string.Format("INSERT INTO GameRecordHit VALUES ('{0}', {1}, {2}, {3}, {4}, {5}, '{6}', {7})", PlayerPrefs.GetString("idGameRecord"), qId, answerHit, cond, score, shapeAns, colorAns, timeH);

        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteNonQuery();

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }
    private void SaveTimeSolvedQuestion(int qId)
    {
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/rundomdb.db";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = string.Format("UPDATE GameRecordTrue SET time{0} = {1} WHERE idGameRecord = {2}", qId+1, GetComponent<PlayerController>().secTime, PlayerPrefs.GetString("idGameRecord"));

        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteNonQuery();

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    public int GetQuestionSolved()
    {
        return questSolved;
    }
    
}
