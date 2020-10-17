using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthBarScript : MonoBehaviour {
    public int healthPartsInBar = 10;
    public GameObject healthPartPrefab;
    public Transform partsHolder;

    public float partSpawnTimer = 0.1f;

    [Header("HideInInspector")]
    public List<HealthPartScript> healthParts = new List<HealthPartScript>();
    Controller cl;

    public void Init(Controller control) {
        cl = control;
        if (!partsHolder) {
            partsHolder = transform;
        }

        for (int i = 0; i < healthPartsInBar; i++) {
            GameObject part = Instantiate(healthPartPrefab, partsHolder);
            HealthPartScript hps = part.GetComponent<HealthPartScript>();
            hps.img.gameObject.SetActive(false);
            healthParts.Add(hps);
        }
        StartCoroutine(ShowPartsOverTime(null));
    }

    public IEnumerator ShowPartsOverTime(Health health) {
        print("start");
        if (cl) { 
            if (!cl.playerView.devView) {
                cl.canMove = false;
            }
        }

        for (int i = 0; i < healthParts.Count; i++) {
            yield return new WaitForSeconds(partSpawnTimer);
            HealthPartScript hp = healthParts[i];
            Image img = hp.img;
            img.gameObject.SetActive(true);
            img.color = Color.white;
            img.fillAmount = 1f;
            hp.outline.effectColor = hp.outlineColor;
        }

        if (cl) {
            if (!cl.playerView.devView) {
                cl.canMove = true;
            }
        }
        if (health) {
            health.respawning = false;
        }
        print("Done");
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