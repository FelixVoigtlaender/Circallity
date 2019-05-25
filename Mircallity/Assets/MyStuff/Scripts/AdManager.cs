using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;


public class AdManager : MonoBehaviour {
    public int showEveryN = 0;
    public RewardManager rewardManager;
    int n = 0;

    public void ShowAd()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show();
        }
        
    }
    public void CheckAd()
    {
        if (WillShowAd())
        {
            ShowAd();
            n = 0;
        }
        else
        {
            n++;
        }
    }

    public void DelayAd()
    {
        n = 0;
    }

    public bool WillShowAd()
    {
        return n >= showEveryN;
    }

    public void ShowRewardedAd()
    {
        if (Advertisement.IsReady("rewardedVideo"))
        {
            var options = new ShowOptions { resultCallback = HandleShowResult };
            Advertisement.Show("rewardedVideo", options);
        }
    }

    private void HandleShowResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("The ad was successfully shown.");
                rewardManager.Reward();
                break;
            case ShowResult.Skipped:
                Debug.Log("The ad was skipped before reaching the end.");
                break;
            case ShowResult.Failed:
                Debug.LogError("The ad failed to be shown.");
                break;
        }
    }
}
