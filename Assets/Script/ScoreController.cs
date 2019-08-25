using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreController : MonoBehaviour
{
    private float score = 0.0f;

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

    // Start is called before the first frame update
    void Start()
    {
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
            GamePlay.GetComponent<Gameplay>().questionImg.gameObject.SetActive(false);
        else
            GamePlay.GetComponent<Gameplay>().questionImg.gameObject.SetActive(true);

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
                    score += 10.0f;
                    truePoint++;
                    benarText.GetComponent<Text>().text = truePoint.ToString();
                    for (int i=0; i < hitObjects.Count; i++)
                    {
                        hitObjects[i].SetActive(true);
                    }
                    GamePlay.GetComponent<Gameplay>().GantiSoal();
                    GamePlay.GetComponent<Gameplay>().timeHit = GetComponent<PlayerController>().secTime;
                    trueNotif.SetActive(true);
                }
                else
                {
                    score -= 2.0f;
                    falsePoint++;
                    salahText.GetComponent<Text>().text = falsePoint.ToString();
                    GamePlay.GetComponent<Gameplay>().choicesImg[int.Parse(hit.gameObject.name.Substring(8, 1)) - 1].gameObject.SetActive(false);
                    hit.gameObject.SetActive(false);
                    timeHitFalse = GetComponent<PlayerController>().secTime;
                    falseNotif.SetActive(true);
                }

                isColl = true;
                hitObjects.Add(hit.gameObject);
            }
        }
    }
}
