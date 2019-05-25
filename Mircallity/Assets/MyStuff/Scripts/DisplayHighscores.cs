using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DisplayHighscores : MonoBehaviour
{
    public GameObject scoresContainer;
    public GameObject scoreViewPrefab;
    public GameObject background;
    public float scoreDistance = 1.1f;
    Highscores highscoresManager;

    public bool isHighscore;

    RectTransform uiParent;

    Highscore[] myHighscoreList = new Highscore[0];
    Highscore myHighscore;



    void Start()
    {
        highscoresManager = GetComponent<Highscores>();
        //StartCoroutine("RefreshHighscores");

        uiParent = scoresContainer.GetComponent<RectTransform>();
        uiParent.localPosition = Vector2.down * 1000;
    }

    public void ToggleDisplay(bool isHighscore)
    {
        this.isHighscore = isHighscore;
        background.SetActive(isHighscore);

        if (isHighscore && uiParent)
        {
            ShowScores();
            uiParent.localPosition = Vector2.down * 1000;
            highscoresManager.DownloadHighscores();
            highscoresManager.DownloadMyHighscore();
        }
        else
        {
            DeleteScores();
        }
    }

    public void OnHighscoresDownloaded(Highscore[] highscoreList)
    {
        myHighscoreList = highscoreList;
        DeleteScores();
        if (isHighscore)
        {
            ShowScores();
        }
    }

    public void OnMyHighscoreDownloaded(Highscore myHighscore)
    {
        this.myHighscore = myHighscore;
        DeleteScores();
        if (isHighscore)
        {
            ShowScores();
        }
    }
    
    void ShowScores()
    {
        //All Scores
        DeleteScores();
        for (int i = 0; i < myHighscoreList.Length; i++)
        {
            SetUpScore(i,myHighscoreList[i]);
        }

        //My Score
        if (!string.IsNullOrEmpty(myHighscore.username)){
            if (myHighscore.place >= myHighscoreList.Length)
            {
                if (myHighscoreList.Length > 0)
                {
                    SetUpScore(myHighscoreList.Length, "...", "...", "...",false);
                    SetUpScore(myHighscoreList.Length+1, myHighscore);
                }
                else
                {
                    SetUpScore(myHighscoreList.Length, myHighscore);
                }
            }
        }
    }
    void DeleteScores()
    {
        foreach (Transform childTransform in scoresContainer.transform)
        {
            Destroy(childTransform.gameObject);
        }
    }
    

    void SetUpScore(int postion, Highscore highscore)
    {
        GameObject myScoreView = Instantiate(scoreViewPrefab, scoresContainer.transform,false);

        RectTransform uiTransform = myScoreView.GetComponent<RectTransform>();
        float deltaY = (uiTransform.rect.height) * scoreDistance;
        float y = postion * deltaY;
        uiTransform.localPosition = Vector2.down * y;
        myScoreView.GetComponent<DisplayedScore>().SetUp(highscore);
        uiParent.sizeDelta = Vector2.up * deltaY * (postion+ 1) + Vector2.right;
    }
    void SetUpScore(int postion, string place, string name, string score, bool isMe)
    {
        GameObject myScoreView = Instantiate(scoreViewPrefab, scoresContainer.transform, false);

        RectTransform uiTransform = myScoreView.GetComponent<RectTransform>();
        float deltaY = (uiTransform.rect.height) * scoreDistance;
        float y = postion * deltaY;
        uiTransform.localPosition = Vector2.down * y;
        myScoreView.GetComponent<DisplayedScore>().SetUp(place,name,score,isMe);
        uiParent.sizeDelta = Vector2.up * deltaY * (postion + 1) + Vector2.right;
    }

    IEnumerator RefreshHighscores()
    {
        while (true)
        {
            highscoresManager.DownloadHighscores();
            yield return new WaitForSeconds(30);
        }
    }
}