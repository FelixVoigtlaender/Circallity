using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLogicOpitions : MonoBehaviour {
    public static GameLogicOpitions instance;
    //Player - Color
    public Color standardPlayerColor;
    public static Color playerColor = Color.red;
    //Generation - isBall
    public static bool isFadeout, isKill, isDoubleJump;

    public static bool allowDoubleJump, allowDoubleBall, allowNormal;

    public void Awake()
    {
        instance = this;
        ResetGameLogicOptions();
    }

    public static void ResetGameLogicOptions()
    {
        //playerColor = instance.standardPlayerColor;
        isFadeout = isDoubleJump = isKill = true;
        allowDoubleJump = allowDoubleBall = allowNormal = true;
    }
}
