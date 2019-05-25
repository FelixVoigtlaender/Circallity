using UnityEngine;
using System.Collections;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Menu : MonoBehaviour {
    public GameLogic gameLogic;

    [Header("Texts")]
    public Text textHighScore;
    public Text textScore;

    [Header("Buttons")]
    public Button buttonMusic;
    public Button buttonSound;
    public Button buttonVibration;
    public Button buttonLeaderboard;
    public Button buttonReward;

    [Header("Hide")]
    public GameObject[] hideOnLeaderboard;


    [Header("Colors")]
    public Color colorActivated;
    public Color colorDeactivated;

    public bool isMenu = false;

    string soundSave = "Sound";
    string musicSave = "Music";
    string vibrationSave = "Vibration";

    DisplayHighscores displayHighscores;
    public RewardManager rewardManager;
    public InputField nameText;

    public AudioMixer masterMixer;
    float normalMusicVol, normalSoundVol;

    public RectTransform buttonContainer;
    void Start()
    {

        nameText.text = PlayerPrefs.GetString("MyName");
        masterMixer.GetFloat("musicVol", out normalMusicVol);
        masterMixer.GetFloat("soundVol", out normalSoundVol);
        displayHighscores = GetComponent<DisplayHighscores>();

        ToggleMenu(isMenu);


        //Advanced Stuff
        displayHighscores.ToggleDisplay(string.IsNullOrEmpty(PlayerPrefs.GetString("MyName")));
        CheckButtons();
        

        LoadMenu();
        CheckButtons();
    }
    void LoadMenu()
    {
        //Music
        float musicVol = PlayerPrefs.GetFloat(musicSave);
        masterMixer.SetFloat("musicVol", musicVol);
        //Sound
        float soundVol = PlayerPrefs.GetFloat(soundSave);
        masterMixer.SetFloat("soundVol", soundVol);
        //Vibration
        Vibration.isVibrate = PlayerPrefs.GetInt(vibrationSave, 1) == 1;
    }
    void SaveMenu()
    {
        //Music
        float musicVol = -80;
        masterMixer.GetFloat("musicVol", out musicVol);
        PlayerPrefs.SetFloat(musicSave, musicVol);
        //Sound
        float soundVol = -80;
        masterMixer.GetFloat("soundVol", out soundVol);
        PlayerPrefs.SetFloat(soundSave, soundVol);
        //Vibration
        PlayerPrefs.SetInt(vibrationSave, Vibration.isVibrate ? 1 : 0);
    }
    public void ToggleMenu(bool flag)
    {

        isMenu = flag;
        foreach(Transform child in transform)
        {
            child.gameObject.SetActive(flag);
        }

        displayHighscores.ToggleDisplay(false);
        CheckButtons();

        buttonContainer.localPosition = Vector2.up * 1000;
        if (flag)
        {
        }
    }
    void CheckButtons()
    {
        //Music
        float musicVol = -80;
        masterMixer.GetFloat("musicVol", out musicVol);
        if (musicVol == -80)
        {
            buttonMusic.GetComponent<Image>().color = colorDeactivated;
        }
        else
        {
            buttonMusic.GetComponent<Image>().color = colorActivated;
        }
        //Sound
        float soundVol = -80;
        masterMixer.GetFloat("soundVol", out soundVol);
        if (soundVol == -80)
        {
            buttonSound.GetComponent<Image>().color = colorDeactivated;
        }
        else
        {
            buttonSound.GetComponent<Image>().color = colorActivated;
        }
        //Vibration
        buttonVibration.GetComponent<Image>().color = Vibration.isVibrate ? colorActivated : colorDeactivated;
        //Leaderboard
        buttonLeaderboard.GetComponent<Image>().color = displayHighscores.isHighscore ? colorActivated : colorDeactivated;
        //Reward
        buttonReward.GetComponent<Image>().color = rewardManager.isReward ? colorActivated : colorDeactivated;
    }

    //Buttontasks
    public void ToggleMusic()
    {
        float musicVol = -80;
        masterMixer.GetFloat("musicVol", out musicVol);

        if (musicVol == -80)
        {
            masterMixer.SetFloat("musicVol", normalMusicVol);
        }
        else
        {
            normalMusicVol = musicVol;
            masterMixer.SetFloat("musicVol", -80);
        }

        CheckButtons();

        SaveMenu();
        Vibration.Vibrate(30);
    }
    public void ToggleSound()
    {
        float soundVol = -80;
        masterMixer.GetFloat("soundVol", out soundVol);

        if (soundVol == -80)
        {
            masterMixer.SetFloat("soundVol", normalSoundVol);
        }
        else
        {
            normalSoundVol = soundVol;
            masterMixer.SetFloat("soundVol", -80);
        }

        CheckButtons();

        SaveMenu();
        Vibration.Vibrate(30);
    }

    public void ToggleVibration()
    {
        Vibration.isVibrate = !Vibration.isVibrate;
        CheckButtons();
        Vibration.Vibrate(30);
    }
    public void StartGame()
    {
        gameLogic.StartGame();
    }

    public void ShowLeaderboard()
    {
        displayHighscores.ToggleDisplay(!displayHighscores.isHighscore);


        foreach (GameObject go in hideOnLeaderboard)
        {
            go.SetActive(!displayHighscores.isHighscore);
        }
        Vibration.Vibrate(30);

        if (displayHighscores.isHighscore)
        {
            rewardManager.ToggleReward(false);
        }



        CheckButtons();
        Vibration.Vibrate(30);
    }

    public void HideLeaderboard()
    {
        displayHighscores.ToggleDisplay(false);
        foreach (GameObject go in hideOnLeaderboard)
        {
            go.SetActive(!displayHighscores.isHighscore);
        }

        CheckButtons();
        Vibration.Vibrate(30);
    }

    public void ToggleReward()
    {
        rewardManager.ToggleReward(!rewardManager.isReward);
        HideLeaderboard();
        Vibration.Vibrate(30);
        foreach (GameObject go in hideOnLeaderboard)
        {
            go.SetActive(!rewardManager.isReward);
        }


        CheckButtons();
    }
}
