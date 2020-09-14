using UnityEngine.UI;
using UnityEngine;
using Photon.Pun;
using System.Collections;

public class Health : MonoBehaviourPun {
    public float maxHealth = 50f;
    public Image fillHealthBar;

    public int respawnTime = 6;

    [HideInInspector] public bool isDead, respawning;
    [HideInInspector] public float currentHealth;
    [HideInInspector] public Controller controller;

    private void Awake() {
        currentHealth = maxHealth;
    }
    
    IEnumerator Countdown() {
        UiManager.single_UM.respawnUiHolder.SetActive(true);
        int timer = respawnTime;
        while (timer > 0) {
            if (Manager.single_M.dev) { break; }
            UiManager.single_UM.respawnAnim.SetTrigger("Respawn");
            UiManager.single_UM.respawnTimer.text = timer.ToString();
            yield return new WaitForSeconds(1);
            timer--;
        }
        Respawn();
    }

    void Respawn() {
        UiManager.single_UM.respawnUiHolder.SetActive(false);
        if (!Manager.single_M.dev) {
            controller.ResetAtStartPosition();
        }
        photonView.RPC("RPC_ChangeHealth", RpcTarget.All, maxHealth, 1);
        isDead = false;
        respawning = false;
    }

    public void DoDamage(float value) {
        if (!isDead) {
            photonView.RPC("RPC_ChangeHealth", RpcTarget.All, value, 0);
        }
    }

    [PunRPC]
    void RPC_ChangeHealth(float value, int add) {
        if (add == 1) {
            currentHealth += value;
        } else {
            currentHealth -= value;
        }
        if (currentHealth <= 0) {
            isDead = true;
            currentHealth = 0;
            if (photonView.IsMine) {
                if (!respawning) {
                    respawning = true;
                    StartCoroutine(Countdown());
                }
            }
        } else {
            isDead = false;
        }
        UpdateUiHeath();
    }

    void UpdateUiHeath() { 
        if (fillHealthBar) {
            fillHealthBar.fillAmount = currentHealth / maxHealth;
        }
    }
}