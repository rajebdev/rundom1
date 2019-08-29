using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mono.Data.Sqlite;
using System;
using System.Data;

public class MenuController : MonoBehaviour
{
    
    public Image quitImg;
    public Image settImg;
    public Image helpImg;

    private bool isQuitShowned = false;
    private bool isSettShowned = false;
    private bool isInputShowned = false;
    private bool isLeaderShowned = false;
    private bool isHelpShowned = false;

    private float transition;

    public GameObject inputContainer;
    public InputField nameText;
    public GameObject maleCek;
    public GameObject femaleCek;

    private bool genderMale;
    public GameObject addPlayerContainer;

    public GameObject listContainer;
    public GameObject detailPlayer;
    public GameObject contentView;

    // Leader Board
    public GameObject leaderBoard;
    public GameObject leaderBoardList;
    public GameObject contentLeader;
    public GameObject detailLeader;

    // Leader Board Detail
    public GameObject detailBoard;
    public Text nameLeaderDetail;
    public GameObject detailLeaderPref;
    public GameObject contentDetail;

    // Info Detail Record
    public GameObject infoDetail;
    public Text recordText;
    public GameObject InfoDetailRecordPref;
    public GameObject contentInfoDetil;

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

    // Sound Effect Object
    public GameObject buttonSoundEffect;

    //help Video
    public VideoStreamController videoPlayer;


    // Start is called before the first frame update
    void Start()
    {
        quitImg.gameObject.SetActive(false);
        settImg.gameObject.SetActive(false);
        PlayerPrefs.DeleteKey("id");
        PlayerPrefs.DeleteKey("nama");
        PlayerPrefs.DeleteKey("gender");
        CreateListPlayer();
        CreateLeaderBoardList();
    }

    // Update is called once per frame
    void Update()
    {
        if (isLeaderShowned)
        {
            transition += Time.deltaTime * 2;
            leaderBoard.GetComponent<Image>().color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 0.5f), transition);
        }

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

        if (isHelpShowned)
        {
            transition += Time.deltaTime * 2;
            helpImg.GetComponent<Image>().color = Color.Lerp(new Color(0, 0, 0, 0), new Color(0, 0, 0, 0.5f), transition);
        }
    }

    public void OnPlayPlayer(int id, string namePlayer, string gender)
    {
        ButtonClick();
        PlayerPrefs.SetInt("id", id);
        PlayerPrefs.SetString("nama", namePlayer);
        PlayerPrefs.SetString("gender", gender);
        SceneManager.LoadScene("SubMenuGame");
    }

    private void CreateListPlayer()
    {
        foreach (Transform child in contentView.transform)
        {
            if (child.gameObject.name != "ListDetailContainer")
                GameObject.Destroy(child.gameObject);
        }

        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/rundomdb.db";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT * FROM player";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        int i = 0;
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            string gender = reader.GetString(2);
            GameObject tempPlayer = Instantiate(detailPlayer) as GameObject;
            tempPlayer.transform.position = new Vector3(0, -5 - 35 * i, 0);
            tempPlayer.transform.GetChild(0).gameObject.GetComponent<Text>().text = name;
            tempPlayer.transform.SetParent(contentView.transform, false);
            tempPlayer.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(delegate { OnClickDeletePlayer(id); });
            tempPlayer.transform.GetChild(2).GetComponent<Button>().onClick.AddListener(delegate { OnPlayPlayer(id, tempPlayer.transform.GetChild(0).gameObject.GetComponent<Text>().text, gender); });
            tempPlayer.name = i.ToString() + " DetailPlayer";
            tempPlayer.SetActive(true);
            i++;
        }

        contentView.GetComponent<RectTransform>().sizeDelta = new Vector2(contentView.GetComponent<RectTransform>().sizeDelta.x, 35 * i + 10);

        reader.Close();
        reader = null;
        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    private int AddPlayerDatabase(string name, string gender)
    {
        int id = 0;
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/rundomdb.db";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open();
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "INSERT INTO player (nama, gender, score) VALUES ('" + name + "', '" + gender + "', 0);";
        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteNonQuery();

        // get id
        dbcmd.CommandText = "SELECT last_insert_rowid()";
        Int64 LastRowID64 = (Int64)dbcmd.ExecuteScalar();
        id = (int)LastRowID64;

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

        return id;
    }

    private void OnClickDeletePlayer(int id)
    {
        ButtonClick();
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/rundomdb.db";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        List<string> idRecordPlayers = new List<string>();
        string sqlQuery0 = string.Format("SELECT * FROM GameRecord WHERE id = {0}", id);
        dbcmd.CommandText = sqlQuery0;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            string idRecordTemp = reader.GetString(1);
            idRecordPlayers.Add(idRecordTemp);
        }
        reader.Close();
        reader = null;
        for (int i = 0; i < idRecordPlayers.Count; i++)
        {
            string sqlQuery = "DELETE FROM GameDetail WHERE idGameRecord = '" + idRecordPlayers[i] + "'";
            dbcmd.CommandText = sqlQuery;
            dbcmd.ExecuteNonQuery();

            string sqlQuery2 = "DELETE FROM GameRecordHit WHERE idGameRecord = '" + idRecordPlayers[i] + "'";
            dbcmd.CommandText = sqlQuery2;
            dbcmd.ExecuteNonQuery();

            string sqlQuery3 = "DELETE FROM GameRecordTrue WHERE idGameRecord = '" + idRecordPlayers[i] + "'";
            dbcmd.CommandText = sqlQuery3;
            dbcmd.ExecuteNonQuery();

            string sqlQuery4 = "DELETE FROM GameRecord WHERE idGameRecord = '" + idRecordPlayers[i] + "'";
            dbcmd.CommandText = sqlQuery4;
            dbcmd.ExecuteNonQuery();
        }

        string sqlQuery5 = "DELETE FROM player WHERE id = " + id;
        dbcmd.CommandText = sqlQuery5;
        dbcmd.ExecuteNonQuery();

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
        CreateListPlayer();
    }

    private void CreateLeaderBoardList()
    {
        foreach (Transform child in contentLeader.transform)
        {
            if (child.gameObject.name != "ListDetailContainer")
                GameObject.Destroy(child.gameObject);
        }
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/rundomdb.db";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT * FROM player ORDER BY score DESC";
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        List<int> idPlayer = new List<int>();
        List<GameObject> goPlayer = new List<GameObject>();
        int i = 0;
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            string name = reader.GetString(1);
            string gender = reader.GetString(2);
            int score = reader.GetInt32(3);
            GameObject tempPlayer = Instantiate(detailLeader) as GameObject;
            tempPlayer.transform.position = new Vector3(0, -5 - 35 * i, 0);
            tempPlayer.transform.GetChild(0).gameObject.GetComponent<Text>().text = (i+1).ToString();
            tempPlayer.transform.GetChild(1).gameObject.GetComponent<Text>().text = name;
            tempPlayer.transform.GetChild(2).gameObject.GetComponent<Text>().text = score.ToString();
            tempPlayer.transform.GetChild(3).GetComponent<Button>().onClick.AddListener(delegate { OnClickDetail(id); });
            tempPlayer.transform.SetParent(contentLeader.transform, false);
            tempPlayer.name = i.ToString() + " DetailLeader";
            tempPlayer.SetActive(true);

            idPlayer.Add(id);
            goPlayer.Add(tempPlayer);
            i++;
        }

        contentLeader.GetComponent<RectTransform>().sizeDelta = new Vector2(contentLeader.GetComponent<RectTransform>().sizeDelta.x, 35 * i + 10);

        reader.Close();
        reader = null;

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    private void OnClickDetail(int id)
    {

        ButtonClick();
        detailBoard.SetActive(true);
        leaderBoardList.SetActive(false);
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/rundomdb.db";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "SELECT * FROM player WHERE id = "+id.ToString();
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        while (reader.Read())
        {
            nameLeaderDetail.text = reader["nama"].ToString();
        }
        reader.Close();
        reader = null;

        string sqlQuery2 = "SELECT * FROM GameRecord WHERE id = " + id.ToString();
        dbcmd.CommandText = sqlQuery2;
        IDataReader reader2 = dbcmd.ExecuteReader();
        foreach (Transform child in contentDetail.transform)
        {
            if (child.gameObject.name != "DetailRecord")
                GameObject.Destroy(child.gameObject);
        }
        List<string> idRecords = new List<string>();
        List<GameObject> goRecord = new List<GameObject>();
        int i = 0;
        while (reader2.Read())
        {
            int idP = reader2.GetInt32(0);
            string idRecord = reader2["idGameRecord"].ToString();
            string gameType = reader2["gametype"].ToString();
            GameObject tempRecord = Instantiate(detailLeaderPref) as GameObject;
            tempRecord.transform.position = new Vector3(0, -5 - 35 * i, 0);
            tempRecord.transform.GetChild(0).gameObject.GetComponent<Text>().text = idRecord;
            tempRecord.transform.GetChild(1).gameObject.GetComponent<Text>().text = gameType;
            tempRecord.transform.GetChild(6).GetComponent<Button>().onClick.AddListener(delegate { OnClickInfoDetail(idRecord); });
            tempRecord.transform.GetChild(7).GetComponent<Button>().onClick.AddListener(delegate { OnClickDeleteRecord(idRecord, idP); });
            tempRecord.transform.SetParent(contentDetail.transform, false);
            tempRecord.name = i.ToString() + " DetailLeader";
            tempRecord.SetActive(true);
            idRecords.Add(idRecord);
            goRecord.Add(tempRecord);
            i++;
        }

        contentDetail.GetComponent<RectTransform>().sizeDelta = new Vector2(contentDetail.GetComponent<RectTransform>().sizeDelta.x, 35 * i + 10);

        reader2.Close();
        reader2 = null;

        int y = 0;
        while (y < idRecords.Count)
        {
            string sqlQuery3 = "SELECT * FROM GameDetail WHERE idGameRecord = " + idRecords[y].ToString();
            dbcmd.CommandText = sqlQuery3;
            IDataReader reader3 = dbcmd.ExecuteReader();
            while (reader3.Read())
            {
                int benar = reader3.GetInt32(1);
                int salah = reader3.GetInt32(2);
                int score = reader3.GetInt32(3);
                int time = reader3.GetInt32(4);
                goRecord[y].transform.GetChild(2).GetComponent<Text>().text = benar.ToString();
                goRecord[y].transform.GetChild(3).GetComponent<Text>().text = salah.ToString();
                goRecord[y].transform.GetChild(4).GetComponent<Text>().text = score.ToString();
                goRecord[y].transform.GetChild(5).GetComponent<Text>().text = string.Format("{0:00}:{1:00}",(int)(time/60), (int)(time%60));
            }

            reader3.Close();
            reader3 = null;
            y++;
        }

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
    }

    private void OnClickInfoDetail(string idRecord)
    {
        ButtonClick();
        foreach (Transform child in contentInfoDetil.transform)
        {
            if (child.gameObject.name != "InfoDetailRecord")
                GameObject.Destroy(child.gameObject);
        }
        infoDetail.SetActive(true);
        detailBoard.SetActive(false);
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/rundomdb.db";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();

        recordText.text = idRecord;

        string sqlQuery2 = "SELECT * FROM GameRecordHit WHERE idGameRecord = " + idRecord;
        dbcmd.CommandText = sqlQuery2;
        IDataReader reader2 = dbcmd.ExecuteReader();
        List<int> idSoal = new List<int>();
        List<GameObject> goRecord = new List<GameObject>();
        int j = 0;
        while (reader2.Read())
        {
            int soalId = reader2.GetInt32(1);
            bool kondisi = reader2.GetBoolean(3);
            int poin = reader2.GetInt32(4);
            int shapeAnswer = reader2.GetInt32(5);
            string colorAnswer = reader2.GetString(6);
            int timeHit = reader2.GetInt32(7);
            GameObject tempRecord = Instantiate(InfoDetailRecordPref) as GameObject;
            tempRecord.transform.position = new Vector3(0, -5 - 35 * j, 0);
            tempRecord.transform.GetChild(0).gameObject.GetComponent<Text>().text = (j+1).ToString();
            tempRecord.transform.GetChild(2).gameObject.GetComponent<Text>().text = colorAnswer+" "+namaBangun[shapeAnswer];
            tempRecord.transform.GetChild(3).gameObject.GetComponent<Text>().text = kondisi ? "BENAR" : "SALAH";
            tempRecord.transform.GetChild(4).gameObject.GetComponent<Text>().text = poin.ToString();
            tempRecord.transform.GetChild(5).gameObject.GetComponent<Text>().text = string.Format("{0:00}:{1:00}",(int)(timeHit/60),(int)(timeHit%60));
            tempRecord.transform.SetParent(contentInfoDetil.transform, false);
            tempRecord.name = j.ToString() + " InfoDetailRecord";
            tempRecord.SetActive(true);
            idSoal.Add(soalId);
            goRecord.Add(tempRecord);
            j++;
        }

        contentInfoDetil.GetComponent<RectTransform>().sizeDelta = new Vector2(contentInfoDetil.GetComponent<RectTransform>().sizeDelta.x, 35 * j + 10);

        reader2.Close();
        reader2 = null;

        string sqlQuery = "SELECT * FROM GameRecord WHERE idGameRecord = " + idRecord;
        dbcmd.CommandText = sqlQuery;
        IDataReader reader = dbcmd.ExecuteReader();
        int i = 0;
        while (reader.Read())
        {
            int idP = reader.GetInt32(0);
            string idGameRecord = reader["idGameRecord"].ToString();
            int questShape1 = reader.GetInt32(3);
            int questShape2 = reader.GetInt32(4);
            int questShape3 = reader.GetInt32(5);
            int questShape4 = reader.GetInt32(6);
            int questShape5 = reader.GetInt32(7);

            string questColor1 = reader.GetString(13);
            string questColor2 = reader.GetString(14);
            string questColor3 = reader.GetString(15);
            string questColor4 = reader.GetString(16);
            string questColor5 = reader.GetString(17);
            for (int y = 0; y < goRecord.Count; y++)
            {
                if (idSoal[y] == 0)
                    goRecord[y].transform.GetChild(1).GetComponent<Text>().text = questColor1 + " " + namaBangun[questShape1];
                else if (idSoal[y] == 1)
                    goRecord[y].transform.GetChild(1).GetComponent<Text>().text = questColor2 +" "+namaBangun[questShape2];
                else if (idSoal[y] == 2)
                    goRecord[y].transform.GetChild(1).GetComponent<Text>().text = questColor3 + " " + namaBangun[questShape3];
                else if (idSoal[y] == 3)
                    goRecord[y].transform.GetChild(1).GetComponent<Text>().text = questColor4 + " " + namaBangun[questShape4];
                else if (idSoal[y] == 4)
                    goRecord[y].transform.GetChild(1).GetComponent<Text>().text = questColor5 + " " + namaBangun[questShape5];
            }
            i++;
        }

        reader.Close();
        reader = null;

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;

    }

    private void OnClickDeleteRecord(string idRecord, int idp)
    {
        ButtonClick();
        string conn = "URI=file:" + Application.dataPath + "/StreamingAssets/rundomdb.db";
        IDbConnection dbconn;
        dbconn = (IDbConnection)new SqliteConnection(conn);
        dbconn.Open(); //Open connection to the database.
        IDbCommand dbcmd = dbconn.CreateCommand();
        string sqlQuery = "DELETE FROM GameDetail WHERE idGameRecord = '"+idRecord+"'";
        dbcmd.CommandText = sqlQuery;
        dbcmd.ExecuteNonQuery();
        
        string sqlQuery2 = "DELETE FROM GameRecordHit WHERE idGameRecord = '"+idRecord+"'";
        dbcmd.CommandText = sqlQuery2;
        dbcmd.ExecuteNonQuery();

        string sqlQuery3 = "DELETE FROM GameRecordTrue WHERE idGameRecord = '" + idRecord + "'";
        dbcmd.CommandText = sqlQuery3;
        dbcmd.ExecuteNonQuery();
        
        string sqlQuery4 = "DELETE FROM GameRecord WHERE idGameRecord = '" + idRecord + "'";
        dbcmd.CommandText = sqlQuery4;
        dbcmd.ExecuteNonQuery();

        int score = 0;
        string sqlQuery5 = string.Format("SELECT SUM(Score) as 'score' FROM GameDetail WHERE idGameRecord IN (SELECT idGameRecord FROM GameRecord WHERE id = {0})", idp);
        dbcmd.CommandText = sqlQuery5;
        IDataReader scoreReader = dbcmd.ExecuteReader();
        while (scoreReader.Read())
        {
            score = int.Parse(scoreReader["score"].ToString() == "" ? "0" : scoreReader["score"].ToString());
        }
        scoreReader.Close();
        scoreReader = null;

        string sqlQuery6 = string.Format("UPDATE player SET score = {0} WHERE id = {1}", score, idp);

        dbcmd.CommandText = sqlQuery6;
        dbcmd.ExecuteNonQuery();

        dbcmd.Dispose();
        dbcmd = null;
        dbconn.Close();
        dbconn = null;
        OnClickDetail(idp);
    }

    public void OnClickCloseInfoDetail()
    {
        ButtonClick();
        infoDetail.SetActive(false);
        detailBoard.SetActive(true);
    }

    public void OnCloseDetailLeaderBoard()
    {
        ButtonClick();
        detailBoard.SetActive(false);
        leaderBoardList.SetActive(true);
        CreateLeaderBoardList();
    }

    public void OnClickLeaderBoard()
    {
        ButtonClick();
        CreateLeaderBoardList();
        leaderBoard.SetActive(true);
        isLeaderShowned = true;
        transition = 0.0f;
    }

    public void OnClickCloseLeaderBoard()
    {
        ButtonClick();
        leaderBoard.SetActive(false);
        isLeaderShowned = false;
        transition = 0.0f;
    }

    public void ToSubmit()
    {
        ButtonClick();
        string gender = "";
        if (genderMale)
            gender = "L";
        else
            gender = "P";
        int id = AddPlayerDatabase(nameText.text, gender);
        PlayerPrefs.SetInt("id", id);
        PlayerPrefs.SetString("nama", nameText.text);
        PlayerPrefs.SetString("gender", gender);
        SceneManager.LoadScene("SubMenuGame");
    }

    public void ToPlay()
    {
        ButtonClick();
        inputContainer.SetActive(true);
        isInputShowned = true;
        transition = 0.0f;
    }

    public void ToAddPlayer()
    {
        ButtonClick();
        listContainer.SetActive(false);
        addPlayerContainer.SetActive(true);
    }

    public void ToCancelAddPlayer()
    {
        listContainer.SetActive(true);
        addPlayerContainer.SetActive(false);
    }

    public void OnCancelPlay()
    {
        ButtonClick();
        inputContainer.SetActive(false);
        isInputShowned = false;
        transition = 0.0f;
        maleCek.SetActive(false);
        femaleCek.SetActive(false);
    }

    public void ToClearText()
    {
        ButtonClick();
        nameText.Select();
        nameText.text = "";
    }

    public void ToMale()
    {
        ButtonClick();
        genderMale = true;
        maleCek.SetActive(true);
        femaleCek.SetActive(false);
    }

    public void ToFemale()
    {
        ButtonClick();
        genderMale = false;
        femaleCek.SetActive(true);
        maleCek.SetActive(false);
    }

    public void OnClickQuit()
    {
        ButtonClick();
        quitImg.gameObject.SetActive(true);
        isQuitShowned = true;
    }

    public void OnClickQuitNo()
    {
        ButtonClick();
        quitImg.gameObject.SetActive(false);
        isQuitShowned = false;
        transition = 0.0f;
    }

    public void OnClickQuitYes()
    {
        ButtonClick();
        Application.Quit();
    }

    public void OnClickSett()
    {
        ButtonClick();
        settImg.gameObject.SetActive(true);
        isSettShowned = true;
    }

    public void OnClickSettClose()
    {
        ButtonClick();
        settImg.gameObject.SetActive(false);
        isSettShowned = false;
        transition = 0.0f;
    }

    public void OnClickHelp()
    {
        ButtonClick();
        helpImg.gameObject.SetActive(true);
        isHelpShowned = true;
        StartCoroutine(videoPlayer.PlayVideo());
    }

    public void OnClickhelpClose()
    {
        videoPlayer.videoPlayer.Stop();
        ButtonClick();
        helpImg.gameObject.SetActive(false);
        isHelpShowned = false;
        transition = 0.0f;
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
