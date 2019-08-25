using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class Gameplay : MonoBehaviour
{
    public GameObject player;

    public Image questionImg;
    public GameObject[] choicesBox;
    public GameObject[] choicesImg;

    public Sprite[] shapes;

    public int questionId;

    private int banyakSoal = 5;

    public int[] quesList, answerList, shapeChoice1List, shapeChoice2List;

    // Change Soal

    private bool changeQuestion;

    public int timeHit;

    public int countdownQuestion;

    public int jarakSetiapSoal = 12;


    // Countdown
    public Image containerCountdownSoal;
    public Text countdownText;

    // Start is called before the first frame update
    void Start()
    {
        quesList = new int[banyakSoal];
        answerList = new int[banyakSoal];
        shapeChoice1List = new int[banyakSoal];
        shapeChoice2List = new int[banyakSoal];
        for (int i=0; i < banyakSoal; i++)
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
        }

    }

    // Update is called once per frame
    void Update()
    {

        // Mengacak jawaban
        questionImg.GetComponent<Image>().sprite = shapes[GetShapeNum()];
        containerCountdownSoal.gameObject.SetActive(false);
        if (timeHit != 0)
            countdownQuestion = player.GetComponent<PlayerController>().secTime - timeHit;
        
        if (questionId < 4)
        {
            if (countdownQuestion > jarakSetiapSoal - 3 && countdownQuestion < jarakSetiapSoal + 1)
            {
                containerCountdownSoal.gameObject.SetActive(true);
                countdownText.text = (jarakSetiapSoal + 1 - countdownQuestion).ToString();
                return;
            }

        }
        else
        {
            containerCountdownSoal.gameObject.SetActive(false);
        }


        if (questionId == 5)
        {
            player.GetComponent<ScoreController>().OnDeath();
            return;
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
                return;
            }
        }

        if (questionId < 5)
        {
            questionImg.GetComponent<Image>().sprite = shapes[quesList[questionId]];
            choicesImg[answerList[questionId]].GetComponent<SpriteRenderer>().sprite = shapes[quesList[questionId]];

            if (answerList[questionId] == 0)
            {
                choicesImg[1].GetComponent<SpriteRenderer>().sprite = shapes[shapeChoice1List[questionId]];
                choicesImg[2].GetComponent<SpriteRenderer>().sprite = shapes[shapeChoice2List[questionId]];
            }
            else if (answerList[questionId] == 1)
            {
                choicesImg[0].GetComponent<SpriteRenderer>().sprite = shapes[shapeChoice1List[questionId]];
                choicesImg[2].GetComponent<SpriteRenderer>().sprite = shapes[shapeChoice2List[questionId]];
            }
            else if (answerList[questionId] == 2)
            {
                choicesImg[0].GetComponent<SpriteRenderer>().sprite = shapes[shapeChoice1List[questionId]];
                choicesImg[1].GetComponent<SpriteRenderer>().sprite = shapes[shapeChoice2List[questionId]];
            }
        }
    }

    private int GetShapeNum()
    {
        int shapeNum = 0;
        shapeNum = Random.Range(0, shapes.Length);
        return shapeNum;
    }

    public void GantiSoal()
    {
        changeQuestion = true;
    }
}
