using Mono.Data.Sqlite;
using System.Collections.Generic;
using System.Data;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class GrafikController : MonoBehaviour
{
    // inisialisasi
    public Text namaPlayer, levelPlayer;
    private float xRange, yRange;
    private List<GameObject> pointList;
    public GameObject graphUI;
    public GameObject XminLevel, XmaxLevel, YminLevel, YmaxLevel;
    
    private float scoreMax, waktuMax;
    private float xSpacing, ySpacing;

    public GameObject graphChoice;
    public GameObject samplePoint;

    public List<Text> levelx, levely;

    private int ID;

    // Start is called before the first frame update
    void Start()
    {
        graphUI.SetActive(false);
        graphChoice.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Get_user(int id)
    {
        string connection = "URI=file:" + Application.dataPath + "/StreamingAssets/rundomdb.db";
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;

        string query = "SELECT * FROM player" +
                       " WHERE id ='" + id.ToString() + "'";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();

        while (reader.Read())
        {
            name = reader[1].ToString();
            namaPlayer.text = name;
        }
    }

    private void Get_ScoreWaktuMax()
    {
        string connection = "URI=file:" + Application.dataPath + "/StreamingAssets/rundomdb.db";
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = "SELECT max(Score), max(Time) FROM GameDetail WHERE idGameRecord in (SELECT idGameRecord FROM GameRecord WHERE id=" + ID.ToString() + ")";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();
        while (reader.Read())
        {
            //Debug.Log("MAX SCORE" + reader[0].ToString());
            //Debug.Log("MAX WAKTU" + reader[1].ToString());
            scoreMax = int.Parse(reader[0].ToString());
            waktuMax = int.Parse(reader[1].ToString());
        }
        dbcon.Close();
    }

    private void CreateGraph(string level)
    {
        xRange = XmaxLevel.transform.position.x - XminLevel.transform.position.x;
        yRange = YmaxLevel.transform.position.y - YminLevel.transform.position.y;
        //Debug.Log("X : " + xRange + " -- " + "Y :" + yRange);
        pointList = new List<GameObject>();
        //Debug.Log("Xmin "+XminLevel.transform.position);
        //Debug.Log("XMax "+XmaxLevel.transform.position);


        xSpacing = xRange / scoreMax;
        ySpacing = yRange / waktuMax;

        //Debug.Log("Spacing " + xSpacing.ToString()+ " " + ySpacing.ToString());

        float xLevelVal = (int)(scoreMax / levelx.Count);
        float yLevelVal = (int)(waktuMax / levely.Count);
        for (int i = 0; i < levelx.Count; i++)
        {
            levelx[i].text = xLevelVal.ToString();
            levely[i].text = yLevelVal.ToString();
            xLevelVal += (int)(scoreMax / levelx.Count);
            yLevelVal += (int)(waktuMax / levely.Count);
        }


        string connection = "URI=file:" + Application.dataPath + "/StreamingAssets/rundomdb.db";
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;
        string query = "SELECT * FROM GameRecord r, GameDetail d WHERE r.idGameRecord=d.idGameRecord AND r.gametype='" + level +"' AND r.id=" + ID.ToString() + " GROUP BY r.idGameRecord LIMIT 5";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();
        int no = 1;
        while (reader.Read())
        {
            //Debug.Log("MASUK"+no.ToString());
            //Debug.Log("SCORE" + reader[21].ToString());
            //Debug.Log("WAKTU" + reader[22].ToString());

            float xOut, yOut;
            float.TryParse(reader[21].ToString(), out xOut);
            float.TryParse(reader[22].ToString(), out yOut);
            //Debug.Log(xOut.ToString() + " " + yOut.ToString());

            Vector3 pos = new Vector3(xOut * xSpacing + XminLevel.transform.position.x,
                                    yOut * ySpacing + YminLevel.transform.position.y,
                                    0);
            //Debug.Log(pos);

            GameObject tempGO = Instantiate(samplePoint, pos, Quaternion.identity) as GameObject;
            tempGO.SetActive(true);

            // ADD event Hover

            EventTrigger trigger = tempGO.GetComponentInParent<EventTrigger>();
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener((eventData) => { showDetail(tempGO); });
            trigger.triggers.Add(entry);
            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            entry2.eventID = EventTriggerType.PointerExit;
            entry2.callback.AddListener((eventData) => { closeDetail(tempGO); });
            trigger.triggers.Add(entry2);

            // Add Data to detail

            tempGO.transform.GetChild(1).gameObject.transform.GetChild(0).gameObject.GetComponent<Text>().text = reader[1].ToString();
            tempGO.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject.GetComponent<Text>().text = "Score " + xOut.ToString();
            tempGO.transform.GetChild(1).gameObject.transform.GetChild(2).gameObject.GetComponent<Text>().text = "Waktu " + yOut.ToString();


            tempGO.transform.GetChild(0).gameObject.GetComponent<Text>().text = no.ToString();
            tempGO.transform.SetParent(graphUI.transform);
            tempGO.transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
            pointList.Add(tempGO);
            no++;
        }
        dbcon.Close();

    }

    public void Make_Grafik(int id)
    {
        ID = id;
        this.Get_user(id);
        this.Get_ScoreWaktuMax();
        graphChoice.SetActive(true);
    }

    public void showGraphEasy()
    {
        CreateGraph("EASY");
        levelPlayer.text = "EASY";
        graphUI.SetActive(true);
    }

    public void showGraphMedium()
    {
        CreateGraph("MEDIUM");
        levelPlayer.text = "MEDIUM";
        graphUI.SetActive(true);
    }

    public void onCloseGraph()
    {
        graphUI.SetActive(false);
        for (int i = 0; i < pointList.Count; i++)
        {
            Destroy(pointList[i]);
        }
        pointList.Clear();
    }

    public void onCloseGraphChoice()
    {
        graphChoice.SetActive(false);
    }

    private void showDetail(GameObject tmpGO)
    {
        tmpGO.transform.GetChild(1).gameObject.SetActive(true);
    }

    private void closeDetail(GameObject tmpGO)
    {
        tmpGO.transform.GetChild(1).gameObject.SetActive(false);
    }
}
