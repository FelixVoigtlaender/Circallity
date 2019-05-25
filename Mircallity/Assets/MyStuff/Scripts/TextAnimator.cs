using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextAnimator : MonoBehaviour {

    public Text text;

    RectTransform scoreTransform;
    Vector2 sizeNormal;
    Vector2 sizeVelocity;
    public float sizeTime;
    public float scoreTime;

    public float scoreCurrent;
    float scoreVelocity;
    public int goalScore;

    public void Start()
    {
        if (!text)
        {
            text = GetComponent<Text>();
        }

        scoreTransform = text.GetComponent<RectTransform>();
        sizeNormal = scoreTransform.sizeDelta;
    }

    public void Update()
    {
        //Size | Changes size to normal
        float scoreW = Mathf.SmoothDamp(scoreTransform.sizeDelta.x, sizeNormal.x, ref sizeVelocity.x, 0.1f);
        float scoreH = Mathf.SmoothDamp(scoreTransform.sizeDelta.y, sizeNormal.y, ref sizeVelocity.y, 0.1f);
        scoreTransform.sizeDelta = new Vector2(scoreW, scoreH);

        //CurrentScore | Changes currentScore to goalScore
        int lastScore = Mathf.RoundToInt(scoreCurrent);
        scoreCurrent = Mathf.SmoothDamp(scoreCurrent, goalScore, ref scoreVelocity, scoreTime);
        if(Mathf.RoundToInt(scoreCurrent)!= lastScore)  //IF the int score changes, the text is changed
        {
            ChangeScore(scoreCurrent, Mathf.RoundToInt(scoreCurrent) == goalScore);
        }
    }
    public void ChangeColor(Color color)
    {
        text.color = color;
    }


    public void ChangeScore(float score, bool isFinal = false)  //Changes the currentScore+Animation | not goalScore
    {
        scoreCurrent = score;
        int myScore = Mathf.RoundToInt(score);
        float currentPush = scoreTransform.sizeDelta.magnitude / sizeNormal.magnitude;
        float push = 1.2f;
        push = (myScore % 5) == 0 ? 1.5f : push;
        push = (myScore % 10) == 0 ? 1.75f : push;
        push = (myScore % 20) == 0 ? 2f : push;
        push = myScore == 0 ? 1.2f : push;
        push = isFinal ? 2.5f : push;
        if (push > currentPush)
        {
            scoreTransform.sizeDelta = sizeNormal * push;
        }

        text.text = "" + myScore;
    }

    public void SetGoalScore(int goalScore) //Changes goalScore | not current
    {
        this.goalScore = goalScore;
    }
    public void SetScore(int score) //Sets Current & GoalScore | Score will not change after that
    {
        scoreCurrent = score;
        goalScore = score;
        ChangeScore(score);
    }
    public int GetScore()   //Gets goalScore | NOT current
    {
        return goalScore;
    }
}
