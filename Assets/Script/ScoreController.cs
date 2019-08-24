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
    private int maxDifficultyLevel = 10;
    private int scoreToNextLevel = 10;

    // Nilai Contoroller
    public Text benarText;
    public Text salahText;

    private int truePoint;
    private int falsePoint;

    // hit Object Controller

    public GameObject GamePlay;

    private List<GameObject> hitObjects = new List<GameObject>();
    
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
                int id = GamePlay.GetComponent<EasyGameplay>().questionId;
                int answer = GamePlay.GetComponent<EasyGameplay>().answerList[id];
                if (hit.gameObject.name.Substring(0, 10) == string.Format("Choice ({0})",answer+1))
                {
                    score += 10.0f;
                    truePoint++;
                    benarText.GetComponent<Text>().text = truePoint.ToString();
                    for (int i=0; i < hitObjects.Count; i++)
                    {
                        hitObjects[i].SetActive(true);
                    }
                    GamePlay.GetComponent<EasyGameplay>().GantiSoal();
                    GamePlay.GetComponent<EasyGameplay>().timeHit = GetComponent<PlayerController>().secTime;
                }
                else
                {
                    score -= 2.0f;
                    falsePoint++;
                    salahText.GetComponent<Text>().text = falsePoint.ToString();
                    GamePlay.GetComponent<EasyGameplay>().choicesImg[int.Parse(hit.gameObject.name.Substring(8, 1)) - 1].gameObject.SetActive(false);
                    hit.gameObject.SetActive(false);
                }

                isColl = true;
                hitObjects.Add(hit.gameObject);
            }
        }
    }
}
