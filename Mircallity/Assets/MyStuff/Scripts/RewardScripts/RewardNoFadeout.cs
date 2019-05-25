using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardNoFadeout : RewardButton
{
    public override void RecieveReward()
    {
        GameLogicOpitions.isFadeout = false;
    }
}
