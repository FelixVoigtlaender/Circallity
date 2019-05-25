using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//This blueprint for Rewardbuttons  
public class RewardButton : MonoBehaviour {
    public RewardManager rewardManager;


    Image image;

    public virtual void Start()
    {
        image = GetComponent<Image>();
    }

    public virtual void SelectMe()
    {
        rewardManager.SelectReward(this);
    }

    public virtual void SetColor(Color color)
    {
        if (!image)
        {
            image = GetComponent<Image>();
        }
        image.color = color;
    }
    
    public virtual void RecieveReward()
    {
        print("Reward Not Setup!");     //Missing !
    }
}
