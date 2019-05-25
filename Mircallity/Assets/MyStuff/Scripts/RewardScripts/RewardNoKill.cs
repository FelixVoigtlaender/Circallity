using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardNoKill : RewardButton {

    public override void RecieveReward()
    {
        GameLogicOpitions.isKill = false;
    }
}
