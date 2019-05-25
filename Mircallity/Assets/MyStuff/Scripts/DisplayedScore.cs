using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class DisplayedScore : MonoBehaviour {
    public Text placeText;
    public Text nameText;
    public Text scoreText;

    public Color isMeColor;
    public Color isFirstColor;


    public void SetUp(Highscore highscore)
    {
        placeText.text = "" + (highscore.place + 1);
        nameText.text = highscore.username;
        scoreText.text = "" + highscore.score;
        if(highscore.place == 0)
        {
            GetComponent<RawImage>().color = isFirstColor;
        }

        if (highscore.isMe)
        {
            GetComponent<RawImage>().color = isMeColor;
        }
    }

    public void SetUp(string place, string name,string score, bool isMe)
    {
        placeText.text = "" + place;
        nameText.text = name;
        scoreText.text = "" + score;

        if (isMe)
        {
            GetComponent<RawImage>().color = isMeColor;
        }
    }
}
