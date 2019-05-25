using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardNoDoubleJump : RewardButton
{

    public override void RecieveReward()
    {
        GameLogicOpitions.allowDoubleJump = false;
    }
}
