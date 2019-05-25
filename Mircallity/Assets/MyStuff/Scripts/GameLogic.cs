using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameLogic : MonoBehaviour
{
    public Menu menu;
    public GameObject playerPrefab;
    public TextAnimator gameScore;
    public TextAnimator menuScore;
    public CameraController cameraController;
    [Header("Options")]
    public bool useScreenSize;
    [Header("Generation")]
    public float skinWidth;
    public float distanceY, distanceX;
    public float deltaY;
    public float minSize, maxSize;
    [Header("Balls")]
    public GameObject ballNormal;
    public GameObject ballFadeout;
    public GameObject ballKill;
    [Header("Ads")]
    public AdManager adManager;
    public Color colorShowAd;
    public Color colorNormal;
    [Header("Analytics")]
    public AnalyticsManager analyticsManager;
    [Header("Hints")]
    public HintManager hintManager;
    public string startText;
    public float reminderTime;
    float menuStart;
    public RewardManager rewardManager;

    string highscoreSave = "Highscore2";

    bool deleteAllSaves = true;

    List<GameObject> allBalls = new List<GameObject>();

    int score;
    int highScore;


    GameObject[] currentGoodBalls;
    GameObject[] currentBadBalls;

    

    public void Start()
    {
        GameLogicOpitions.ResetGameLogicOptions();

        if (PlayerPrefs.GetInt("HasHacked", 0) == 1)
        {
            menu.ToggleMenu(false);
        }
        Screen.orientation = ScreenOrientation.Portrait;

        menu.ToggleMenu(true);
        cameraController = Camera.main.GetComponent<CameraController>();
        cameraController.SetPosition(Vector2.zero);
        highScore = PlayerPrefs.GetInt(highscoreSave, 0);
        menu.textHighScore.text = "" + highScore;
        gameScore.SetScore(0);

        if (useScreenSize)
        {
            SetUpOptions();
        }

        menuStart = Time.time;

        SetupBalls();

        //Score
        gameScore.gameObject.SetActive(false);
    }

    void SetupBalls()
    {
        //Good/Normal
        List<GameObject> goodBalls = new List<GameObject>();
        goodBalls.Add(ballNormal);
        if (GameLogicOpitions.isFadeout)
        {
            goodBalls.Add(ballFadeout);
        }
        currentGoodBalls = goodBalls.ToArray();
        //Bad
        List<GameObject> badBalls = new List<GameObject>();
        if (GameLogicOpitions.isKill)
        {
            badBalls.Add(ballKill);
        }
        currentBadBalls = badBalls.ToArray();
    }

    void Update()
    {
        if (menu.isMenu)
        {
            if(Time.time>=menuStart + reminderTime)
            {
                TriggerReminder();
            }
        }

    }

    void TriggerReminder()
    {
        HintManager.SetText(startText);
        hintManager.ToggleHints(menu.isMenu);
    }

    void SetUpOptions()
    {
        Camera cam = Camera.main;
        float camHeight = 2f * cam.orthographicSize;
        float camWidth = camHeight * cam.aspect;

        distanceX = camWidth / 2 - skinWidth;
        minSize = distanceX * 0.6f;
        maxSize = distanceX * 0.85f;
    }

    public void StartGame()
    {
        SetupBalls();
        float cameraDistance = ((Vector2)Camera.main.transform.position).magnitude;
        if (cameraDistance > 2)
        {
            return;
        }

        GameObject player = Instantiate(playerPrefab);
        PlayerController playerController = player.GetComponent<PlayerController>();
        playerController.SetColor(GameLogicOpitions.playerColor);
        cameraController.SetPosition(Vector2.zero);
        //playerController.Spawn(Vector2.zero, Quaternion.Euler(Vector2.zero));
        playerController.gameLogic = this;
        Time.timeScale = 1f;
        SetScore(0);
        menu.ToggleMenu(false);

        //Clearing all Balls from last play.    
        foreach (GameObject go in allBalls)
        {
            Destroy(go);
        }
        allBalls.Clear();

        Vibration.Vibrate(50);

        CheckColor();

        hintManager.ToggleHints(false);

        //Score
        gameScore.gameObject.SetActive(true);
        
    }

    public void Loose()
    {
        GameLogicOpitions.ResetGameLogicOptions();
        SetFinalScore(score);

        menu.ToggleMenu(true);
        cameraController.SetPosition(Vector2.zero);

        if (score >= 5)
        {
            rewardManager.GenerateRewards();
            menu.ToggleReward();
        }

        menuStart = Time.time;

        //Score
        gameScore.gameObject.SetActive(false);

    }
    int CountJumpObjects()
    {
        return GameObject.FindGameObjectsWithTag("ManagerBall").Length -1;
    }
    public void SetFinalScore(int score)    //When you die
    {
        menuScore.SetScore(0);
        menuScore.SetGoalScore(score);

        if (score > highScore)
        {
            SetHighscore(score);
        }
        else
        {
            SetNormalScore(score);
        }
    }
    public void SetNormalScore(int score)   //When score is below highscore
    {
        menu.ToggleMenu(true);
        cameraController.SetPosition(Vector2.zero);
        highScore = score > highScore ? score : highScore;
        menu.textHighScore.text = "" + highScore;
        PlayerPrefs.SetInt("Highscore1", highScore);

        if (adManager)
        {
            adManager.CheckAd();
        }

        if (hintManager)
        {
            HintManager.CheckHints();
        }
        if (analyticsManager)
        {
            analyticsManager.OnScore(score);
        }
    }
    public void SetHighscore(int score) //Sets Client and Server Score
    {

        if (score == CountJumpObjects())
        {
            Highscores.AddNewHighscore(CountJumpObjects());
        }
        else
        {
            adManager.showEveryN = 0;   //Thank the hacker for ma Ads!
            PlayerPrefs.SetInt("HasHacked", 1);
        }
        SetLocalHighscore(score);


        if (adManager)
        {
            adManager.DelayAd();
            CheckColor();
        }

        if (hintManager)
        {
            hintManager.RewardPlayer();
            hintManager.ToggleHints(true);
        }
        if (analyticsManager)
        {
            analyticsManager.OnHighscoreReached(score);
        }
    }
    public void SetLocalHighscore(int score)    //Sets Client Score (Menu "for the looks")
    {
        this.highScore = score;
        menu.textHighScore.text = "" + highScore;
    }

    public void DeltaScore(int n)
    {
        SetScore(score + n);
    }
    public void SetScore(int score)
    {
        this.score = score;
        gameScore.SetScore(score);

        Time.timeScale = Mathf.Clamp( 1f + score * 0.1f,1,3f);

        if (adManager)
        {
            if (score >= highScore)
            {
                adManager.DelayAd();
                CheckColor();
            }
        }
    }


    public void SetLastBall(GameObject lastBall)
    {
        float y = (distanceY + Random.Range(-1f, 1f) * deltaY);
        //GenerateNewBalls(lastBall.transform.position + Vector3.up * y);
        Generate(lastBall.transform.position);
        //cameraController.SetPosition(Vector2.up * (lastBall.transform.position.y + y / 2));
    }

    public void SetUpBall(GameObject ball)
    {
        FadeInOut fade = ball.GetComponent<FadeInOut>();
        fade.HardASet(0, true);
    }

    void InitializeBall(Vector3 position, float size, GameObject ballPrefab)
    {
        if (!ballPrefab)
        {
            return;
        }
        GameObject ball = Instantiate(ballPrefab, position, Quaternion.Euler(Vector3.zero), null);
        ball.transform.localScale = Vector3.one * size;
        Rotation ballRotation = ball.GetComponent<Rotation>();
        if (ballRotation)
        {
            ballRotation.SetSpeed(ballRotation.GetSpeed() * (Random.Range(0, 1f) > 0.5f ? 1 : -1));        //Sets rotation into random direction
        }
        SetUpBall(ball);
        allBalls.Add(ball);
    }

    void OnDrawGizmos()
    {

        Gizmos.color = Color.cyan;
        Gizmos.DrawRay(Vector2.left * distanceX, Vector2.up * 100);
        Gizmos.DrawRay(Vector2.right * distanceX, Vector2.up * 100);

        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(Vector2.left * distanceX + Vector2.up, Vector2.right * maxSize);
        Gizmos.DrawRay(Vector2.left * distanceX + Vector2.up * 1.1f, Vector2.right * minSize);

        Gizmos.DrawRay(Vector2.right * 0.1f, Vector2.up * (distanceY + deltaY));
        Gizmos.DrawRay(Vector2.zero + Vector2.right * 0.2f, Vector2.up * (distanceY - deltaY));

    }

    void CheckColor()
    {
        gameScore.ChangeColor( adManager.WillShowAd() ? colorShowAd : colorNormal);
    }

    public void Generate(Vector3 position)
    {
        List<GenerationOption> genOptions = new List<GenerationOption>();
        if (GameLogicOpitions.allowNormal)
        {
            genOptions.Add(new Normal());
        }
        if (GameLogicOpitions.allowDoubleJump)
        {
            genOptions.Add(new DoubleJump());
        }
        if (GameLogicOpitions.allowDoubleBall)
        {
            genOptions.Add(new DoubleBalls());
        }

        GenerationOption[] options = genOptions.ToArray();
        options[Random.Range(0, options.Length)].Generate(position, this);
    }

    public void GenerateNormal(Vector3 position)
    {
        float size = minSize + (maxSize - minSize) * Random.Range(0f, 1f);
        float x = Random.Range(-1f, 1f) * (distanceX - size / 2);
        float y = (distanceY + Random.Range(-1f, 1f) * deltaY) + position.y;
        int index = Random.Range(0, currentGoodBalls.Length);
        Vector3 newPosition = new Vector3(x, y, 0);
        if (currentGoodBalls.Length > 0)
        {
            InitializeBall(newPosition, size, currentGoodBalls[index]);
        }

        //Camera
        cameraController.SetPosition(Vector2.up * ((position.y + y) / 2));
    }
    public void GenerateDoubleJump(Vector3 position)
    {
        float xDir = Mathf.Sign(position.x);
        xDir = xDir == 0 ? Mathf.Sign(Random.Range(-1f, 1f)) : xDir;
        print(xDir);

        float sizeN = minSize + (maxSize - minSize) * Random.Range(0f, 1f);
        float xN = xDir * Random.Range(0f, 1f) * (distanceX - sizeN / 2);
        float yN = (distanceY*1.8f + Random.Range(-1f, 1f) * deltaY) + position.y;
        int indexN = Random.Range(0, currentGoodBalls.Length);
        Vector3 newPositionN = new Vector3(xN, yN, 0);
        if (currentGoodBalls.Length > 0)
        {
            InitializeBall(newPositionN, sizeN, currentGoodBalls[indexN]);
        }

        float sizeB = sizeN + sizeN * 0.75f;
        float mustDistance = sizeN / 2 + sizeB / 2 + 0.21f;
        float deltaXB = Mathf.Clamp(distanceX - (mustDistance - Mathf.Abs(xN)), 0, distanceX);
        float xB=xN - (mustDistance + Random.Range(0, 1f) * deltaXB) * xDir;
        float yB = (position.y + yN)/2;
        int indexB = Random.Range(0, currentBadBalls.Length);
        Vector3 newPositionB = new Vector3(xB, yB, 0);
        if (currentBadBalls.Length > 0)
        {
            InitializeBall(newPositionB, sizeB, currentBadBalls[indexB]);
        }

        //Camera
        cameraController.SetPosition(Vector2.up * ((position.y + yN) / 2));
    }
    public void GenerateDoubleBall(Vector3 position)
    {
        float sizeN = minSize + (maxSize - minSize) * Random.Range(0f, 1f);
        float xN = Random.Range(-1f, 1f) * (distanceX - sizeN / 2);
        float yN = (distanceY + Random.Range(-1f, 1f) * deltaY) + position.y;
        int indexN = Random.Range(0, currentGoodBalls.Length);
        Vector3 newPositionN = new Vector3(xN, yN, 0);
        if (currentGoodBalls.Length > 0)
        {
            InitializeBall(newPositionN, sizeN, currentGoodBalls[indexN]);
        }

        float xDirN = Mathf.Sign(xN);
        xDirN = xDirN == 0 ? Mathf.Sign(Random.Range(-1f, 1f)) : xDirN;
        float sizeB = sizeN + sizeN * 0.75f;
        float mustDistance = sizeN / 2 + sizeB / 2 + 0.21f;
        float deltaXB = Mathf.Clamp(distanceX - (mustDistance - Mathf.Abs(xN)),0,distanceX);
        float xB = xN - (mustDistance + Random.Range(0, 1f) * deltaXB) * xDirN;
        float yB = (distanceY + Random.Range(-1f, 1f) * deltaY) + position.y;
        int indexB = Random.Range(0, currentBadBalls.Length);
        Vector3 newPositionB = new Vector3(xB, yB, 0);
        if (currentBadBalls.Length > 0)
        {
            InitializeBall(newPositionB, sizeB, currentBadBalls[indexB]);
        }

        //Camera
        cameraController.SetPosition(Vector2.up * ((position.y + yN) / 2));
    }

}

class GenerationOption
{
    public virtual void Generate(Vector3 position, GameLogic gameLogic)
    {
        gameLogic.GenerateNormal(position);
    }
}

class Normal : GenerationOption
{
    public override void Generate(Vector3 position, GameLogic gameLogic)
    {
        gameLogic.GenerateNormal(position);
    }
}

class DoubleJump : GenerationOption
{
    public override void Generate(Vector3 position, GameLogic gameLogic)
    {
        gameLogic.GenerateDoubleJump(position);
    }
}

class DoubleBalls : GenerationOption
{
    public override void Generate(Vector3 position, GameLogic gameLogic)
    {
        gameLogic.GenerateDoubleBall(position);
    }
}