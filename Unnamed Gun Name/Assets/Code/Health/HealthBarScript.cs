using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthBarScript : MonoBehaviour {
    public int healthPartsInBar = 10;
    public GameObject healthPartPrefab;
    public Transform partsHolder;

    [Header("HideInInspector")]
    public List<Image> healthPart = new List<Image>();

    void Start() {
        if (!partsHolder) {
            partsHolder = transform;
        }

        for (int i = 0; i < healthPartsInBar; i++) {
            GameObject part = Instantiate(healthPartPrefab, partsHolder);
            Image partImg = part.GetComponent<Image>();
            healthPart.Add(partImg);
        }
    }
    public void ChangeHealth(int currentHealth, int maxHealth) {
        int partAmount = currentHealth / healthPartsInBar;
        int remain = currentHealth % healthPartsInBar;

        for (int i = 0; i < healthPart.Count; i++) {
            float fill;
            Color color = Color.white;
            if (i < partAmount) {
                fill = 1f;
            } else {
                if(i == partAmount) {
                    fill = (float)remain/healthPartsInBar;
                    color = Color.HSVToRGB(0, 1-fill, 1);
                    color.a = 1;
                } else {
                    fill = 0;
                }
            }
            healthPart[i].fillAmount = fill;
            healthPart[i].color = color;
        }
    }
}