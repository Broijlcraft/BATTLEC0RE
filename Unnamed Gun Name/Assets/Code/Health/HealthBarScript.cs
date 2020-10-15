using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthBarScript : MonoBehaviour {
    public int healthPartsInBar = 10;
    public GameObject healthPartPrefab;
    public Transform partsHolder;

    [Header("HideInInspector")]
    public List<HealthPartScript> healthParts = new List<HealthPartScript>();

    void Start() {
        if (!partsHolder) {
            partsHolder = transform;
        }

        for (int i = 0; i < healthPartsInBar; i++) {
            GameObject part = Instantiate(healthPartPrefab, partsHolder);
            HealthPartScript hps = part.GetComponent<HealthPartScript>();
            healthParts.Add(hps);
        }
    }

    public void ChangeHealth(int currentHealth, int maxHealth) {
        int partAmount = currentHealth / healthPartsInBar;
        int remain = currentHealth % healthPartsInBar;
        float H, S, V;

        for (int i = 0; i < healthParts.Count; i++) {
            HealthPartScript hp = healthParts[i];
            float fill;
            Color imgColor = Color.white;
            Color outlineColor = hp.outlineColor;
            if (i < partAmount) {
                fill = 1f;
            } else {
                if(i == partAmount) {
                    fill = (float)remain/healthPartsInBar;
                    imgColor = Color.HSVToRGB(0, 1-fill, 1);
                    imgColor.a = 1;
                    Color.RGBToHSV(outlineColor, out H, out S, out V);
                    outlineColor = Color.HSVToRGB(0, 1-fill, V);
                } else {
                    fill = 0;
                }
            }
            Image image = healthParts[i].img;
            image.fillAmount = fill;
            image.color = imgColor;
            hp.outline.effectColor = outlineColor;
        }
    }
}