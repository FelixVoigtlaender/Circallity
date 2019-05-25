using UnityEngine;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;

public class Highscores : MonoBehaviour
{
    string privateCode = "fm`,vR3gbmxlTf2OQ4s5cwfNyEi1eai,w5Ddqg^7pGqG";
    string publicCode = "78d478b31/cc8723544a3084";
    const string webURL = "http://dreamlo.com/lb/";

    DisplayHighscores highscoreDisplay;
    public Highscore[] highscoresList;
    static Highscores instance;

    private string myId;
    private string myName;

    private string newMyName;

    public GameLogic gameLogic;

    int myOnlineHighscore;

    void Awake()
    {
        if(PlayerPrefs.GetInt("HasHacked", 0) == 1)
        {
            privateCode = "FUCK OFF";
            publicCode = "FUCK OFF";
            PlayerPrefs.SetString("MyID", "000000000");
        }
        highscoreDisplay = GetComponent<DisplayHighscores>();
        instance = this;

        if (string.IsNullOrEmpty(PlayerPrefs.GetString("MyID")))
        {
            myId = GenerateId();
            PlayerPrefs.SetString("MyID", myId);
        }
        else
        {
            myId = PlayerPrefs.GetString("MyID");
        }
        if (string.IsNullOrEmpty(PlayerPrefs.GetString("MyName")))
        {
            myName = "NoName";
        }
        else
        {
            myName = PlayerPrefs.GetString("MyName");
        }


        print("ID: " + myId + " Name: " + myName);

        DownloadMyHighscore();
        //MUSTBEDELETED();
    }

    string Twister(string s, bool isUp)
    {
        string s2 = "";
        for(int i = s.Length-1; i>=0; i--)
        {
            s2 += s[i];
        }
        s = "";
        char[] charArray = s2.ToCharArray();
        for(int i = 0; i < charArray.Length; i++)
        {
            if (isUp)
            {
                charArray[i]++;
            }
            else
            {
                charArray[i]--;
            }
           s += charArray[i];
        }
        s2 = charArray.ToString();
        return s;
    }

    string GenerateId()
    {
        string id = "";
        id += System.DateTime.Now.ToString("hhmmss");
        id += System.DateTime.Now.ToString("MMddyyyy");
        id += Random.Range(1, 1000);
        return id;
    }
    string GenerateName()
    {
        string name = "";
        string alphabet = "abcdefghijklmnopqrstuvwxyz";
        for (int i = 0; i < Random.Range(5, 10); i++)
        {
            name += alphabet[Random.Range(0, alphabet.Length)];
        }
        return name;
    }

    public void ChangeName()
    {
        if (string.IsNullOrEmpty(newMyName))
        {
            return;
        }

        print("Changing name to: " + newMyName);
        PlayerPrefs.SetString("MyName", newMyName);
        instance.myName = newMyName;
        int score = instance.myOnlineHighscore;
        
        AddNewHighscore(score);
    }

    public void AlterNewName(string myName)
    {
        newMyName = myName;
    }

    public static void AddNewHighscore(int score)
    {
        instance.StartCoroutine(instance.UploadNewHighscore(instance.myId, score, instance.myName));
    }
    public static void AddNewHighscore(string id, int score, string name)
    {
        instance.StartCoroutine(instance.UploadNewHighscore(id,score,name));
    }


    IEnumerator UploadNewHighscore(string id, int score, string name)
    {
        print(name);
        WWW www = new WWW(webURL + Twister(privateCode,true) + "/add/" + WWW.EscapeURL(id) + "/" + score + "/" +(1000-score) + "/" + WWW.EscapeURL(name));
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            print("Upload Successful");
            DownloadHighscores();
            DownloadMyHighscore();
        }
        else
        {
            print("Error uploading: " + www.error);
        }
    }

    public void DownloadMyHighscore()
    {
        StartCoroutine("DownloadMyHighscoreFromDatabase");
    }

    IEnumerator DownloadMyHighscoreFromDatabase()
    {
        WWW www = new WWW(webURL + Twister(publicCode,true) + "/pipe-get/" + myId);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            if (!string.IsNullOrEmpty(www.text))
            {
                print(www.text);
                Highscore myHighscore = FormatHighscore(www.text);
                highscoreDisplay.OnMyHighscoreDownloaded(myHighscore);
                myOnlineHighscore = myHighscore.score;

                if (gameLogic)
                {
                    gameLogic.SetLocalHighscore(myHighscore.score);
                }
            }
        }
        else
        {
            print("Error Downloading: " + www.error);
        }
    }

    public void DownloadHighscores()
    {
        StartCoroutine("DownloadHighscoresFromDatabase");
    }

    IEnumerator DownloadHighscoresFromDatabase()
    {
        WWW www = new WWW(webURL + Twister(publicCode,true) + "/pipe/" + "0/25");
        //WWW www = new WWW(webURL + publicCode + "/pipe-get/" + "ID414");
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            FormatHighscores(www.text);
            highscoreDisplay.OnHighscoresDownloaded(highscoresList);
        }
        else
        {
            print("Error Downloading: " + www.error);
        }
    }

    void FormatHighscores(string textStream)
    {
        string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        highscoresList = new Highscore[entries.Length];

        for (int i = 0; i < entries.Length; i++)
        {

            highscoresList[i] = FormatHighscore(entries[i]);
        }
        print(entries.Length + " Scores");
        print(textStream);
    }

    Highscore FormatHighscore(string text)
    {
        string[] entryInfo = text.Split(new char[] { '|' });
        string id = entryInfo[0];
        string username = entryInfo[3]; //Since 0 is now the ID
        int score = int.Parse(entryInfo[1]);
        int place = int.Parse(entryInfo[entryInfo.Length - 1]);
        bool isMe = entryInfo[0] == myId;
        if (isMe)
        {
            myOnlineHighscore = score;
        }
        return new Highscore(username, score, isMe, place, id);
    }
}

public struct Highscore
{
    public string username;
    public int score;
    public bool isMe;
    public int place;
    public string id;

    public Highscore(string _username, int _score, bool _isMe, int _place, string _id)
    {
        username = _username;
        score = _score;
        isMe = _isMe;
        place = _place;
        id = _id;
    }
}
