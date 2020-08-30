using UnityEngine.UI;
using UnityEngine;

public class HeatGauge : MonoBehaviour {

    public static HeatGauge hg_Single;
    public Image fillImg;

    private void Awake() {
        hg_Single = this;
    }
}