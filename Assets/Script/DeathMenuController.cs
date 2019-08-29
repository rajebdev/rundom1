using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Data;
using System;
using Mono.Data.Sqlite;

public class DeathMenuController : MonoBehaviour
{
    public ScoreController playerScore;
    public Text scoreText;
    public Image backgoundImg;

    private bool isShowned = false;
    private float transition = 0.0f;

    // Sound Effect Object
    public GameObject buttonSoundEffect;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isShowned) return;

        transition += Time.deltaTime;
        backgoundImg.color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 0.5f), transition);
    }

    public void ToggleEndMenu(float score)
    {
        ButtonClick();
        try
        {
            SaveScoreToDatabase();
        }
        catch (SqliteException)
        {
            Debug.Log("Data Succes Masuk");
        }
        gameObject.SetActive(true);
        scoreText.text = ((int)score).ToString();
        isShowned = true;
    }

    // Play Buttons
    public void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToMenu()
    {
        ButtonClick();
        SceneManager.LoadScene("Menu");
        
        // Playing background menu music
        GameObject soundObject = GameObject.Find("BackgroundSoundMenu");
        if (soundObject != null && PlayerPrefs.GetString("BGM") == "ON")
        {
            AudioSource audioSource = soundObject.GetComponent<AudioSource>();
            audioSource.Play();
        }
    }

    private void SaveScoreToDatabase()
    {
        string conn = "URI=file:" + Application.dataPath + "/Assets_Store/Database/rundomdb.db";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();

        string sqlQuery = string.Format("INSERT INTO GameDetail VALUES ('{0}', {1}, {2}, {3}, {4})", PlayerPrefs.GetString("idGameRecord"), playerScore.GetTruePoint(), playerScore.GetFalsePoint(), playerScore.GetScore(), playerScore.GetTimeClear());

        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteNonQuery();

        int score = 0;
        string sqlQuery2 = string.Format("SELECT SUM(Score) as 'score' FROM GameDetail WHERE idGameRecord IN (SELECT idGameRecord FROM GameRecord WHERE id = {0})", PlayerPrefs.GetInt("id"));
        dbcmd.CommandText = sqlQuery2;
        IDataReader scoreReader = dbcmd.ExecuteReader();
        while (scoreReader.Read())
        {
            score = int.Parse(scoreReader["score"].ToString());
        }
        scoreReader.Close();
        scoreReader = null;

        string sqlQuery3 = string.Format("UPDATE player SET score = {0} WHERE id = {1}", score, PlayerPrefs.GetInt("id"));

        dbcmd.CommandText = sqlQuery3;
        dbcmd.ExecuteNonQuery();

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
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
