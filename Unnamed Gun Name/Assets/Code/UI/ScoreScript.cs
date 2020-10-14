using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreScript : MonoBehaviour {
    public HudScoreListing left, right;

    private void Start() {
        if (left.fillImage && right.fillImage) {
            left.fillImage.fillAmount = 0;
            right.fillImage.fillAmount = 0;
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
}