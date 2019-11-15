using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideMusicController : MonoBehaviour
{
    private static GuideMusicController instance = null;
    public AudioSource suara;
    public AudioClip satu;
    public AudioClip dua;
    public AudioClip tiga;
    public AudioClip tekanMenu;
    public AudioClip tekanPlay;
    public AudioClip submenuLevel;
    public AudioClip jawabBenar;
    public AudioClip semangatBenar;
    public AudioClip jawabSalah;
    public AudioClip semangatSalah;
    public AudioClip bersiap;
    public AudioClip go;
    public AudioClip soalMuncul;
    public AudioClip pilihBentuk;
    public AudioClip gameSelesai;
    public AudioClip blue;
    public AudioClip green;
    public AudioClip red;
    public AudioClip yellow;
    public AudioClip Circle;
    public AudioClip Hexagon;
    public AudioClip Kite;
    public AudioClip Oval;
    public AudioClip Parrallelogram;
    public AudioClip Pentagon;
    public AudioClip Rectangle;
    public AudioClip Rhombus;
    public AudioClip Square;
    public AudioClip Star;
    public AudioClip Triangle;
    public AudioClip blueCircle;
    public AudioClip blueHexagon;
    public AudioClip blueKite;
    public AudioClip blueOval;
    public AudioClip blueParrallelogram;
    public AudioClip bluePentagon;
    public AudioClip blueRectangle;
    public AudioClip blueRhombus;
    public AudioClip blueSquare;
    public AudioClip blueStar;
    public AudioClip blueTriangle;
    public AudioClip greenCircle;
    public AudioClip greenHexagon;
    public AudioClip greenKite;
    public AudioClip greenOval;
    public AudioClip greenParrallelogram;
    public AudioClip greenPentagon;
    public AudioClip greenRectangle;
    public AudioClip greenRhombus;
    public AudioClip greenSquare;
    public AudioClip greenStar;
    public AudioClip greenTriangle;
    public AudioClip redCircle;
    public AudioClip redHexagon;
    public AudioClip redKite;
    public AudioClip redOval;
    public AudioClip redParrallelogram;
    public AudioClip redPentagon;
    public AudioClip redRectangle;
    public AudioClip redRhombus;
    public AudioClip redSquare;
    public AudioClip redStar;
    public AudioClip redTriangle;
    public AudioClip yellowCircle;
    public AudioClip yellowHexagon;
    public AudioClip yellowKite;
    public AudioClip yellowOval;
    public AudioClip yellowParrallelogram;
    public AudioClip yellowPentagon;
    public AudioClip yellowRectangle;
    public AudioClip yellowRhombus;
    public AudioClip yellowSquare;
    public AudioClip yellowStar;
    public AudioClip yellowTriangle;


    // Start is called before the first frame update
    void Start()
    {
        suara = gameObject.GetComponent<AudioSource>();
        suara.Play();
    }

    // Update is called once per frame
    void Update()
    {

    }
    // Music Global Start
    public static GuideMusicController Instance
    {
        get { return instance; }
    }

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        else
        {
            instance = this;
        }

        DontDestroyOnLoad(this.gameObject);
    }
    // Music Global End

    public void playGuideMusic(AudioClip audioTemp)
    {
        suara.Stop();
        suara.clip = audioTemp;
        suara.Play();
    }
}
