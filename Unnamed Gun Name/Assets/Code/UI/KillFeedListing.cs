using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class KillFeedListing : MonoBehaviour {
    public Image weapon, headshot;
    public Text killerName, victimName;
    public Animator animator;
    public float timeBeforeFade = 5f, timeToFadeOut = 1f;

    bool isActive;
    float timer;

    private void Update() {
        if (isActive) {
            timer += Time.deltaTime;
            if (timer > timeBeforeFade) {
                animator.ResetTrigger("Default");
                animator.SetTrigger("FadeOut");
                print("fade");
                timer = 0;
                isActive = false;
            }
        }
    }

    public void SetKill(string killer, string victim, Sprite weaponIcon, Sprite headshotIcon, bool isHeadshot) {
        gameObject.SetActive(true);
        ResetFadeOut();
        transform.SetAsLastSibling();
        headshot.gameObject.SetActive(isHeadshot);
        weapon.sprite = weaponIcon;
        headshot.sprite = headshotIcon;
        killerName.text = killer;
        victimName.text = victim;
        
        isActive = true;
    }

    void ResetFadeOut() {
        print("reset");
        isActive = false;
        timer = 0;
        animator.ResetTrigger("FadeOut");
        animator.SetTrigger("Default");
    }
}