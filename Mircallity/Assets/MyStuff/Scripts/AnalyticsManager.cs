using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviour {

	public void OnHighscoreReached(int highscore)
    {
        Analytics.CustomEvent("highscoreReached", new Dictionary<string, object>
  {
    { "Score", highscore }
  });
    }

    public void OnScore(int score)
    {
        /*
        Analytics.CustomEvent("gameOver", new Dictionary<string, object>
  {
    { "Score", score }
  });
  */
    }
}
