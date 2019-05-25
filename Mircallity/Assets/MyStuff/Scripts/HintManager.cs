using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HintManager : MonoBehaviour {
    public Text textBox;
    public string[] randomHints;
    public string[] rareHints;
    public string[] rewardHints;
    Color colorNormal;
    public Color colorReward;

    public static HintManager instance;

    [Range(0f, 1f)]
    public float showPercentage;

    bool isHints = false;

    public void Start()
    {
        colorNormal = textBox.color;
        ToggleHints(true);
        instance = this;
    }

    public static void CheckHints()
    {
        if (Random.Range(0f, 1f) < instance.showPercentage)
        {
            ResetColor();
            instance.SelectRandomHint();
            SetColor(instance.colorNormal);
            instance.ToggleHints(true);
        }
        else
        {
            instance.ToggleHints(false);
        }
    }

    public void ToggleHints(bool flag)
    {
        isHints = flag;
        textBox.enabled = flag;
        if (!flag)
        {
            textBox.text = "";
        }
    }
    public static void SetText(string myText)
    {
        instance.textBox.text = myText;
    }

    public static void SetColor(Color color)
    {
        instance.textBox.color = color;
    }
    public static void ResetColor()
    {
        SetColor(instance.colorNormal);
    }

    public void SelectRandomHint()
    {
        string randomHint = randomHints[Random.Range(0, randomHints.Length)];
        randomHint = Random.Range(0f, 1f) < 0.1f ? rareHints[Random.Range(0, rareHints.Length)] : randomHint;

        SetText(randomHint);
    }

    public void RewardPlayer()
    {
        SetText(rewardHints[Random.Range(0, rewardHints.Length)]);
        SetColor(colorReward);
    }
    
}
