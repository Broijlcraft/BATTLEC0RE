using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthPartScript : MonoBehaviour {
    public Image img;
    public Outline outline;
    [HideInInspector] public Color outlineColor;

    private void Start() {
        outlineColor = outline.effectColor;
    }
}