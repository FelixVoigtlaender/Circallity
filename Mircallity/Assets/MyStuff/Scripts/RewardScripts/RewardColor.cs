using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardColor : RewardButton {

    public Image displayColor;
	// Use this for initialization
	public override void Start () {
        base.Start();
        displayColor.color = Random.ColorHSV();
	}

    public override void RecieveReward()
    {
        GameLogicOpitions.playerColor = displayColor.color;
    }
}
