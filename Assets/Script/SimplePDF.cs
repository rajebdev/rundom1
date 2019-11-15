using System.Data;
using Mono.Data.Sqlite;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using sharpPDF;
using sharpPDF.Enumerators;
using System;



public class SimplePDF : MonoBehaviour
{
    private int spacing = 160;
    private List<pdfPage> pageList;
    private List<pdfTable> tableList;
    private string nama, sex, nilaibenar, nilaisalah, score, time, attachName;
    private string gametype;
    public GameObject pdfWarn, pdfSuc;
    private string[] soal = {
        "aaa",
        "aaa",
        "aaa"

    };
    
    pdfDocument myDoc;
    pdfPage myFirstPage;

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

    private void Start()
    {
        pdfWarn.SetActive(false);
        pdfSuc.SetActive(false);
    }
    // Use this for initialization
    public void Make_pdf(int id, string idrecord)
    {
        pageList = new List<pdfPage>();
        tableList = new List<pdfTable>();
        CreatePDF(id, idrecord);
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
            nama = reader[1].ToString();
            sex = reader[2].ToString();

        }
    }

    private void Get_GameDetail(string idrecord)
    {
        string connection = "URI=file:" + Application.dataPath + "/StreamingAssets/rundomdb.db";
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;

        string query = "SELECT * FROM GameDetail" +
                       " WHERE idGameRecord='" + idrecord + "'";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();

        while (reader.Read())
        {
            nilaibenar = reader[1].ToString();
            nilaisalah = reader[2].ToString();
            score = reader[3].ToString();
            time = reader[4].ToString();
        }
    }

    private void Get_GameRecord(int id, string idrecord)
    {
        string connection = "URI=file:" + Application.dataPath + "/StreamingAssets/rundomdb.db";
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;

        string query = "SELECT * FROM GameRecord" +
                       " WHERE id='"+ id.ToString() + "' AND idGameRecord='" + idrecord + "'";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();

        while (reader.Read())
        {
            gametype = reader[2].ToString();
            soal[0] = reader[13].ToString() + " " + namaBangun[int.Parse(reader[3].ToString())];
            soal[1] = reader[14].ToString() + " " + namaBangun[int.Parse(reader[4].ToString())];
            soal[2] = reader[15].ToString() + " " + namaBangun[int.Parse(reader[5].ToString())];
        }
    }

    public int CreatePDF(int id, string idrecord)
    {
        int pageIndex = 0;
        int tableIndex = 0;
        DateTime dateNow = DateTime.Now;
        string getDate = String.Format("{0:D4}/{1:D2}/{2:D2} {3:D2}:{4:D2}", dateNow.Year, dateNow.Month, dateNow.Day, dateNow.Hour, dateNow.Minute);
        this.Get_user(id);
        this.Get_GameDetail(idrecord);
        this.Get_GameRecord(id, idrecord);
        attachName = "Rundom_" + nama.ToUpper() + "_" + gametype + "_" + idrecord + ".pdf";
        myDoc = new pdfDocument("Rundom PDF Proses", "rajebdev", false);
        myFirstPage = myDoc.addPage();       

        pageList.Add(myFirstPage);
      

        pageList[0].addText("Hasil Perkembangan Siswa - RUNDOM GAME", 45, 730, predefinedFont.csHelveticaOblique, 20, new pdfColor(predefinedColor.csBlack));
        pageList[0].addText("Nama Siswa        :  "+nama.ToUpper(), 50, 680, predefinedFont.csHelveticaOblique, 14, new pdfColor(predefinedColor.csBlack));
        pageList[0].addText("Jenis Kelamin     :  "+sex, 50, 660, predefinedFont.csHelveticaOblique, 14, new pdfColor(predefinedColor.csBlack));
        pageList[0].addText("Tingkatan           :  " + gametype, 50, 640, predefinedFont.csHelveticaOblique, 14, new pdfColor(predefinedColor.csBlack));
        pageList[0].addText("ID Permainan     :  " + idrecord, 50, 620, predefinedFont.csHelveticaOblique, 14, new pdfColor(predefinedColor.csBlack));

        pageList[0].addText("Ringkasan Permainan ", 50, 580, predefinedFont.csHelveticaOblique, 16, new pdfColor(predefinedColor.csBlack));

        tableList.Add(new pdfTable());
        //Set table's border
        tableList[tableIndex].borderSize = 1;
        tableList[tableIndex].borderColor = new pdfColor(predefinedColor.csDarkBlue);

        /*Set Header's Style*/
        tableList[tableIndex].tableHeaderStyle = new pdfTableRowStyle(predefinedFont.csCourierBoldOblique, 12, new pdfColor(predefinedColor.csBlack), new pdfColor(predefinedColor.csLightOrange));
        tableList[tableIndex].rowStyle = new pdfTableRowStyle(predefinedFont.csCourier, 12, new pdfColor(predefinedColor.csBlack), new pdfColor(predefinedColor.csWhite));
        tableList[tableIndex].alternateRowStyle = new pdfTableRowStyle(predefinedFont.csCourier, 12, new pdfColor(predefinedColor.csBlack), new pdfColor(predefinedColor.csLightYellow));
        tableList[tableIndex].cellpadding = 7;
        //==========================

        /*Add Columns to a grid*/
        tableList[tableIndex].tableHeader.addColumn(new pdfTableColumn("Keterangan", predefinedAlignment.csCenter, 150));
        tableList[tableIndex].tableHeader.addColumn(new pdfTableColumn("Nilai", predefinedAlignment.csCenter, 200));


        pdfTableRow myRow = tableList[tableIndex].createRow();
        myRow[0].columnValue = "Jumlah Benar";
        myRow[1].columnValue = nilaibenar;
        tableList[tableIndex].addRow(myRow);

        pdfTableRow myRow1 = tableList[tableIndex].createRow();
        myRow1[0].columnValue = "Jumlah Salah";
        myRow1[1].columnValue = nilaisalah;
        tableList[tableIndex].addRow(myRow1);

        pdfTableRow myRow2 = tableList[tableIndex].createRow();
        myRow2[0].columnValue = "Score";
        myRow2[1].columnValue = score;
        tableList[tableIndex].addRow(myRow2);

        
        pdfTableRow myRow3 = tableList[tableIndex].createRow();
        myRow3[0].columnValue = "Waktu";
        myRow3[1].columnValue = time+" Detik";
        tableList[tableIndex].addRow(myRow3);


        pdfTableRow myRow4 = tableList[tableIndex].createRow();
        myRow4[0].columnValue = "Total Lompatan";
        myRow4[1].columnValue = (int.Parse(nilaibenar) + int.Parse(nilaisalah)).ToString() + " Kali";
        tableList[tableIndex].addRow(myRow4);

        pageList[pageIndex].addTable(tableList[tableIndex], 55, 560);

        // Detail Permainan

        pageList[0].addText("Detail Permainan ", 50, 350, predefinedFont.csHelveticaOblique, 16, new pdfColor(predefinedColor.csBlack));

        tableList.Add(new pdfTable());
        //Set table's border
        tableList[tableIndex].borderSize = 1;
        tableList[tableIndex].borderColor = new pdfColor(predefinedColor.csDarkBlue);

        /*Set Header's Style*/
        tableList[1].tableHeaderStyle = new pdfTableRowStyle(predefinedFont.csCourierBoldOblique, 11, new pdfColor(predefinedColor.csBlack), new pdfColor(predefinedColor.csLightOrange));
        tableList[1].rowStyle = new pdfTableRowStyle(predefinedFont.csCourier, 11, new pdfColor(predefinedColor.csBlack), new pdfColor(predefinedColor.csWhite));
        tableList[1].alternateRowStyle = new pdfTableRowStyle(predefinedFont.csCourier, 11, new pdfColor(predefinedColor.csBlack), new pdfColor(predefinedColor.csLightYellow));
        tableList[1].cellpadding = 5;
        //==========================

        /*Add Columns to a grid*/
        tableList[1].tableHeader.addColumn(new pdfTableColumn("No", predefinedAlignment.csCenter, 30));
        tableList[1].tableHeader.addColumn(new pdfTableColumn("Soal", predefinedAlignment.csCenter, 150));
        tableList[1].tableHeader.addColumn(new pdfTableColumn("Jawab", predefinedAlignment.csCenter, 150));
        tableList[1].tableHeader.addColumn(new pdfTableColumn("Kondisi", predefinedAlignment.csCenter, 50));
        tableList[1].tableHeader.addColumn(new pdfTableColumn("Score", predefinedAlignment.csCenter, 50));
        tableList[1].tableHeader.addColumn(new pdfTableColumn("Waktu", predefinedAlignment.csCenter, 70));

        // add data gameecord hit to data;

        string connection = "URI=file:" + Application.dataPath + "/StreamingAssets/rundomdb.db";
        IDbConnection dbcon = new SqliteConnection(connection);
        dbcon.Open();
        IDbCommand cmnd_read = dbcon.CreateCommand();
        IDataReader reader;

        string query = "SELECT * FROM GameRecordHit" +
                       " WHERE idGameRecord='" + idrecord + "'";
        cmnd_read.CommandText = query;
        reader = cmnd_read.ExecuteReader();
        int no = 1;
        int tmp = 0;
        while (reader.Read())
        {
            pdfTableRow tmpRow = tableList[1].createRow();
            tmpRow[0].columnValue = no.ToString();
            tmpRow[1].columnValue = soal[int.Parse(reader[1].ToString())];
            tmpRow[2].columnValue = reader[6].ToString() + " " + namaBangun[int.Parse(reader[5].ToString())];
            tmpRow[3].columnValue = reader[3].ToString() == "0" ? "SALAH" : "BENAR";
            tmpRow[4].columnValue = reader[4].ToString();
            tmpRow[5].columnValue = (int.Parse(reader[7].ToString())-tmp).ToString() + " Detik";
            tmp = int.Parse(reader[7].ToString());
            tableList[1].addRow(tmpRow);
            no++;
        }
        
        pageList[pageIndex].addTable(tableList[1], 55, 330);


        pageList[0].addText("Generate at  " + getDate, 400, 50, predefinedFont.csHelveticaOblique, 8, new pdfColor(predefinedColor.csBlack));

        myDoc.createPDF(Application.dataPath + "/StreamingAssets/Pdf File/" + attachName);
        if (myDoc.isSucces == true)
        {
            pdfSuc.SetActive(true);
        }
        else {
            pdfWarn.SetActive(true);
        }

        pageList = null;
        tableList = null;
        return 0;
    }

    public void ClosePdfConf()
    {
        pdfWarn.SetActive(false);
        pdfSuc.SetActive(false);
    }

    public void openPdfSuc()
    {
        Application.OpenURL(Application.dataPath + "/StreamingAssets/Pdf File/" + attachName);
    }
}
