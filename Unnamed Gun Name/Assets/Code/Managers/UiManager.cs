using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class UiManager : MonoBehaviour {
    public static UiManager single_UM;

    public GameObject respawnUiHolder;
    public Animator respawnAnim;
    public Text respawnTimer;
    public Image ingameHealthBar;
    [Space]
    public int maxKillsInFeed = 5;
    public Sprite defaultWeaponIconInFeed, defaultHeadshotIconInFeed;
    public Transform killFeed;
    public GameObject killfeedListingPrefab;
    Queue<KillFeedListing> listings = new Queue<KillFeedListing>();

    private void Awake() {
        single_UM = this;
        respawnUiHolder.SetActive(false);
        for (int i = 0; i < maxKillsInFeed; i++) {
            GameObject listing = Instantiate(killfeedListingPrefab, killFeed);
            KillFeedListing kfl = listing.GetComponent<KillFeedListing>();
            listing.gameObject.SetActive(false);
            listings.Enqueue(kfl);
        }
    }

    public void AddKillToFeed(string killer, string victim, Sprite weaponIcon, Sprite headshotIcon, bool isHeadshot) {
        KillFeedListing listing = listings.Dequeue();

        if (!weaponIcon) {
            weaponIcon = defaultWeaponIconInFeed;
        }
        if (!headshotIcon) {
            headshotIcon = defaultHeadshotIconInFeed;
        }

        listing.SetKill(killer, victim, weaponIcon, headshotIcon, isHeadshot);
        listings.Enqueue(listing);
    }
}