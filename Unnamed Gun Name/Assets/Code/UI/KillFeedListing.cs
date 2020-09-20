using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class KillFeedListing : MonoBehaviour {
    public Image weapon, headshot;
    public Text killerName, victimName;
    public Animator animator;
    public float maxTimeOnScreen = 5f, timeToFadeOut = 1f;

    public void SetKill(string killer, string victim, Sprite weaponIcon, Sprite headshotIcon, bool isHeadshot) {
        StopCoroutine(Hide());
        gameObject.SetActive(true);
        animator.ResetTrigger("FadeOut");
        animator.SetTrigger("Default");
        transform.SetAsLastSibling();
        headshot.gameObject.SetActive(isHeadshot);
        weapon.sprite = weaponIcon;
        headshot.sprite = headshotIcon;
        killerName.text = killer;
        victimName.text = victim;
        StartCoroutine(Hide());
    }

    IEnumerator Hide() {
        yield return new WaitForSeconds(maxTimeOnScreen);
        FadeOut();
    }

    void FadeOut() {
        animator.SetTrigger("FadeOut");
    }
}