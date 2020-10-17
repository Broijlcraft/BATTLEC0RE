using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreScript : MonoBehaviour {
    public static ScoreScript single_ss;

    public int maxScore;
    public HudScoreListing[] scoreListings = new HudScoreListing[2];

    private void Awake() {
        if (!ScoreScript.single_ss) {
            single_ss = this;
        }
    }

    private void Start() {
        for (int i = 0; i < scoreListings.Length; i++) {
            HudScoreListing sl = scoreListings[i];

            sl.fillImage.fillAmount = 0;
            sl.scoreText.text = "";
            sl.maxScore = maxScore;
            sl.IncreaseScore(0);
        }
    }
}

public enum LeftRight {
    left,
    right
}

[System.Serializable]
public class HudScoreListing {
    public Image fillImage;
    public Text scoreText;

    public string symbolBetweenScore = "/";

    [Header("HideInInspector")]
    public int maxScore, currentScore;

    public void IncreaseScore(int value) {
        currentScore += value;
        if(currentScore >= maxScore) {
            currentScore = maxScore;
        }

        float fill = 0;
        if(currentScore != 0) {
            fill = (float)currentScore/maxScore;
        }
        fillImage.fillAmount = fill;

        scoreText.text = currentScore + $" {symbolBetweenScore} " + maxScore;
    }
}