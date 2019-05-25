using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class RewardManager : MonoBehaviour {

    public int maxRewards = 4;

    [Header("Looks")]
    public float distance;
    public GameObject rewardContainer;
    public GameObject[] rewardPrefabs;
    
    [Header("Logic")]
    public GameLogic gameLogic;
    public AdManager adManager;
    public Menu menu;

    [Header("Button")]
    public Button buttonAccept;
    public Button buttonDecline;
    public Button buttonBack;
    public Color selectedColor;
    public Color unselectedColor;

    List<RewardButton> availableRewards = new List<RewardButton>(); //All current rewards
    RectTransform uiParent;
    AudioSource audioSource;


    RewardButton selectedReward;    //Currently selected reward!
    public bool isReward;


    public void Start()
    {
        audioSource = GetComponent<AudioSource>();
        uiParent = rewardContainer.GetComponent<RectTransform>();
        uiParent.localPosition = Vector2.right * 1000;

        ToggleReward(false);

        GenerateRewards();
        
    }

    public void ToggleReward(bool flag)
    {
        this.isReward = flag;
        uiParent.localPosition = Vector2.right * 1000;
        foreach(Transform t in transform)
        {
            t.gameObject.SetActive(flag);
        }
    }

    public void SelectReward(RewardButton rewardButton)
    {
        
        selectedReward = rewardButton;
        if (availableRewards == null)
        {
            return;
        }
        foreach (RewardButton rb in availableRewards)
        {
            rb.SetColor(rb == rewardButton ? selectedColor : unselectedColor);
        }
    }

    public void RemoveRewards()
    {
        foreach(Transform t in rewardContainer.transform)
        {
            Destroy(t.gameObject);
        }

        availableRewards.Clear();
        selectedReward = null;
    }


    //Rewarding

    public void GenerateRewards()
    {
        RemoveRewards();
        for (int i = 0; i < maxRewards; i++)
        {
            GameObject rewardButton = Instantiate(rewardPrefabs[Random.Range(0, rewardPrefabs.Length)], rewardContainer.transform, false);

            RectTransform uiTransform = rewardButton.GetComponent<RectTransform>();
            float deltaX = (uiTransform.rect.width) * distance;
            float x = i * deltaX + (uiTransform.rect.width) * 0.1f;
            uiTransform.localPosition = Vector2.right * x;

            RewardButton rb = rewardButton.GetComponent<RewardButton>();
            rb.rewardManager = this;
            availableRewards.Add(rb);

            uiParent.sizeDelta = Vector2.right * (x + deltaX) + Vector2.up * 100;
        }

        SelectReward(availableRewards[0]);

        CheckBackButton();
    }

    public void PressAcceptButton()
    {
        OrderReward();
    }
    public void PressDeclineButton()
    {
        RemoveRewards();

        Vibration.Vibrate(50);
        CheckBackButton();
    }
    public void PressBackButton()
    {
        menu.ToggleReward();

        Vibration.Vibrate(20);

    }

    public void OrderReward()
    {
        adManager.ShowRewardedAd();
    }

    public void Reward()
    {
        if (selectedReward)
        {
            selectedReward.RecieveReward();
        }
        adManager.DelayAd();
        RemoveRewards();

        Vibration.Vibrate(50);
        audioSource.Play();

        CheckBackButton();
    }

    public bool CheckBackButton()
    {
        bool isStart = availableRewards.Count == 0;

        buttonBack.gameObject.SetActive(isStart);
        buttonAccept.gameObject.SetActive(!isStart);
        buttonDecline.gameObject.SetActive(!isStart);

        return isStart;
    }
}
