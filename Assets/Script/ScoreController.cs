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
                if (hit.gameObject.name.Substring(0, 10) == "Choice (2)")
                {
                    score += 1.0f;
                    truePoint++;
                    benarText.GetComponent<Text>().text = truePoint.ToString();
                }
                else
                {
                    falsePoint++;
                    salahText.GetComponent<Text>().text = falsePoint.ToString();
                }

                isColl = true;
                GameObject hitTemp = hit.gameObject;
                Destroy(hit.gameObject);
                GameObject newCube = Instantiate(hitTemp) as GameObject;
                newCube.transform.SetParent(cubeBox);
                newCube.transform.position = new Vector3(hitTemp.transform.position.x, hitTemp.transform.position.y, cubeBox.position.z);
                //newCube.gameObject.GetComponent<Renderer>().material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
            }
        }
    }
}
